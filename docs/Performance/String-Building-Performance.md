# String Building Performance

> Subject: [Performance](../README.md)

## String Building Performance

### Comparison

```csharp
// ❌ WORST: String concatenation (1000x slower)
public string BuildMessage_Bad(int count)
{
    string result = "";
    for (int i = 0; i < count; i++)
        result += $"Item {i},";  // ❌ New string each iteration!
    return result;
}

// ✅ GOOD: StringBuilder (100x faster)
public string BuildMessage_Better(int count)
{
    var sb = new StringBuilder(count * 10);  // ✅ Preallocate
    for (int i = 0; i < count; i++)
        sb.Append($"Item {i},");
    return sb.ToString();
}

// ✅ BEST: Span + stackalloc (zero allocations until final string)
public string BuildMessage_Best(int count)
{
    var chars = ArrayPool<char>.Shared.Rent(count * 10);
    try
    {
        Span<char> buffer = chars;
        int position = 0;
        
        for (int i = 0; i < count; i++)
        {
            // Write directly to buffer
            if (!i.TryFormat(buffer[position..], out int written))
                break;
            position += written;
            buffer[position++] = ',';
        }
        
        return new string(buffer[..position]);
    }
    finally
    {
        ArrayPool<char>.Shared.Return(chars);
    }
}
```

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for String Building Performance before implementation work begins.
- Keep boundaries explicit so String Building Performance decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring String Building Performance in production-facing code.
- When performance, correctness, or maintainability depends on consistent String Building Performance decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying String Building Performance as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where String Building Performance is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for String Building Performance are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

