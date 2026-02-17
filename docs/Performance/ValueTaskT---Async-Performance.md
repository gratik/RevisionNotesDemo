# ValueTask<T> - Async Performance

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
> Subject: [Performance](../README.md)

## ValueTask<T> - Async Performance

### When to Use ValueTask

**Use `Task<T>`** (default):
- Result is always async
- Method awaited multiple times
- Task stored or passed around

**Use `ValueTask<T>`** (hot paths only):
- Result often available synchronously (cached)
- Await exactly once
- Not stored in fields

### Example

```csharp
// ✅ Task: Always async
public async Task<User> GetUserAsync(int id)
{
    return await _db.Users.FindAsync(id);  // Always hits DB
}

// ✅ ValueTask: Often sync (cached)
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately (no Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous, no allocation
    
    // Only allocates Task on cache miss
    var user = await _db.Users.FindAsync(id);
    _cache[id] = user;
    return user;
}
```

**ValueTask Rules:**
- ❌ Never await twice
- ❌ Never store in field
- ❌ Never access `.Result`
- ✅ Use when sync completion is common

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for ValueTask<T> - Async Performance before implementation work begins.
- Keep boundaries explicit so ValueTask<T> - Async Performance decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring ValueTask<T> - Async Performance in production-facing code.
- When performance, correctness, or maintainability depends on consistent ValueTask<T> - Async Performance decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying ValueTask<T> - Async Performance as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where ValueTask<T> - Async Performance is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for ValueTask<T> - Async Performance are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- ValueTask<T> - Async Performance is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem ValueTask<T> - Async Performance solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines ValueTask<T> - Async Performance but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose ValueTask<T> - Async Performance, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define ValueTask<T> - Async Performance and map it to one concrete implementation in this module.
- 3 minutes: compare ValueTask<T> - Async Performance with an alternative, then walk through one failure mode and mitigation.