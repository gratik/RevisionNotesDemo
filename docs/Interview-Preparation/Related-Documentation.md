# Related Documentation

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Interview-Preparation](../README.md)

## Related Documentation

- [Core C#](../Core-CSharp.md) - Fundamentals review
- [Modern C#](../Modern-CSharp.md) - Latest C# features
- [Design Patterns](../Design-Patterns.md) - Common interview patterns
- [SOLID Principles](../OOP-Principles.md) - OOP interview topics
- [Data Access](../Data-Access.md) - EF Core and SQL questions
- [Async Multithreading](../Async-Multithreading.md) - Async/await deep dive
- [Performance](../Performance.md) - Optimization questions
- [Security](../Security.md) - Security best practices
- [Testing](../Testing.md) - Testing strategies

---

## Detailed Guidance

Related Documentation guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Related Documentation before implementation work begins.
- Keep boundaries explicit so Related Documentation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Related Documentation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Related Documentation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Related Documentation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Related Documentation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Related Documentation are documented and reviewable.
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

