# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# syntax, object-oriented fundamentals, and basic collection usage.
- Related examples: docs/Core-CSharp/README.md
> Subject: [Core-CSharp](../README.md)

## Best Practices

### ✅ Generics
- Use constraints to make APIs type-safe
- Prefer `List<T>` over `ArrayList` (always)
- Use `IEnumerable<T>` for read-only sequences
- Avoid over-constraining (only add constraints you need)

### ✅ Delegates and Events
- Use `EventHandler<TEventArgs>` for events
- Use `Func<T>` and `Action<T>` instead of custom delegates
- Always check for null before invoking events (`?.Invoke`)
- Unsubscribe from events to prevent memory leaks

### ✅ Extension Methods
- Put in static classes with clear names (`StringExtensions`)
- Don't overuse (prefer instance methods when you own the class)
- Use for "utility" methods on types you don't control
- Make them discoverable (good naming)

### ✅ Interfaces
- Prefer composition over inheritance
- Keep interfaces small (ISP)
- Use interfaces for dependency injection
- Name interfaces descriptively (`IRepository`, not `IRepo`)

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
- Best Practices is about core C# language features and API design. It matters because it directly affects correctness, readability, and maintainability.
- Use it when designing reusable domain and application abstractions.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: flexibility vs API complexity.
- Close with one failure mode and mitigation: over-abstracting simple code paths; keep public contracts intentional.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.