# Thread-Safe Collections

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Tasks/async-await basics and thread-safety fundamentals.
- Related examples: docs/Async-Multithreading/README.md
> Subject: [Async-Multithreading](../README.md)

## Thread-Safe Collections

### Comparison Table

| Collection                          | Use Case                        | Performance                   |
| ----------------------------------- | ------------------------------- | ----------------------------- |
| **ConcurrentBag&lt;T&gt;**          | Unordered producer/consumer     | Fast adds, moderate reads     |
| **ConcurrentQueue&lt;T&gt;**        | FIFO ordered                    | Fast, minimal contention      |
| **ConcurrentStack&lt;T&gt;**        | LIFO ordered                    | Fast, minimal contention      |
| **ConcurrentDictionary&lt;K,V&gt;** | Key-value lookup                | Good, uses fine-grained locks |
| **BlockingCollection&lt;T&gt;**     | Producer-consumer with blocking | Slower, but convenient        |
| **Channel&lt;T&gt;**                | Async producer-consumer         | Best for async scenarios      |

### ConcurrentDictionary - Most Common

```csharp
var cache = new ConcurrentDictionary<int, User>();

// ✅ Thread-safe add
var user = cache.GetOrAdd(userId, id => _db.GetUser(id));

// ✅ Thread-safe update
cache.AddOrUpdate(
    userId,
    addValue: new User { Id = userId },
    updateValueFactory: (id, existing) =>
    {
        existing.LastUpdated = DateTime.UtcNow;
        return existing;
    });

// ✅ Thread-safe remove
cache.TryRemove(userId, out var removed);
```

### Channel&lt;T&gt; - Modern Async Producer-Consumer

```csharp
// ✅ Create bounded channel
var channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait
});

// Producer
await channel.Writer.WriteAsync(workItem);

// Consumer
await foreach (var item in channel.Reader.ReadAllAsync())
{
    await ProcessAsync(item);
}
```

---

## Detailed Guidance

Thread-Safe Collections guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Thread-Safe Collections before implementation work begins.
- Keep boundaries explicit so Thread-Safe Collections decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Thread-Safe Collections in production-facing code.
- When performance, correctness, or maintainability depends on consistent Thread-Safe Collections decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Thread-Safe Collections as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Thread-Safe Collections is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Thread-Safe Collections are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Thread-Safe Collections is about concurrency and asynchronous flow control. It matters because it determines responsiveness and resource efficiency under load.
- Use it when handling I/O workloads safely in APIs and background jobs.

2-minute answer:
- Start with the problem Thread-Safe Collections solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: parallelism gains vs coordination complexity.
- Close with one failure mode and mitigation: blocking async code paths and causing deadlocks or thread starvation.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Thread-Safe Collections but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Thread-Safe Collections, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Thread-Safe Collections and map it to one concrete implementation in this module.
- 3 minutes: compare Thread-Safe Collections with an alternative, then walk through one failure mode and mitigation.