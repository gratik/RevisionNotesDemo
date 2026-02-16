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

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

### Design Notes
- Define success criteria for Cancellation and Timeouts before implementation work begins.
- Keep boundaries explicit so Cancellation and Timeouts decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Cancellation and Timeouts in production-facing code.
- When performance, correctness, or maintainability depends on consistent Cancellation and Timeouts decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Cancellation and Timeouts as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Cancellation and Timeouts is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Cancellation and Timeouts are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

