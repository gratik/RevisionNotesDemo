// ==============================================================================
// TIMEOUT AND BULKHEAD PATTERNS - Resource Protection
// ==============================================================================
// PURPOSE:
//   Implement timeout and bulkhead patterns with Polly to prevent resource exhaustion.
//   Timeouts prevent indefinite waits. Bulkheads isolate resources.
//
// WHY THESE PATTERNS:
//   - Timeout: Prevent hanging operations from consuming resources forever
//   - Bulkhead: Isolate failures, prevent one failing dependency from exhausting all resources
//   - Based on ship bulkheads - compartments prevent sinking entire ship
//
// WHAT YOU'LL LEARN:
//   1. Pessimistic timeout (cancellation)
//   2. Optimistic timeout (monitoring)
//   3. Timeout with retry
//   4. Bulkhead isolation
//   5. Bulkhead with queue
//   6. Combining all patterns
//
// TIMEOUT TYPES:
//   - Pessimistic: Cancels operation (HttpClient timeout)
//   - Optimistic: Monitors but doesn't cancel (long-running processes)
//
// BULKHEAD ANALOGY:
//   Ship has watertight compartments. One leak doesn't sink entire ship.
//   Service has resource pools. One slow dependency doesn't exhaust all threads.
// ==============================================================================

using Polly;
using Polly.Timeout;
using Polly.Bulkhead;
using Microsoft.Extensions.Logging;

namespace RevisionNotesDemo.Resilience;

/// <summary>
/// EXAMPLE 1: TIMEOUT POLICY - Preventing Indefinite Waits
/// 
/// THE PROBLEM:
/// External service hangs. Your request waits indefinitely.
/// Threads exhausted waiting for responses that never come.
/// 
/// THE SOLUTION:
/// Set timeout policy - fail fast after maximum wait time.
/// 
/// WHY IT MATTERS:
/// - Free up threads stuck waiting
/// - Predictable SLAs (99th percentile)
/// - Better error handling
/// - Prevents resource exhaustion
/// 
/// TYPES:
/// - Pessimistic: Actively cancels operation (recommended)
/// - Optimistic: Throws but doesn't cancel (for operations that can't be cancelled)
/// </summary>
public static class TimeoutExamples
{
    // ❌ BAD: No timeout - can wait forever
    public static async Task<string> NoTimeout(HttpClient client)
    {
        // If service is down, waits until default HttpClient.Timeout (100s)
        var response = await client.GetAsync("https://slow-api.example.com/data");
        return await response.Content.ReadAsStringAsync();
        // Could wait 100 seconds!
    }
    
    // ✅ GOOD: Pessimistic timeout (actively cancels)
    public static async Task<string> WithPessimisticTimeout(HttpClient client)
    {
        var timeoutPolicy = Policy
            .TimeoutAsync(
                timeout: TimeSpan.FromSeconds(5),
                timeoutStrategy: TimeoutStrategy.Pessimistic);  // ✅ Cancels operation
        
        return await timeoutPolicy.ExecuteAsync(async ct =>
        {
            // ✅ CancellationToken passed through
            var response = await client.GetAsync("https://slow-api.example.com/data", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }, CancellationToken.None);
        
        // If operation takes > 5s, TimeoutRejectedException thrown
        // HttpClient receives cancellation and aborts request
    }
    
    // ✅ GOOD: Optimistic timeout (monitoring only)
    public static async Task<string> WithOptimisticTimeout()
    {
        var timeoutPolicy = Policy
            .TimeoutAsync(
                timeout: TimeSpan.FromSeconds(5),
                timeoutStrategy: TimeoutStrategy.Optimistic);  // ✅ Doesn't cancel, just monitors
        
        return await timeoutPolicy.ExecuteAsync(async () =>
        {
            // Operation continues even after timeout
            await Task.Delay(10000);  // Simulates long operation
            return "Done";
        });
        
        // Use optimistic when:
        // - Operation can't be cancelled
        // - Want to track timeout but let operation complete
        // - Cleanup code must run
    }
    
    // ✅ GOOD: Timeout with logging
    public static async Task<string> TimeoutWithLogging(HttpClient client, ILogger logger)
    {
        var timeoutPolicy = Policy
            .TimeoutAsync(
                timeout: TimeSpan.FromSeconds(5),
                timeoutStrategy: TimeoutStrategy.Pessimistic,
                onTimeoutAsync: (context, timespan, task) =>
                {
                    // ✅ Log timeout events
                    logger.LogWarning("Operation timed out after {Timeout}s", timespan.TotalSeconds);
                    return Task.CompletedTask;
                });
        
        return await timeoutPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://slow-api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
    
    // ✅ GOOD: Different timeouts per operation
    public static class OperationTimeouts
    {
        // Fast operations
        public static readonly TimeSpan FastTimeout = TimeSpan.FromSeconds(2);
        
        // Standard operations
        public static readonly TimeSpan StandardTimeout = TimeSpan.FromSeconds(5);
        
        // Slow operations (reports, exports)
        public static readonly TimeSpan SlowTimeout = TimeSpan.FromSeconds(30);
        
        // Background jobs
        public static readonly TimeSpan BackgroundTimeout = TimeSpan.FromMinutes(5);
    }

/// <summary>
/// EXAMPLE 2: TIMEOUT + RETRY - Handling Slow Responses
/// 
/// THE PROBLEM:
/// Service occasionally slow. Single timeout too aggressive.
/// Want to retry slow operations.
/// 
/// THE SOLUTION:
/// Combine timeout (inner) with retry (outer).
/// 
/// WHY IT MATTERS:
/// - Timeout individual attempts
/// - Retry timeouts (might succeed faster)
/// - Total time bounded
/// </summary>
public static class TimeoutWithRetryExamples
{
    // ✅ GOOD: Retry slow operations
    public static async Task<string> RetryWithTimeout(HttpClient client, ILogger logger)
    {
        // ✅ Inner policy: Timeout each attempt
        var timeoutPolicy = Policy
            .TimeoutAsync(
                timeout: TimeSpan.FromSeconds(5),
                timeoutStrategy: TimeoutStrategy.Pessimistic);
        
        // ✅ Outer policy: Retry timeouts
        var retryPolicy = Policy
            .Handle<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                retryCount: 2,  // Total attempts: 3
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    logger.LogWarning("Request timed out, retry {RetryCount}", retryCount);
                });
        
        // ✅ Wrap timeout with retry
        var policyWrap = retryPolicy.WrapAsync(timeoutPolicy);
        
        return await policyWrap.ExecuteAsync(async ct =>
        {
            var response = await client.GetAsync("https://slow-api.example.com/data", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }, CancellationToken.None);
        
        // Timeline:
        // 0-5s:   Attempt 1 (timeout)
        // 6-11s:  Attempt 2 (timeout)
        // 12-17s: Attempt 3 (timeout or success)
        // Max total time: ~17 seconds
    }
    
    // ✅ GOOD: Exponential timeout (each retry gets more time)
    public static async Task<string> ExponentialTimeout(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<TimeoutRejectedException>()
            .RetryAsync(3);
        
        int attempt = 0;
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            attempt++;
            var attemptTimeout = TimeSpan.FromSeconds(2 * attempt);  // 2s, 4s, 6s, 8s
            
            var timeoutPolicy = Policy.TimeoutAsync(attemptTimeout, TimeoutStrategy.Pessimistic);
            
            return await timeoutPolicy.ExecuteAsync(async ct =>
            {
                var response = await client.GetAsync("https://slow-api.example.com/data", ct);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(ct);
            }, CancellationToken.None);
        });
    }
}

/// <summary>
/// EXAMPLE 3: BULKHEAD PATTERN - Resource Isolation
/// 
/// THE PROBLEM:
/// One slow dependency exhausts all threads.
/// All requests blocked waiting for slow service.
/// 
/// THE SOLUTION:
/// Bulkhead limits concurrent executions per dependency.
/// 
/// WHY IT MATTERS:
/// - Isolate failures
/// - Reserve resources for other operations
/// - Prevent cascading failures
/// - Fair resource allocation
/// 
/// ANALOGY: Ship bulkheads - one  compartment floods, others stay dry
/// </summary>
public static class BulkheadExamples
{
    // ❌ BAD: No isolation - slow service exhausts all threads
    public static async Task<string> NoIsolation(HttpClient slowService)
    {
        // If slowService hangs, ALL threads can get stuck here
        var response = await slowService.GetAsync("/data");
        return await response.Content.ReadAsStringAsync();
        
        // Scenario:
        // - Thread pool: 100 threads
        // - Slow service: 2s response time
        // - 200 concurrent requests
        // - Result: All 100 threads stuck on slow service
        // - Other operations: Starved of threads
    }
    
    // ✅ GOOD: Bulkhead limits concurrent calls
    public static async Task<string> WithBulkhead(HttpClient slowService)
    {
        var bulkheadPolicy = Policy
            .BulkheadAsync(
                maxParallelization: 10,  // ✅ Max 10 concurrent executions
                maxQueuingActions: 20);   // ✅ Max 20 waiting in queue
        
        return await bulkheadPolicy.ExecuteAsync(async () =>
        {
            var response = await slowService.GetAsync("/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Scenario with bulkhead:
        // - 200 concurrent requests
        // - 10 executing
        // - 20 queued
        // - 170 rejected immediately with BulkheadRejectedException
        // - Other operations: Still have 90 threads available
    }
    
    // ✅ GOOD: Multiple bulkheads per service
    public class IsolatedServiceCalls
    {
        // ✅ Separate bulkhead for each external service
        private static readonly AsyncBulkheadPolicy PaymentServiceBulkhead = Policy
            .BulkheadAsync(maxParallelization: 20, maxQueuingActions: 50);
        
        private static readonly AsyncBulkheadPolicy InventoryServiceBulkhead = Policy
            .BulkheadAsync(maxParallelization: 30, maxQueuingActions: 100);
        
        private static readonly AsyncBulkheadPolicy NotificationServiceBulkhead = Policy
            .BulkheadAsync(maxParallelization: 10, maxQueuingActions: 20);
        
        public async Task<string> CallPaymentService(HttpClient client)
        {
            return await PaymentServiceBulkhead.ExecuteAsync(async () =>
            {
                var response = await client.GetAsync("/payment");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }
        
        // Now each service has isolated resources
        // Slow payment service won't affect inventory or notifications
    }
    
    // ✅ GOOD: Bulkhead with rejection handling
    public static async Task<string> BulkheadWithRejectionHandling(HttpClient client, ILogger logger)
    {
        var bulkheadPolicy = Policy
            .BulkheadAsync(
                maxParallelization: 10,
                maxQueuingActions: 20,
                onBulkheadRejectedAsync: context =>
                {
                    // ✅ Log rejections
                    logger.LogWarning("Request rejected by bulkhead - system overloaded");
                    return Task.CompletedTask;
                });
        
        try
        {
            return await bulkheadPolicy.ExecuteAsync(async () =>
            {
                var response = await client.GetAsync("/data");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }
        catch (BulkheadRejectedException)
        {
            // ✅ Handle rejection gracefully
            logger.LogWarning("Service overloaded, please try again");
            throw new InvalidOperationException("Service temporarily unavailable");
        }
    }
}

/// <summary>
/// EXAMPLE 4: BULKHEAD WITH FALLBACK AND MONITORING
/// 
/// THE PROBLEM:
/// Bulkhead rejection throws exception.
/// Need graceful degradation and monitoring.
/// 
/// THE SOLUTION:
/// Wrap bulkhead with fallback, track metrics.
/// </summary>
/* NOTE: This example requires careful mixing of generic and non-generic Polly policies.
 * Commented out due to Polly API compatibility issues.
 * For Polly V8+, use the new ResiliencePipeline API instead.
public static class BulkheadWithFallbackExamples
{
    // ✅ GOOD: Bulkhead + fallback
    public static async Task<string> BulkheadWithFallback(HttpClient client, ICache cache, ILogger logger)
    {
        var bulkheadPolicy = Policy<string>
            .BulkheadAsync(
                maxParallelization: 10,
                maxQueuingActions: 20);
        
        var fallbackPolicy = Policy<string>
            .Handle<BulkheadRejectedException>()
            .FallbackAsync(
                fallbackAction: async (context, cancellationToken) =>
                {
                    logger.LogWarning("Bulkhead full, returning cached data");
                    return await cache.GetAsync("data") ?? "Service busy, please try again";
                });
        
        var policyWrap = fallbackPolicy.WrapAsync(bulkheadPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
*/
    // ✅ GOOD: Bulkhead with metrics
    public class MonitoredBulkhead
    {
        private readonly AsyncBulkheadPolicy _bulkheadPolicy;
        private readonly ILogger _logger;
        private int _rejectionCount;
        private int _executionCount;
        
        public int RejectionCount => _rejectionCount;
        public int ExecutionCount => _executionCount;
        public int Available => 10 - ExecutionCount;  // Assuming max 10
        
        public MonitoredBulkhead(ILogger logger)
        {
            _logger = logger;
            
            _bulkheadPolicy = Policy
                .BulkheadAsync(
                    maxParallelization: 10,
                    maxQueuingActions: 20,
                    onBulkheadRejectedAsync: context =>
                    {
                        Interlocked.Increment(ref _rejectionCount);
                        _logger.LogWarning("Bulkhead rejection count: {Count}", _rejectionCount);
                        return Task.CompletedTask;
                    });
        }
        
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            Interlocked.Increment(ref _executionCount);
            try
            {
                return await _bulkheadPolicy.ExecuteAsync(action);
            }
            finally
            {
                Interlocked.Decrement(ref _executionCount);
            }
        }
    }
}

/// <summary>
/// EXAMPLE 5: COMBINING ALL PATTERNS - Production-Grade Resilience
/// 
/// THE PROBLEM:
/// Real systems need multiple resilience patterns together.
/// 
/// THE SOLUTION:
/// Wrap policies: Fallback → CircuitBreaker → Timeout → Bulkhead → Retry
/// 
/// WHY IT MATTERS:
/// - Complete resilience strategy
/// - Handles all failure modes
/// - Production-ready
/// 
/// ORDER (outermost to innermost):
/// 1. Fallback (outermost) - last resort
/// 2. Circuit Breaker - stops sustained failures
/// 3. Timeout - bounds per-attempt time
/// 4. Bulkhead - limits concurrent calls
/// 5. Retry (innermost) - handles transient failures
/// </summary>
/* NOTE: This example requires careful mixing of generic and non-generic Polly policies.
 * Commented out due to Polly V7/V8 API compatibility issues.
 * For Polly V8+, use the new ResiliencePipeline API instead.
 
public static class CombinedPatternsExample
{
    // ✅ BEST PRACTICE: Full resilience stack
    public static async Task<string> FullResilienceStack(
        HttpClient client,
        ICache cache,
        ILogger logger)
    {
        // 1. Retry (innermost) - handle transient failures
        var retryPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (result, timespan, retryCount, context) =>
                {
                    logger.LogWarning("Retry attempt {RetryCount}", retryCount);
                });
        
        // 2. Bulkhead - limit concurrent executions
        var bulkheadPolicy = Policy<string>
            .BulkheadAsync(
                maxParallelization: 10,
                maxQueuingActions: 20,
                onBulkheadRejectedAsync: context =>
                {
                    logger.LogWarning("Bulkhead rejected request");
                    return Task.CompletedTask;
                });
        
        // 3. Timeout - bound execution time per attempt
        var timeoutPolicy = Policy
            .TimeoutAsync(
                timeout: TimeSpan.FromSeconds(5),
                timeoutStrategy: TimeoutStrategy.Pessimistic,
                onTimeoutAsync: (context, timespan, task) =>
                {
                    logger.LogWarning("Request timed out after {Timeout}s", timespan.TotalSeconds);
                    return Task.CompletedTask;
                });
        
        // 4. Circuit Breaker - stop sustained failures
        var circuitBreakerPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .Or<BulkheadRejectedException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    logger.LogWarning("Circuit breaker opened for {Duration}s", duration.TotalSeconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker closed");
                });
        
        // 5. Fallback (outermost) - last resort
        var fallbackPolicy = Policy<string>
            .Handle<BrokenCircuitException>()
            .Or<BulkheadRejectedException>()
            .Or<TimeoutRejectedException>()
            .Or<HttpRequestException>()
            .FallbackAsync(
                fallbackAction: async (context, cancellationToken) =>
                {
                    logger.LogWarning("Falling back to cached data");
                    return await cache.GetAsync("data") ?? "Service unavailable";
                });
        
        // ✅ Wrap all policies: Fallback wraps CircuitBreaker wraps Timeout wraps Bulkhead wraps Retry
        var policyWrap = fallbackPolicy
            .WrapAsync(circuitBreakerPolicy)
            .WrapAsync(timeoutPolicy)
            .WrapAsync(bulkheadPolicy)
            .WrapAsync(retryPolicy);
        
        return await policyWrap.ExecuteAsync(async ct =>
        {
            var response = await client.GetAsync("https://api.example.com/data", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }, CancellationToken.None);
        
        // Flow:
        // → Request enters fallback
        // → Passes to circuit breaker (if closed)
        // → Passes to timeout (5s limit)
        // → Passes to bulkhead (if slots available)
        // → Passes to retry (will retry transient failures)
        // → Makes HTTP call
        //
        // On any failure at any level:
        // ← Propagates up through policies
        // ← Each policy handles or passes up
        // ← Fallback catches everything and returns cached data
    }
    
    // ✅ GOOD: Configurable resilience
    public class ConfigurableResilientHttpClient
    {
        private readonly HttpClient _client;
        private readonly IAsyncPolicy<string> _policy;
        
        public ConfigurableResilientHttpClient(
            HttpClient client,
            ICache cache,
            ILogger logger,
            ResilienceConfig config)
        {
            _client = client;
            
            // Build pipeline from config
            var retry = Policy<string>
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    config.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            var bulkhead = Policy<string>
                .BulkheadAsync(config.MaxParallelization, config.MaxQueueSize);
            
            var timeout = Policy
                .TimeoutAsync(config.Timeout, TimeoutStrategy.Pessimistic);
            
            var circuitBreaker = Policy<string>
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    config.CircuitBreakerThreshold,
                    config.CircuitBreakerDuration);
            
            var fallback = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(async (context, ct) =>
                    await cache.GetAsync("data") ?? config.FallbackValue);
            
            _policy = fallback
                .WrapAsync(circuitBreaker)
                .WrapAsync(timeout)
                .WrapAsync(bulkhead)
                .WrapAsync(retry);
        }
        
        public Task<string> GetAsync(string url)
        {
            return _policy.ExecuteAsync(async ct =>
            {
                var response = await _client.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(ct);
            }, CancellationToken.None);
        }
    }
    
    public class ResilienceConfig
    {
        public int RetryCount { get; set; } = 3;
        public int MaxParallelization { get; set; } = 10;
        public int MaxQueueSize { get; set; } = 20;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
        public string FallbackValue { get; set; } = "Service unavailable";
    }
}
*/

// SUMMARY - Pattern Selection Guide:
//
// USE TIMEOUT when:
// ✅ Calling external services
// ✅ Operations with SLA requirements
// ✅ Preventing indefinite waits
//
// USE BULKHEAD when:
// ✅ Multiple external dependencies
// ✅ Want to isolate failures
// ✅ Need resource fairness
// ✅ Microservices architecture
//
// COMBINE PATTERNS when:
// ✅ Production systems
// ✅ High availability requirements
// ✅ Multiple failure modes possible
//
// CONFIGURATION GUIDELINES:
// Timeout:
// - Fast operations: 2-5s
// - Standard operations: 5-10s
// - Slow operations: 30-60s
//
// Bulkhead:
// - Critical services: Conservative (10-20 concurrent)
// - Non-critical: More generous (50-100 concurrent)
// - Queue size: 2-5x parallelization
//
// Policy Order (outside → inside):
// Fallback → CircuitBreaker → Timeout → Bulkhead → Retry
