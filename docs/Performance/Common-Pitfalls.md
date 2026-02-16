# Common Pitfalls

> Subject: [Performance](../README.md)

## Common Pitfalls

### ❌ Premature Optimization

```csharp
// ❌ BAD: Optimizing code that runs once
public void InitializeApp()
{
    // This runs once at startup - don't optimize
    LoadConfiguration();
    ConnectToDatabase();
}

// ✅ GOOD: Optimize hot paths
public void ProcessRequest()  // Called 10,000x/second
{
    // This is worth optimizing
}
```

### ❌ Span Lifetime Violations

```csharp
// ❌ BAD: Storing Span in field
public class BadExample
{
    private Span<int> _data;  // ❌ Compiler error: ref struct in class
}

// ❌ BAD: Returning Span from stackalloc
public Span<int> GetData()
{
    Span<int> buffer = stackalloc int[10];
    return buffer;  // ❌ Compiler error: escaping stack reference
}
```

### ❌ Large stackalloc

```csharp
// ❌ BAD: Stack overflow risk
Span<byte> huge = stackalloc byte[100_000];  // ❌ Too large for stack

// ✅ GOOD: Use threshold
const int MaxStackSize = 256;
Span<byte> buffer = size <= MaxStackSize
    ? stackalloc byte[size]
    : ArrayPool<byte>.Shared.Rent(size);
```

### ❌ Not Returning to ArrayPool

```csharp
// ❌ BAD: Rent but forget to return
var buffer = ArrayPool<byte>.Shared.Rent(1024);
// ... use buffer ...
// ❌ Forgot to Return() - pool depleted!

// ✅ GOOD: Always use try/finally
var buffer = ArrayPool<byte>.Shared.Rent(1024);
try
{
    // Use buffer
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);  // ✅ Always returned
}
```

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

