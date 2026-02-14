// ==============================================================================
// POLLY RETRY PATTERNS - Transient Fault Handling
// ==============================================================================
// PURPOSE:
//   Master retry patterns using Polly for resilient applications.
//   Handle transient failures in network calls, database operations, external services.
//
// WHY POLLY:
//   - Industry-standard resilience library
//   - Declarative policy-based approach
//   - Async-first design
//   - Integration with HttpClient, IHttpClientFactory
//   - Powerful policy composition
//
// WHAT YOU'LL LEARN:
//   1. Simple retry (immediate)
//   2. Wait and retry (fixed delays)
//   3. Exponential backoff
//   4. Jitter (randomization)
//   5. Retry with specific exceptions
//   6. Retry with fallback
//
// INSTALL: Install-Package Polly
//          Install-Package Microsoft.Extensions.Http.Polly
//
// WHEN TO USE RETRY:
//   ✅ HTTP 5xx errors, timeouts
//   ✅ Database deadlocks, connection failures
//   ✅ Rate limit errors (HTTP 429)
//   ❌ HTTP 4xx client errors (except 429)
//   ❌ Authentication failures
//   ❌ Validation errors
// ==============================================================================

using Polly;
using Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;

namespace RevisionNotesDemo.Resilience;

/// <summary>
/// EXAMPLE 1: SIMPLE RETRY - Immediate Retry Without Delay
/// 
/// THE PROBLEM:
/// Transient network failures cause immediate operation failure.
/// A single retry often succeeds.
/// 
/// THE SOLUTION:
/// Retry N times immediately before giving up.
/// 
/// WHY IT MATTERS:
/// - 90% of transient failures resolve on first retry
/// - Simplest retry pattern
/// - Low latency (no waiting)
/// 
/// WHEN TO USE:
/// - Very fast operations (< 100ms)
/// - In-memory services
/// - Local network calls
/// 
/// CAUTION: Can overwhelm failing service - use wait-and-retry for most cases
/// </summary>
public static class SimpleRetryExamples
{
    // ❌ BAD: No retry - fails on first transient error
    public static async Task<string> NoRetry(HttpClient client)
    {
        var response = await client.GetAsync("https://api.example.com/data");
        return await response.Content.ReadAsStringAsync();
        // ❌ Network  blip = operation fails
    }
    
    // ✅ GOOD: Simple retry policy
    public static async Task<string> WithSimpleRetry(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()  // Retry on HTTP exceptions
            .RetryAsync(3);  // Retry up to 3 times
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Total attempts: 1 initial + 3 retries = 4 attempts
    }
    
    // ✅ GOOD: Retry with logging
    public static async Task<string> WithRetryAndLogging(HttpClient client, Microsoft.Extensions.Logging.ILogger logger)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .RetryAsync(3, onRetry: (exception, retryCount) =>
            {
                // ✅ Log each retry attempt
                logger.LogWarning(exception,
                    "Retry {RetryCount} due to: {Message}",
                    retryCount, exception.Message);
            });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
    
    // ✅ GOOD: Retry specific HTTP status codes
    public static async Task<HttpResponseMessage> RetryOnServerErrors(HttpClient client)
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                r.StatusCode >= HttpStatusCode.InternalServerError ||  // 5xx errors
                r.StatusCode == HttpStatusCode.RequestTimeout)          // 408 timeout
            .RetryAsync(3);
        
        return await retryPolicy.ExecuteAsync(() =>
            client.GetAsync("https://api.example.com/data"));
    }
}

/// <summary>
/// EXAMPLE 2: WAIT AND RETRY - Fixed Delays Between Attempts
/// 
/// THE PROBLEM:
/// Immediate retries can overwhelm a recovering service.
/// Need time for service to recover.
/// 
/// THE SOLUTION:
/// Wait fixed duration between retries.
/// 
/// WHY IT MATTERS:
/// - Give service time to recover
/// - Prevent thundering herd
/// - More polite to external services
/// 
/// PATTERN: Wait 1s, 1s, 1s between retries
/// </summary>
public static class WaitAndRetryExamples
{
    // ✅ GOOD: Fixed delay wait-and-retry
    public static async Task<string> FixedDelayRetry(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(2));  // Wait 2s between each retry
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Timeline:
        // 0s:   Attempt 1 (fails)
        // 2s:   Attempt 2 (fails)
        // 4s:   Attempt 3 (fails)
        // 6s:   Attempt 4 (succeeds or final failure)
    }
    
    // ✅ GOOD: Progressive delays
    public static async Task<string> ProgressiveDelayRetry(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromMilliseconds(500),  // 1st retry: wait 500ms
                TimeSpan.FromSeconds(1),          // 2nd retry: wait 1s
                TimeSpan.FromSeconds(2),          // 3rd retry: wait 2s
                TimeSpan.FromSeconds(5)           // 4th retry: wait 5s
            });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
    
    // ✅ GOOD: With detailed logging
    public static async Task<string> WaitAndRetryWithContext(HttpClient client, Microsoft.Extensions.Logging.ILogger logger)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),  // 2s, 4s, 8s
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(exception,
                        "Retry {RetryCount} after {Delay}ms. Operation: {Operation}",
                        retryCount, timeSpan.TotalMilliseconds, context["Operation"]);
                });
        
        return await retryPolicy.ExecuteAsync(
            context => client.GetStringAsync("https://api.example.com/data"),
            new Context { ["Operation"] = "FetchData" });  // Pass context for logging
    }
}

/// <summary>
/// EXAMPLE 3: EXPONENTIAL BACKOFF - Increasing Delays
/// 
/// THE PROBLEM:
/// Fixed delays don't adapt - service might need more time to recover.
/// 
/// THE SOLUTION:
/// Exponentially increase wait time: 1s, 2s, 4s, 8s, 16s...
/// 
/// WHY IT MATTERS:
/// - Adapt to recovery time
/// - Reduce load on struggling service
/// - Industry best practice
/// - Most cloud services recommend this
/// 
/// FORMULA: delay = baseDelay * 2^retryAttempt
/// </summary>
public static class ExponentialBackoffExamples
{
    // ✅ GOOD: Exponential backoff
    public static async Task<string> ExponentialBackoff(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));  // 2^1=2s, 2^2=4s, 2^3=8s, 2^4=16s, 2^5=32s
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Timeline:
        // 0s:   Attempt 1 (fails)
        // 2s:   Attempt 2 (fails)
        // 6s:   Attempt 3 (fails) - waited 4s
        // 14s:  Attempt 4 (fails) - waited 8s
        // 30s:  Attempt 5 (fails) - waited 16s
        // 62s:  Attempt 6 (final) - waited 32s
    }
    
    // ✅ BETTER: Exponential backoff with max delay
    public static async Task<string> ExponentialBackoffWithCap(HttpClient client)
    {
        var maxDelay = TimeSpan.FromSeconds(30);
        
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: retryAttempt =>
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    return delay > maxDelay ? maxDelay : delay;  // ✅ Cap at 30s
                });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
    
    // ✅ BEST: Exponential backoff with jitter (see next example)
}

/// <summary>
/// EXAMPLE 4: JITTER - Randomization to Prevent Thundering Herd
/// 
/// THE PROBLEM:
/// Many clients retry at same time → synchronized retries → thundering herd.
/// Service recovers, then immediately hit by wave of retries.
/// 
/// THE SOLUTION:
/// Add randomness (jitter) to retry delays.
/// 
/// WHY IT MATTERS:
/// - Spread retry load over time
/// - Prevent synchronized retries
/// - Recommended by AWS, Azure, Google Cloud
/// - Critical for high-scale systems
/// 
/// STRATEGIES:
/// - Full jitter: random(0, exponentialDelay)
/// - Decorrelated jitter: more sophisticated
/// </summary>
public static class JitterExamples
{
    private static readonly Random Jitterer = new();
    
    // ✅ GOOD: Exponential backoff with full jitter
    public static async Task<string> ExponentialBackoffWithJitter(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                {
                    var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    // ✅ Random delay between 0 and exponentialDelay
                    var jitterDelay = TimeSpan.FromMilliseconds(Jitterer.Next(0, (int)exponentialDelay.TotalMilliseconds));
                    return jitterDelay;
                });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Instead of all clients retrying at 2s, 4s, 8s...
        // Clients retry at scattered times: 1.2s, 1.8s, 3.1s, 3.7s, 6.2s, 7.4s...
    }
    
    // ✅ BETTER: Using Polly's built-in jitter (Polly v8+)
    public static async Task<string> PollyBuiltInJitter(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Add jitter using Polly extension
                });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
    
    // ✅ BEST: Decorrelated jitter (AWS recommendation)
    public static async Task<string> DecorrelatedJitter(HttpClient client)
    {
        var random = new Random();
        var baseDelay = TimeSpan.FromSeconds(1);
        
        TimeSpan previousDelay = baseDelay;
        
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt =>
                {
                    // ✅ Decorrelated jitter formula (AWS)
                    var temp = Math.Min(previousDelay.TotalMilliseconds * 3, baseDelay.TotalMilliseconds * Math.Pow(2, retryAttempt));
                    var jitter = random.Next((int)(baseDelay.TotalMilliseconds), (int)temp);
                    previousDelay = TimeSpan.FromMilliseconds(jitter);
                    return previousDelay;
                });
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
    }
}

/// <summary>
/// EXAMPLE 5: RETRY WITH SPECIFIC EXCEPTIONS - Fine-Grained Control
/// 
/// THE PROBLEM:
/// Not all exceptions should be retried.
/// 401/403 errors won't resolve with retry.
/// 
/// THE SOLUTION:
/// Only retry specific transient exceptions.
/// 
/// WHY IT MATTERS:
/// - Don't waste time retrying permanent failures
/// - Different handling for different errors
/// - Faster failure for non-transient issues
/// </summary>
public static class SelectiveRetryExamples
{
    // ✅ GOOD: Retry only transient HTTP errors
    public static async Task<string> RetryTransientHttpErrors(HttpClient client)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode == HttpStatusCode.RequestTimeout ||        // 408
                r.StatusCode == HttpStatusCode.TooManyRequests ||       // 429
                r.StatusCode >= HttpStatusCode.InternalServerError)     // 5xx
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        
        var response = await retryPolicy.ExecuteAsync(() =>
            client.GetAsync("https://api.example.com/data"));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    // ✅ GOOD: Retry specific exceptions only
    public static async Task<string> RetrySpecificExceptions()
    {
        var retryPolicy = Policy
            .Handle<TimeoutException>()              // Retry timeouts
            .Or<HttpRequestException>(ex =>
                ex.StatusCode == HttpStatusCode.ServiceUnavailable)  // Retry 503 only
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
        
        return await retryPolicy.ExecuteAsync(async () =>
        {
            // Your operation
            await Task.Delay(100);
            return "Success";
        });
    }
    
    // ✅ GOOD: Don't retry authorization failures
    public static async Task<string> AvoidRetryingAuthFailures(HttpClient client)
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                // ✅ Only retry server errors, NOT 401/403
                r.StatusCode >= HttpStatusCode.InternalServerError &&
                r.StatusCode != HttpStatusCode.Unauthorized &&
                r.StatusCode != HttpStatusCode.Forbidden)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
        
        var response = await retryPolicy.ExecuteAsync(() =>
            client.GetAsync("https://api.example.com/data"));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

/// <summary>
/// EXAMPLE 6: RETRY WITH FALLBACK - Graceful Degradation
/// 
/// THE PROBLEM:
/// After all retries fail, operation throws exception.
/// User sees error instead of degraded experience.
/// 
/// THE SOLUTION:
/// Combine retry with fallback policy.
/// 
/// WHY IT MATTERS:
/// - Better user experience (cached/default data vs error)
/// - Graceful degradation
/// - Higher perceived availability
/// </summary>
public static class RetryWithFallbackExamples
{
    // ✅ GOOD: Retry then fallback to cache
    public static async Task<string> RetryThenFallbackToCache(HttpClient client, ICache cache)
    {
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(1));
        
        var fallbackPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .FallbackAsync(
                fallbackValue: await cache.GetAsync("data") ?? "Default data",
                onFallbackAsync: async (result, context) =>
                {
                    // ✅ Log fallback usage
                    Console.WriteLine("Falling back to cached data");
                    await Task.CompletedTask;
                });
        
        // ✅ Wrap retry with fallback
        var policyWrap = fallbackPolicy.WrapAsync(retryPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        });
        
        // Flow:
        // 1. Try  API call
        // 2. Fail → Retry (wait 1s)
        // 3. Fail → Retry (wait 1s)
        // 4. Fail → Fallback to cache
    }
    
    // ✅ GOOD: Retry with fallback to default
    public static async Task<Product?> RetryThenDefault(HttpClient client)
    {
        var retryPolicy = Policy<Product?>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
        
        var fallbackPolicy = Policy<Product?>
            .Handle<HttpRequestException>()
            .FallbackAsync(
                fallbackValue: new Product { Id = 0, Name = "Unavailable" },
                onFallbackAsync: (result, context) =>
                {
                    Console.WriteLine("API unavailable, returning default product");
                    return Task.CompletedTask;
                });
        
        var policyWrap = fallbackPolicy.WrapAsync(retryPolicy);
        
        return await policyWrap.ExecuteAsync(async () =>
        {
            var response = await client.GetAsync("https://api.example.com/products/123");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>();
        });
    }
}

/// <summary>
/// EXAMPLE 7: HTTPCLIENT FACTORY INTEGRATION - ASP.NET Core
/// 
/// THE PROBLEM:
/// Need retry policies for specific HTTP clients.
/// 
/// THE SOLUTION:
/// Configure policies in IHttpClientFactory.
/// 
/// WHY IT MATTERS:
/// - Centralized retry configuration
/// - Per-service policies
/// - Integrates with DI
/// </summary>
public static class HttpClientFactoryExamples
{
    // ✅ GOOD: Configure retry policy in Startup
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddHttpClient("ExternalApi", client =>
        {
            client.BaseAddress = new Uri("https://api.example.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddTransientHttpErrorPolicy(policy =>  // ✅ Retry 5xx and 408
            policy.WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
        .AddPolicyHandler(Policy<HttpResponseMessage>  // ✅ Also retry 429 (rate limit)
            .HandleResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5)));
    }
    
    // Usage in service:
    public class ApiService
    {
        private readonly HttpClient _client;
        
        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ExternalApi");  // ✅ Gets retry policies
        }
        
        public async Task<string> GetData()
        {
            // ✅ Automatic retry on transient failures
            var response = await _client.GetAsync("/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

// SUMMARY - Retry Pattern Decision Tree:
//
// Fast operation (< 100ms)?
//   → Simple retry (no delay)
//
// Slow operation or external service?
//   → Wait-and-retry
//
// Service under heavy load?
//   → Exponential backoff
//
// High-scale system (many clients)?
//   → Exponential backoff + jitter
//
// Need graceful degradation?
//   → Retry + fallback
//
// BEST PRACTICES:
// ✅ Use exponential backoff + jitter for production
// ✅ Cap maximum wait time (e.g., 30s)
// ✅ Limit total retry attempts (3-5 typical)
// ✅ Only retry transient failures
// ✅ Log retry attempts
// ✅ Consider fallback for better UX
// ❌ Don't retry authentication/authorization failures
// ❌ Don't retry validation errors (4xx except 408, 429)
