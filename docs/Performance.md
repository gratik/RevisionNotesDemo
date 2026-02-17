# Performance

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
This landing page summarizes the Performance documentation area and links into topic-level guides.

## Start Here

- [Subject README](Performance/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [ArrayPoolT---Object-Pooling](Performance/ArrayPoolT---Object-Pooling.md)
- [Benchmarking-with-BenchmarkDotNet](Performance/Benchmarking-with-BenchmarkDotNet.md)
- [Best-Practices](Performance/Best-Practices.md)
- [Common-Pitfalls](Performance/Common-Pitfalls.md)
- [Performance-Checklist](Performance/Performance-Checklist.md)
- [SpanT-and-MemoryT](Performance/SpanT-and-MemoryT.md)
- [String-Building-Performance](Performance/String-Building-Performance.md)
- [The-Performance-Hierarchy](Performance/The-Performance-Hierarchy.md)
- [ValueTaskT---Async-Performance](Performance/ValueTaskT---Async-Performance.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Performance before implementation work begins.
- Keep boundaries explicit so Performance decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Performance in production-facing code.
- When performance, correctness, or maintainability depends on consistent Performance decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Performance as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Performance is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Performance are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Performance is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem Performance solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Performance but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Performance, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Performance and map it to one concrete implementation in this module.
- 3 minutes: compare Performance with an alternative, then walk through one failure mode and mitigation.