# Combining Policies

> Subject: [Resilience](../README.md)

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


