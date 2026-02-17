# String Building Performance

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
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

## Interview Answer Block
30-second answer:
- String Building Performance is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem String Building Performance solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines String Building Performance but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose String Building Performance, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define String Building Performance and map it to one concrete implementation in this module.
- 3 minutes: compare String Building Performance with an alternative, then walk through one failure mode and mitigation.