# Best Practices

> Subject: [Modern-CSharp](../README.md)

## Best Practices

### ✅ Records
- Use for immutable DTOs and value objects
- Prefer `record` over `class` for data transfer
- Use `with` expressions for non-destructive updates
- Don't use for entities with identity

### ✅ Pattern Matching
- Use `switch` expressions for multiple branches
- Combine with LINQ for powerful queries
- Use property patterns to eliminate null checks
- Prefer pattern matching over multiple if/else

### ✅ Nullable Reference Types
- Enable `<Nullable>enable</Nullable>` in all projects
- Use `?` to explicitly mark nullable references
- Initialize properties in constructors
- Use `required` for mandatory properties (C# 11+)

### ✅ Init-Only Properties
- Use for immutable objects
- Combine with records for clean syntax
- Prefer over constructor-only initialization

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

