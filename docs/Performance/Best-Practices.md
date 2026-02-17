# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
> Subject: [Performance](../README.md)

## Best Practices

### ✅ Measure First
- Use BenchmarkDotNet for micro-benchmarks
- Use profilers (dotTrace, PerfView) for real apps
- Identify hot paths (where CPU time is spent)
- Don't optimize code that runs once

### ✅ Allocation Reduction
- Use `Span<T>` for slicing without copying
- Use `stackalloc` for small temporary buffers (<1KB)
- Use `ArrayPool<T>` for large temporary buffers
- Reuse objects instead of recreating
- Avoid boxing (object x = 42)

### ✅ Async Performance
- Use `ValueTask<T>` when sync completion is common
- Use `ConfigureAwait(false)` in library code
- Cache tasks that return same result
- Avoid `async void` except event handlers

### ✅ Collection Performance
- Preallocate collections (capacity parameter)
- Use `Dictionary<K,V>` for lookups (not List)
- Use `HashSet<T>` for uniqueness checks
- Clear and reuse collections in loops

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

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
- Best Practices is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.