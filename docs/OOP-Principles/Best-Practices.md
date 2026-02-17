# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Classes/interfaces, dependency inversion basics, and unit testing fundamentals.
- Related examples: docs/OOP-Principles/README.md
> Subject: [OOP-Principles](../README.md)

## Best Practices

### ✅ Applying SOLID
- **SRP**: Each class should do one thing well
- **OCP**: Design for extension (strategy pattern, inheritance)
- **LSP**: Ensure derived classes don't break base class behavior
- **ISP**: Keep interfaces small and focused
- **DIP**: Always inject dependencies via constructor

### ✅ When to Apply
- Use SOLID for **core domain logic** (services, repositories)
- Don't overengineer simple **DTOs or data classes**
- Apply principles when code becomes **hard to test or change**
- Refactor toward SOLID as complexity grows

### ✅ Balance Pragmatism
- SOLID is a guide, not a law
- Start simple, refactor when needed (YAGNI)
- Avoid over-abstraction
- Consider team size and project lifetime

---

## Detailed Guidance

Best Practices should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

### Design Notes
- Use language features to enforce intent at compile time (constraints, nullability, variance).
- Keep APIs narrow and intention-revealing; avoid generic over-engineering.
- Prefer composition and small interfaces over deep inheritance chains.
- Document where performance optimizations justify additional complexity.

### When To Use
- When building reusable libraries or framework-facing APIs.
- When replacing runtime casts/dynamic code with typed contracts.
- When teaching or reviewing core language design tradeoffs.

### Anti-Patterns To Avoid
- Public APIs with too many type parameters and unclear semantics.
- Constraints that do not correspond to required operations.
- Using reflection/dynamic where static typing is sufficient.

## Practical Example

- Start with a concrete implementation and extract generic behavior only when duplication appears.
- Add minimal constraints needed for compile-time guarantees.
- Validate with tests across reference and value type scenarios.

## Validation Checklist

- API signatures are understandable without deep internal context.
- Nullability and constraints match true invariants.
- Type misuse fails at compile time where possible.
- Benchmarks exist for any non-trivial performance optimizations.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Best Practices is about object-oriented design boundaries and responsibilities. It matters because good boundaries reduce coupling and improve testability.
- Use it when designing services and entities with clear responsibilities.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: extensibility vs added abstraction layers.
- Close with one failure mode and mitigation: applying principles mechanically without considering domain context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.