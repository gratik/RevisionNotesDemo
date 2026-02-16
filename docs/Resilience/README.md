# Resilience Patterns (Polly, Circuit Breaker)

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: HTTP failure handling basics
- Related examples: Learning/Resilience/PollyRetryPatterns.cs, Learning/Resilience/CircuitBreakerPattern.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Web API and MVC, Async Multithreading
- **When to Study**: Before production release for external dependency handling.
- **Related Files**: `../Learning/Resilience/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Design-Patterns.md)
- **Next Step**: [Performance.md](../Performance.md)
<!-- STUDY-NAV-END -->


## Overview

Resilience patterns handle transient failures gracefully using Polly, a .NET resilience library.
Patterns include retry (try again), circuit breaker (fail fast), timeout (bound execution), and
bulkhead (limit concurrency). Building resilient systems means failing gracefully, not catastrophically.

---

## Why Resilience Matters

**The Reality**: Failures are inevitable
- Network timeouts
- Database deadlocks
- Service overload
- Temporary outages

**The Solution**: Handle failures predictably
- Retry transient errors
- Prevent cascading failures
- Fail fast when appropriate
- Give services time to recover

---

## Polly Patterns Overview

| Pattern | Purpose | When to Use |
|---------|---------|-------------|
| **Retry** | Try again after failure | Transient errors (network blips) |
| **Circuit Breaker** | Stop calling failing service | Persistent failures (service down) |
| **Timeout** | Bound operation duration | Prevent hanging |
| **Bulkhead** | Limit concurrent operations | Prevent resource exhaustion |
| **Fallback** | Return default on failure | Provide degraded experience |

---

## Retry Pattern

### Basic Retry

```csharp
// ✅ Retry 3 times with fixed delay
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(2));

var result = await retryPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync("https://api.example.com/data");
});
```

### Exponential Backoff

```csharp
// ✅ Retry with exponential backoff: 1s, 2s, 4s, 8s
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 4,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
```

### Exponential Backoff with Jitter

```csharp
// ✅ Add randomness to prevent thundering herd
var jitterer = new Random();
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 4,
        sleepDurationProvider: attempt =>
        {
            var exponential = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            var jitter = TimeSpan.FromMilliseconds(jitterer.Next(0, 1000));
            return exponential + jitter;
        });
```

---

## Circuit Breaker Pattern

### The Three States

**CLOSED** (Normal): Requests flow through, failures counted
**OPEN** (Tripped): Requests fail immediately, no calls made  
**HALF-OPEN** (Testing): Allow one request to test recovery

### Basic Circuit Breaker

```csharp
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 3,  // Break after 3 failures
        durationOfBreak: TimeSpan.FromSeconds(30));  // Stay open 30s

// Flow:
// Attempt 1: Fails → Count = 1 (CLOSED)
// Attempt 2: Fails → Count = 2 (CLOSED)
// Attempt 3: Fails → Circuit OPENS
// Attempt 4+: BrokenCircuitException (immediate)
// After 30s: Circuit moves to HALF-OPEN
// Next: Success → CLOSED | Failure → OPEN again
```

### Advanced Circuit Breaker

```csharp
// ✅ Based on failure rate, not consecutive failures
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .AdvancedCircuitBreakerAsync(
        failureThreshold: 0.5,  // 50% failure rate
        samplingDuration: TimeSpan.FromSeconds(10),  // Over 10s window
        minimumThroughput: 10,  // Minrequests before evaluating
        durationOfBreak: TimeSpan.FromSeconds(30));
```

---

## Timeout Pattern

```csharp
// ✅ Operation must complete within 5 seconds
var timeoutPolicy = Policy
    .TimeoutAsync(TimeSpan.FromSeconds(5));

var result = await timeoutPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync("https://slow-api.com/data");
});
```

---

## Bulkhead Pattern

```csharp
// ✅ Limit concurrent operations to 10
var bulkheadPolicy = Policy
    .BulkheadAsync(
        maxParallelization: 10,  // Max 10 concurrent
        maxQueuingActions: 5);   // Max 5 queued

// Prevents resource exhaustion
// Request 11-15: Queued
// Request 16+: BulkheadRejectedException
```

---

## Combining Policies

### Wrap Policies Together

```csharp
// ✅ Retry + Circuit Breaker + Timeout
var timeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(5));
var retry = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt));
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// Combine: innermost policy executes first
var combined = Policy.WrapAsync(retry, circuitBreaker, timeout);

var result = await combined.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync(url);
});
```

### PolicyWrap Order Matters

```
Order: Retry → Circuit Breaker → Timeout

1. Timeout: Ensures operation completes within time
2. Circuit Breaker: Stops calling if service is down
3. Retry: Retries transient failures
```

---

## Best Practices

### ✅ Choose Right Pattern
- **Retry**: Transient errors (network blips, temporary DB locks)
- **Circuit Breaker**: Persistent failures (service down)
- **Timeout**: Prevent hanging (slow services)
- **Bulkhead**: Isolate failures (one slow service doesn't block others)

### ✅ Exponential Backoff with Jitter
- Prevents thundering herd (all clients retry simultaneously)
- Adds randomness to spread load

### ✅ Log Policy Events
```csharp
var retry = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        3,
        attempt => TimeSpan.FromSeconds(attempt),
        onRetry: (exception, timespan, attempt, context) =>
        {
            _logger.LogWarning(exception,
                "Retry {Attempt} after {Delay}s", attempt, timespan.TotalSeconds);
        });
```

### ✅ Use Polly in ASP.NET Core
```csharp
// Register typed HTTP client with Polly
services.AddHttpClient<IOrderService, OrderService>()
    .AddTransientHttpErrorPolicy(p =>
        p.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt)))
    .AddTransientHttpErrorPolicy(p =>
        p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
```

---

## Common Pitfalls

### ❌ Retrying Non-Transient Errors

```csharp
// ❌ BAD: Retry 404 Not Found
var policy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .RetryAsync(3);

// ✅ GOOD: Only retry transient errors
var policy = Policy
    .HandleResult<HttpResponseMessage>(r =>
        r.StatusCode == HttpStatusCode.RequestTimeout ||
        r.StatusCode == HttpStatusCode.ServiceUnavailable)
    .RetryAsync(3);
```

### ❌ No Exponential Backoff

```csharp
// ❌ BAD: Fixed delay hammers failing service
var policy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(10, _ => TimeSpan.FromSeconds(1));

// ✅ GOOD: Exponential backoff gives service time
var policy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
```

---

## Related Files

- [Resilience/PollyRetryPatterns.cs](../../Learning/Resilience/PollyRetryPatterns.cs)
- [Resilience/CircuitBreakerPattern.cs](../../Learning/Resilience/CircuitBreakerPattern.cs)
- [Resilience/TimeoutAndBulkhead.cs](../../Learning/Resilience/TimeoutAndBulkhead.cs)

## See Also

- [Logging and Observability](../Logging-Observability.md)
- [Performance](../Performance.md)
- [Web API and MVC](../Web-API-MVC.md)
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Performance.md](../Performance.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: Resilience means controlled failure behavior under dependency issues, not just retries.
- 2-minute deep dive: I combine timeout budgets, jittered retries, circuit breakers, and fallback behavior tied to SLOs and error budgets.
- Common follow-up: How do you prevent retry storms?
- Strong response: Cap attempts, add jitter, short-circuit via breaker state, and enforce per-call timeout budgets.
- Tradeoff callout: Aggressive retries can amplify outages if limits are not enforced.

## Interview Bad vs Strong Answer

- Bad answer: "I know Resilience and I would just follow best practices."
- Strong answer: "For Resilience, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Resilience in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Why Resilience Matters](Why-Resilience-Matters.md)
- [Polly Patterns Overview](Polly-Patterns-Overview.md)
- [Retry Pattern](Retry-Pattern.md)
- [Circuit Breaker Pattern](Circuit-Breaker-Pattern.md)
- [Timeout Pattern](Timeout-Pattern.md)
- [Bulkhead Pattern](Bulkhead-Pattern.md)
- [Combining Policies](Combining-Policies.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)



