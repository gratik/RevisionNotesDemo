# Common Pitfalls

> Subject: [Resilience](../README.md)

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


