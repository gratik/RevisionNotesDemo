# Best Practices

> Subject: [Resilience](../README.md)

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


