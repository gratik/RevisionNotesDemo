# Task, Thread, and ValueTask

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Async-Multithreading](../README.md)

## Task, Thread, and ValueTask

### Comparison Table

| Feature       | Task             | Thread            | ValueTask                      |
| ------------- | ---------------- | ----------------- | ------------------------------ |
| **Purpose**   | Async operations | Direct threading  | High-perf async                |
| **Overhead**  | Moderate         | High (1MB stack)  | Minimal (struct)               |
| **Awaitable** | Yes              | No                | Yes                            |
| **Result**    | Task&lt;T&gt;    | N/A               | ValueTask&lt;T&gt;             |
| **Use Case**  | General async    | Rare, legacy code | Hot paths with sync completion |
| **Can Reuse** | No               | No                | No (await once!)               |

### When to Use Each

```csharp
// ✅ Task: Default choice for async operations
public async Task<User> GetUserAsync(int id)
{
    var user = await _db.Users.FindAsync(id);
    return user;
}

// ✅ ValueTask: When result is often synchronously available
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately from cache (avoiding Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous completion

    // Only allocates Task if cache misses
    return await _db.Users.FindAsync(id);
}

// ❌ Thread: Avoid unless you need very specific control
public void LegacyThreadExample()
{
    var thread = new Thread(() =>
    {
        // Heavy computation
    });
    thread.Start();
    thread.Join();  // ❌ Prefer Task.Run instead
}
```

**ValueTask Rules:**

- ✅ Use for hot paths where sync completion is common (e.g., caching)
- ❌ Never await more than once
- ❌ Don't store in fields
- ❌ Never use `.Result` or `.GetAwaiter().GetResult()`

---

## Detailed Guidance

Task, Thread, and ValueTask guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Task, Thread, and ValueTask before implementation work begins.
- Keep boundaries explicit so Task, Thread, and ValueTask decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Task, Thread, and ValueTask in production-facing code.
- When performance, correctness, or maintainability depends on consistent Task, Thread, and ValueTask decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Task, Thread, and ValueTask as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Task, Thread, and ValueTask is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Task, Thread, and ValueTask are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

