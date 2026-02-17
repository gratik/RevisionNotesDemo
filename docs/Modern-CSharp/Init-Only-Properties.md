# Init-Only Properties

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# concepts, nullable awareness, and common refactoring patterns.
- Related examples: docs/Modern-CSharp/README.md
> Subject: [Modern-CSharp](../README.md)

## Init-Only Properties

```csharp
// ✅ Set during object initialization only
public class User
{
    public int Id { get; init; }  // ✅ Can only set during initialization
    public string Name { get; init; } = string.Empty;
}

// Usage
var user = new User
{
    Id = 1,
    Name = "Alice"
};

user.Id = 2;  // ❌ Compile error: init-only property
```

---

## Detailed Guidance

Init-Only Properties should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- Init-Only Properties is about newer C# language capabilities. It matters because modern syntax reduces boilerplate and improves intent clarity.
- Use it when updating legacy code to safer and more expressive patterns.

2-minute answer:
- Start with the problem Init-Only Properties solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: new language features vs team familiarity and consistency.
- Close with one failure mode and mitigation: mixing old and new idioms inconsistently across the same codebase.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Init-Only Properties but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Init-Only Properties, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Init-Only Properties and map it to one concrete implementation in this module.
- 3 minutes: compare Init-Only Properties with an alternative, then walk through one failure mode and mitigation.