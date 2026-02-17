# Deadlock Prevention

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Tasks/async-await basics and thread-safety fundamentals.
- Related examples: docs/Async-Multithreading/README.md
> Subject: [Async-Multithreading](../README.md)

## Deadlock Prevention

### The Classic ASP.NET Deadlock

```csharp
// ❌ DEADLOCK: Blocking on async code in synchronization context
public void DeadlockExample()
{
    var result = GetDataAsync().Result;  // ❌ DEADLOCK!
    // UI thread waits for task, task waits for UI thread
}

private async Task<string> GetDataAsync()
{
    await Task.Delay(100);  // Tries to resume on UI thread (blocked!)
    return "data";
}
```

### Solution 1: ConfigureAwait(false)

```csharp
// ✅ GOOD: Library code uses ConfigureAwait(false)
public async Task<string> GetDataAsync()
{
    // Don't capture synchronization context
    await Task.Delay(100).ConfigureAwait(false);
    return "data";
}

// Safe to block (but still not recommended)
var result = GetDataAsync().Result;  // ✅ Won't deadlock
```

### Solution 2: Async All The Way

```csharp
// ✅ BEST: Never block on async code
public async Task ProcessAsync()
{
    var result = await GetDataAsync();  // ✅ Properly awaited
}
```

### ConfigureAwait Guidelines

| Context          | Use ConfigureAwait(false)?            |
| ---------------- | ------------------------------------- |
| **Library code** | ✅ Yes (don't need sync context)      |
| **ASP.NET Core** | ⚠️ Optional (no sync context anyway)  |
| **WPF/WinForms** | ❌ No (need UI thread)                |
| **Console apps** | ⚠️ Optional (usually no sync context) |

---

## Detailed Guidance

Deadlock Prevention guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Deadlock Prevention before implementation work begins.
- Keep boundaries explicit so Deadlock Prevention decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Deadlock Prevention in production-facing code.
- When performance, correctness, or maintainability depends on consistent Deadlock Prevention decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Deadlock Prevention as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Deadlock Prevention is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Deadlock Prevention are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Deadlock Prevention is about concurrency and asynchronous flow control. It matters because it determines responsiveness and resource efficiency under load.
- Use it when handling I/O workloads safely in APIs and background jobs.

2-minute answer:
- Start with the problem Deadlock Prevention solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: parallelism gains vs coordination complexity.
- Close with one failure mode and mitigation: blocking async code paths and causing deadlocks or thread starvation.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Deadlock Prevention but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Deadlock Prevention, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Deadlock Prevention and map it to one concrete implementation in this module.
- 3 minutes: compare Deadlock Prevention with an alternative, then walk through one failure mode and mitigation.