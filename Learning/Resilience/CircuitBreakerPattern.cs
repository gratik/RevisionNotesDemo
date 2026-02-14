// ==============================================================================
// CIRCUIT BREAKER PATTERN - Preventing Cascading Failures
// ==============================================================================
// PURPOSE:
//   Implement circuit breaker pattern with Polly to protect systems from cascading failures.
//   Automatically stop calling failing services, allow recovery, then resume.
//
// WHY CIRCUIT BREAKER:
//   - Prevent resource exhaustion (threads, connections)
//   - Fail fast instead of waiting for timeout
//   - Give failing service time to recover
//   - Prevent cascading failures across microservices
//   - Based on Michael Nygard's "Release It!"
//
// WHAT YOU'LL LEARN:
//   1. Circuit breaker states (Closed, Open, Half-Open)
//   2. Basic circuit breaker setup
//   3. Advanced circuit breaker (thresholds, break duration)
//   4. Circuit breaker with fallback
//   5. Monitoring circuit breaker state
//   6. Circuit breaker + retry combination
//
// CIRCUIT BREAKER STATES:
//   CLOSED: Normal operation, requests flow through
//   OPEN: Failure threshold reached, requests fail immediately  
//   HALF-OPEN: Testing if service recovered, limited requests allowed
//
// ANALOGY: Like an electrical circuit breaker - trips when overloaded, resets after cooling
// ==============================================================================

using Polly;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace RevisionNotesDemo.Resilience;

/// <summary>
/// EXAMPLE 1: CIRCUIT BREAKER BASICS - How It Works
/// 
/// THE PROBLEM:
/// Service is down. Clients keep retrying, wasting resources.
/// Retries tie up threads, exhaust connection pools, increase latency.
/// 
/// THE SOLUTION:
/// Circuit breaker stops calling failing service after threshold reached.
/// Fails fast, gives service time to recover.
/// 
/// WHY IT MATTERS:
/// - Prevents thread pool exhaustion
/// - Reduces mean time to recovery
/// - Protects downstream service from request flood
/// - Improves user experience (fast failure > slow timeout)
/// 
/// STATES:
/// 1. CLOSED (normal): Requests flow, failures counted
/// 2. OPEN (tripped): Requests fail immediately, no calls made
/// 3. HALF-OPEN (testing): Allow one request to test recovery
/// </summary>
public static class CircuitBreakerBasics
{
    // ❌ BAD: No circuit breaker - keeps hammering failing service
    public static async Task<string> NoCircuitBreaker(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            // ❌ Exception on every call, tying up resources
            throw;
        }
        // If service is down, EVERY call waits for timeout (e.g., 30s)
        // 100 concurrent users = 100 threads blocked for 30s each
    }
    
    // ✅ GOOD: Simple circuit breaker
    public static async Task<string> WithCircuitBreaker(HttpClient client)
    {
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,       // ✅ Break after 3 consecutive failures
                durationOfBreak: TimeSpan.FromSeconds(30));  // ✅ Stay open for 30s
        
        return await circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Flow:
        // Attempt 1: Fails → Count = 1 (CLOSED)
        // Attempt 2: Fails → Count = 2 (CLOSED)
        // Attempt 3: Fails → Count = 3 → Circuit OPENS
        // Attempt 4+: BrokenCircuitException (immediate, no HTTP call)
        // After 30s: Circuit moves to HALF-OPEN
        // Next attempt: If succeeds → Circuit CLOSES, if fails → Circuit OPENS again
    }
    
    // ✅ GOOD: Circuit breaker with logging
    public static async Task<string> WithLogging(HttpClient client, ILogger logger)
    {
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    // ✅ Log when circuit opens
                    logger.LogWarning(exception,
                        "Circuit breaker OPENED for {Duration}s due to: {Message}",
                        duration.TotalSeconds, exception.Message);
                },
                onReset: () =>
                {
                    // ✅ Log when circuit closes
                    logger.LogInformation("Circuit breaker CLOSED - service recovered");
                },
                onHalfOpen: () =>
                {
                    // ✅ Log when testing recovery
                    logger.LogInformation("Circuit breaker HALF-OPEN - testing if service recovered");
                });
        
        return await circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}

/// <summary>
/// EXAMPLE 2: ADVANCED CIRCUIT BREAKER - Sampling and Thresholds
/// 
/// THE PROBLEM:
/// Simple circuit breaker uses consecutive failures.
/// In high-traffic scenarios, need percentage-based thresholds.
/// 
/// THE SOLUTION:
/// Use AdvancedCircuitBreakerAsync with failure rate and minimum throughput.
/// 
/// WHY IT MATTERS:
/// - More accurate for high-volume services
/// - Won't trip on isolated failures
/// - Configurable sampling period
/// - Industry-standard approach
/// 
/// EXAMPLE: 
/// - 50% failure rate over 10 seconds window
/// - Minimum 20 requests in window
/// </summary>
public static class AdvancedCircuitBreakerExamples
{
    // ✅ GOOD: Advanced circuit breaker with failure percentage
    public static async Task<string> PercentageBasedCircuitBreaker(HttpClient client, ILogger logger)
    {
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode >= HttpStatusCode.InternalServerError)
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,                    // ✅ Break if 50% of requests fail
                samplingDuration: TimeSpan.FromSeconds(10),  // ✅ Over 10-second window
                minimumThroughput: 20,                    // ✅ Need at least 20 requests in window
                durationOfBreak: TimeSpan.FromSeconds(30),   // ✅ Stay open for 30s
                onBreak: (result, duration) =>
                {
                    logger.LogWarning("Circuit breaker OPENED: {FailureRate}% failure rate in last 10s",
                        50);  // Could calculate actual rate from result
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker CLOSED");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker HALF-OPEN - testing recovery");
                });
        
        var response = await circuitBreakerPolicy.ExecuteAsync(() =>
            client.GetAsync("https://api.example.com/data"));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
        
        // Example scenario:
        // 10s window: 30 total requests
        // - 20 succeed
        // - 10 fail (33% failure rate)
        // Circuit stays CLOSED (< 50% threshold)
        //
        // 10s window: 40 total requests (meets minimum throughput)
        // - 15 succeed
        // - 25 fail (62.5% failure rate)
        // Circuit OPENS (> 50% threshold)
    }
    
    // ✅ GOOD: Configurable advanced circuit breaker
    public static async Task<string> ConfigurableCircuitBreaker(
        HttpClient client,
        CircuitBreakerConfig config,
        ILogger logger)
    {
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode >= HttpStatusCode.InternalServerError)
            .AdvancedCircuitBreakerAsync(
                failureThreshold: config.FailureThreshold,
                samplingDuration: config.SamplingDuration,
                minimumThroughput: config.MinimumThroughput,
                durationOfBreak: config.DurationOfBreak,
                onBreak: (result, duration) =>
                {
                    logger.LogWarning("Circuit OPENED for {Service}", config.ServiceName);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit CLOSED for {Service}", config.ServiceName);
                });
        
        var response = await circuitBreakerPolicy.ExecuteAsync(() =>
            client.GetAsync(config.Endpoint));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    // Configuration class
    public class CircuitBreakerConfig
    {
        public string ServiceName { get; init; } = string.Empty;
        public string Endpoint { get; init; } = string.Empty;
        public double FailureThreshold { get; init; } = 0.5;  // 50%
        public TimeSpan SamplingDuration { get; init; } = TimeSpan.FromSeconds(10);
        public int MinimumThroughput { get; init; } = 20;
        public TimeSpan DurationOfBreak { get; init; } = TimeSpan.FromSeconds(30);
    }
    
    // Example configuration in appsettings.json:
    // {
    //   "CircuitBreaker": {
    //     "PaymentService": {
    //       "ServiceName": "PaymentAPI",
    //       "Endpoint": "https://payment-api/process",
    //       "FailureThreshold": 0.6,  // 60%
    //       "SamplingDuration": "00:00:15",
    //       "MinimumThroughput": 30,
    //       "DurationOfBreak": "00:01:00"  // 1 minute
    //     }
    //   }
    // }
}

/// <summary>
/// EXAMPLE 3: CIRCUIT BREAKER WITH FALLBACK - Graceful Degradation
/// 
/// THE PROBLEM:
/// Circuit breaker throws BrokenCircuitException when open.
/// Users see errors instead of degraded functionality.
/// 
/// THE SOLUTION:
/// Wrap circuit breaker with fallback policy.
/// 
/// WHY IT MATTERS:
/// - Better user experience
/// - Service continues operating (degraded)
/// - Can return cached/static data
/// - Higher perceived availability
/// </summary>
/* NOTE: This example requires Polly V7 API with compatible extension methods.
 * Commented out due to PolicyBuilder<T> API compatibility issues.
 * For Polly V8+, use the new ResiliencePipeline API instead.
 
public static class CircuitBreakerWithFallbackExamples
{
    // \u2705 GOOD: Circuit breaker + fallback to cache
    public static async Task<string> CircuitBreakerWithCache(HttpClient client, ICache cache, ILogger logger)
    {
        var circuitBreakerPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    logger.LogWarning("Circuit OPENED - falling back to cache");
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit CLOSED - resuming API calls");
                });
        
        var fallbackPolicy = Policy<string>
            .Handle<BrokenCircuitException>()  // ✅ Catch when circuit is open
            .Or<HttpRequestException>()
            .FallbackAsync(
                fallbackAction: async (context, cancellationToken) =>
                {
                    logger.LogInformation("Using cached data");
                    return await cache.GetAsync("data") ?? "Default data";
                });
        
        // ✅ Wrap circuit breaker with fallback
        var policyWrap = fallbackPolicy.WrapAsync(circuitBreakerPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Flow:
        // 1. Circuit CLOSED → API call succeeds
        // 2. 3 failures → Circuit OPENS
        // 3. Next requests → BrokenCircuitException → Fallback to cache
        // 4. After 30s → Circuit HALF-OPEN → Test API
        // 5. If succeeds → Circuit CLOSES, resume API calls
    }
    
    // ✅ GOOD: Circuit breaker + fallback with degraded data
    public static async Task<Product[]> CircuitBreakerWithDegradedData(HttpClient client)
    {
        var circuitBreakerPolicy = Policy<Product[]>
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        
        var fallbackPolicy = Policy<Product[]>
            .Handle<BrokenCircuitException>()
            .Or<HttpRequestException>()
            .FallbackAsync(new[]
            {
                new Product { Id = 0, Name = "Service temporarily unavailable", IsAvailable = false }
            });
        
        var policyWrap = fallbackPolicy.WrapAsync(circuitBreakerPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/products");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product[]>() ?? Array.Empty<Product>();
        });
    }
}
*/

/// <summary>
/// EXAMPLE 4: MONITORING CIRCUIT BREAKER STATE
/// 
/// THE PROBLEM:
/// Need to know when circuits are open for monitoring/alerting.
/// 
/// THE SOLUTION:
/// Expose circuit breaker state via health checks, metrics.
/// 
/// WHY IT MATTERS:
/// - Alert ops team when circuit opens
/// - Dashboard showing service health
/// - Automatic recovery testing
/// - Capacity planning
/// </summary>
public static class CircuitBreakerMonitoring
{
    // ✅ GOOD: Circuit breaker with state tracking
    public class MonitorableCircuitBreaker
    {
        private readonly HttpClient _client;
        private readonly ILogger<MonitorableCircuitBreaker> _logger;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        
        public CircuitState State => _circuitBreakerPolicy.CircuitState;
        public bool IsOpen => State == CircuitState.Open;
        public bool IsHalfOpen => State == CircuitState.HalfOpen;
        public bool IsClosed => State == CircuitState.Closed;
        
        public MonitorableCircuitBreaker(HttpClient client, ILogger<MonitorableCircuitBreaker> logger)
        {
            _client = client;
            _logger = logger;
            
            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogWarning("Circuit OPENED");
                        // ✅ Could send alert here (email, Slack, PagerDuty)
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit CLOSED");
                        // ✅ Send recovery notification
                    });
        }
        
        public async Task<string> CallService()
        {
            var response = await _circuitBreakerPolicy.ExecuteAsync(() =>
                _client.GetAsync("https://api.example.com/data"));
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
        // ✅ Health check integration
        public bool IsHealthy() => State != CircuitState.Open;
    }
    
    // ✅ GOOD: ASP.NET Core health check
    public class CircuitBreakerHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        private readonly MonitorableCircuitBreaker _circuitBreaker;
        
        public CircuitBreakerHealthCheck(MonitorableCircuitBreaker circuitBreaker)
        {
            _circuitBreaker = circuitBreaker;
        }
        
        public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (_circuitBreaker.IsOpen)
            {
                return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                    "Circuit breaker is OPEN - service unavailable"));
            }
            
            if (_circuitBreaker.IsHalfOpen)
            {
                return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded(
                    "Circuit breaker is HALF-OPEN - testing recovery"));
            }
            
            return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                "Circuit breaker is CLOSED - service healthy"));
        }
    }
    
    // Configure in Startup:
    // services.AddHealthChecks()
    //     .AddCheck<CircuitBreakerHealthCheck>("circuit-breaker");
}

/// <summary>
/// EXAMPLE 5: CIRCUIT BREAKER + RETRY - Combined Resilience
/// 
/// THE PROBLEM:
/// Retry alone can overwhelm failing service.
/// Circuit breaker alone doesn't retry transient failures.
/// 
/// THE SOLUTION:
/// Combine retry (inner) with circuit breaker (outer).
/// 
/// WHY IT MATTERS:
/// - Retry handles transient failures
/// - Circuit breaker prevents sustained hammering
/// - Best of both patterns
/// - Production-grade resilience
/// 
/// ORDER MATTERS: Retry inside circuit breaker
/// </summary>
/* NOTE: This example requires Polly V7 API with compatible PolicyBuilder<T> extension methods.
 * Commented out due to API compatibility issues with FallbackAsync.
 * For Polly V8+, use the new ResiliencePipeline API instead.
 
public static class CircuitBreakerWithRetryExamples
{
    // \u2705 BEST PRACTICE: Circuit breaker wrapping retry
    public static async Task<string> CircuitBreakerAndRetry(HttpClient client, ILogger logger)
    {
        // ✅ Inner policy: Retry transient failures
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    logger.LogWarning("Retry {RetryCount} after {Delay}ms",
                        retryCount, timespan.TotalMilliseconds);
                });
        
        // ✅ Outer policy: Circuit breaker prevents sustained failures
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,  // 3 failed retry sequences
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    logger.LogWarning("Circuit OPENED after multiple retry failures");
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit CLOSED");
                });
        
        // ✅ Wrap retry with circuit breaker
        var policyWrap = circuitBreakerPolicy.WrapAsync(retryPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Flow:
        // Request 1: Fails → Retry (3x) → All fail → Circuit failure count = 1
        // Request 2: Fails → Retry (3x) → All fail → Circuit failure count = 2
        // Request 3: Fails → Retry (3x) → All fail → Circuit OPENS
        // Request 4+: BrokenCircuitException (no retries, immediate failure)
    }
    
    // ✅ GOOD: Full resilience stack
    public static async Task<string> FullResilienceStack(HttpClient client, ICache cache, ILogger logger)
    {
        // 1. Retry (innermost)
        var retryPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        
        // 2. Circuit breaker (middle)
        var circuitBreakerPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        
        // 3. Fallback (outermost)
        var fallbackPolicy = Policy<string>
            .Handle<BrokenCircuitException>()
            .Or<HttpRequestException>()
            .FallbackAsync(async () =>
            {
                logger.LogInformation("Falling back to cache");
                return await cache.GetAsync("data") ?? "Default data";
            });
        
        // ✅ Wrap: Fallback → CircuitBreaker → Retry
        var policyWrap = fallbackPolicy.WrapAsync(circuitBreakerPolicy.WrapAsync(retryPolicy));
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}
*/

/// <summary>
/// EXAMPLE 6: HTTPCLIENT FACTORY INTEGRATION
/// 
/// THE PROBLEM:
/// Need circuit breakers for specific HTTP clients.
/// 
/// THE SOLUTION:
/// Configure in IHttpClientFactory.
/// </summary>
public static class HttpClientFactoryIntegration
{
    // ✅ GOOD: Configure in Startup
    public static void ConfigureInStartup(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        services.AddHttpClient("PaymentAPI")
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddTransientHttpErrorPolicy(policy =>
                policy.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(1)));
        
        // Now any service requesting IHttpClientFactory.CreateClient("PaymentAPI")
        // gets retry + circuit breaker automatically
    }
}

// Supporting types
public interface ICache
{
    Task<string?> GetAsync(string key);
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}

// SUMMARY - Circuit Breaker Decision Guide:
//
// Use simple circuit breaker when:
// ✅ Low to medium traffic
// ✅ Need basic protection
// ✅ Consecutive failures are good indicator
//
// Use advanced circuit breaker when:
// ✅ High traffic (> 100 req/s)
// ✅ Need percentage-based thresholds
// ✅ Want sampling windows
//
// Combine with retry when:
// ✅ Transient failures are common
// ✅ Service has occasional blips
// ✅ Production scenarios
//
// Add fallback when:
// ✅ Can provide cached/default data
// ✅ Need graceful degradation
// ✅ User experience is priority
//
// CONFIGURATION GUIDELINES:
// - exceptionsAllowedBeforeBreaking: 3-5 typical
// - failureThreshold: 0.5 (50%) typical
// - minimumThroughput: 10-50 depending on traffic
// - durationOfBreak: 30s-60s typical
// - samplingDuration: 10-30s typical
