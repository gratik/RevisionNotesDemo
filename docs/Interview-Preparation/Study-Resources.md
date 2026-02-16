# Study Resources

> Subject: [Interview-Preparation](../README.md)

## Study Resources

### Official Documentation

- Microsoft Learn: https://learn.microsoft.com/training/
- .NET Documentation: https://learn.microsoft.com/dotnet/
- C# Programming Guide: https://learn.microsoft.com/dotnet/csharp/

### Practice Platforms

- LeetCode: Algorithm practice
- HackerRank: Coding challenges
- Codewars: Kata exercises
- Exercism: Mentored learning

### Books

- _C# in Depth_ by Jon Skeet
- _CLR via C#_ by Jeffrey Richter
- _Designing Data-Intensive Applications_ by Martin Kleppmann

---

## Detailed Guidance

Study Resources guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Study Resources before implementation work begins.
- Keep boundaries explicit so Study Resources decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Study Resources in production-facing code.
- When performance, correctness, or maintainability depends on consistent Study Resources decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Study Resources as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Study Resources is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Study Resources are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

