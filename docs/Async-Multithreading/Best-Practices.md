# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Async-Multithreading](../README.md)

## Best Practices

### ✅ Async Best Practices

- Use `async`/`await` for I/O-bound work (network, disk, database)
- Use `Task.Run()` for CPU-bound work you want to offload
- Always pass `CancellationToken` through
- Never use `.Result` or `.Wait()` (blocks thread, risks deadlock)
- Avoid `async void` except for event handlers
- Use `ConfigureAwait(false)` in library code

### ✅ Thread Safety Best Practices

- Prefer immutable data structures (no sharing = no problems)
- Use concurrent collections for shared state
- Minimize shared state (use message passing instead)
- Use `async`/`await` instead of manual threading
- Never lock on `this`, `typeof(MyClass)`, or strings

### ✅ Performance Best Practices

- Use `ValueTask<T>` only when profiling shows benefit
- Avoid excessive allocations in hot async paths
- Use `Task.WhenAll` for parallel I/O operations
- Use `Parallel.ForEach` for CPU-bound parallel work
- Cache frequently-used Tasks when appropriate

---

## Detailed Guidance

Best Practices guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
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

