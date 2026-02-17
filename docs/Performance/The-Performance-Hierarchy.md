# The Performance Hierarchy

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
> Subject: [Performance](../README.md)

## The Performance Hierarchy

**Priority Order:**
1. **Algorithm** - O(n²) → O(n log n) matters most
2. **Data Structures** - Right collection (Dictionary vs List)
3. **Allocations** - Reduce GC pressure
4. **CPU Cache** - Memory access patterns
5. **Micro-optimizations** - Last resort, measure first

**Rule**: Optimize algorithms first, then allocations, then micro-optimizations.

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for The Performance Hierarchy before implementation work begins.
- Keep boundaries explicit so The Performance Hierarchy decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring The Performance Hierarchy in production-facing code.
- When performance, correctness, or maintainability depends on consistent The Performance Hierarchy decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying The Performance Hierarchy as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where The Performance Hierarchy is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for The Performance Hierarchy are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- The Performance Hierarchy is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem The Performance Hierarchy solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines The Performance Hierarchy but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose The Performance Hierarchy, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define The Performance Hierarchy and map it to one concrete implementation in this module.
- 3 minutes: compare The Performance Hierarchy with an alternative, then walk through one failure mode and mitigation.