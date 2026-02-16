# Timeout Pattern

> Subject: [Resilience](../README.md)

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


