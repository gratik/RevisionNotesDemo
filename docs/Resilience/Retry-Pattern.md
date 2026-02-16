# Retry Pattern

> Subject: [Resilience](../README.md)

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


