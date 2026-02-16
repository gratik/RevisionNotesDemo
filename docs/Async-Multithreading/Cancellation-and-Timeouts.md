# Cancellation and Timeouts

> Subject: [Async-Multithreading](../README.md)

## Cancellation and Timeouts

### CancellationToken Pattern

```csharp
// ✅ Support cancellation in async methods
public async Task<List<Order>> GetOrdersAsync(CancellationToken ct)
{
    // Pass token to async operations
    var orders = await _db.Orders
        .Where(o => o.Status == "Active")
        .ToListAsync(ct);  // ✅ Operation can be cancelled

    // Check for cancellation in loops
    foreach (var order in orders)
    {
        ct.ThrowIfCancellationRequested();  // ✅ Early exit if cancelled
        await ProcessOrderAsync(order, ct);
    }

    return orders;
}

// Usage with timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
try
{
    var orders = await GetOrdersAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Handle timeout or cancellation
}
```

### Cancellation Best Practices

```csharp
// ✅ GOOD: Always accept and pass CancellationToken
public async Task ProcessAsync(CancellationToken ct = default)
{
    await _httpClient.GetAsync("url", ct);  // Pass it through
}

// ❌ BAD: Ignoring CancellationToken
public async Task ProcessAsync(CancellationToken ct)
{
    await _httpClient.GetAsync("url");  // ❌ Can't be cancelled
}

// ✅ Combine multiple cancellation sources
var cts1 = new CancellationTokenSource();
var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var linked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
```

---


