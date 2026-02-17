# ArrayPool<T> - Object Pooling

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
> Subject: [Performance](../README.md)

## ArrayPool<T> - Object Pooling

### The Problem

```csharp
// ❌ BAD: Allocates array every time
public void ProcessData(int size)
{
    var buffer = new byte[size];  // ❌ Heap allocation + GC
    // Use buffer
}  // ❌ GC must collect this
```

### The Solution

```csharp
// ✅ GOOD: Rent from pool, return when done
public void ProcessData(int size)
{
    var buffer = ArrayPool<byte>.Shared.Rent(size);  // ✅ Reuses arrays
    try
    {
        Span<byte> span = buffer.AsSpan(0, size);  // ✅ Use exact size
        // Use buffer
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);  // ✅ Return to pool
    }
}
```

**ArrayPool Benefits:**
- 10-100x fewer allocations
- Reduced GC pressure (fewer collections)
- Reuses memory (better CPU cache)
- Thread-safe (concurrent access)

**When to Use:**
- Large temporary buffers (> 1KB)
- Hot paths with frequent allocations
- Buffer sizes vary (pool handles different sizes)

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for ArrayPool<T> - Object Pooling before implementation work begins.
- Keep boundaries explicit so ArrayPool<T> - Object Pooling decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring ArrayPool<T> - Object Pooling in production-facing code.
- When performance, correctness, or maintainability depends on consistent ArrayPool<T> - Object Pooling decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying ArrayPool<T> - Object Pooling as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where ArrayPool<T> - Object Pooling is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for ArrayPool<T> - Object Pooling are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- ArrayPool<T> - Object Pooling is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem ArrayPool<T> - Object Pooling solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines ArrayPool<T> - Object Pooling but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose ArrayPool<T> - Object Pooling, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define ArrayPool<T> - Object Pooling and map it to one concrete implementation in this module.
- 3 minutes: compare ArrayPool<T> - Object Pooling with an alternative, then walk through one failure mode and mitigation.