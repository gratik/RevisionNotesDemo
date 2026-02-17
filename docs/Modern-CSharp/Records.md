# Records

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# concepts, nullable awareness, and common refactoring patterns.
- Related examples: docs/Modern-CSharp/README.md
> Subject: [Modern-CSharp](../README.md)

## Records

### Record Classes (Reference Types)

```csharp
// ✅ Concise immutable data class
public record Customer(string Name, string Email, int LoyaltyTier);

// Compiler generates:
// - Constructor
// - Properties (init-only)
// - Equals/GetHashCode (value-based)
// - ToString
// - Deconstruct
// - Copy (with expressions)

// Usage
var customer = new Customer("Alice", "alice@example.com", 3);
var modified = customer with { LoyaltyTier = 4 };  // ✅ Non-destructive mutation

Console.WriteLine(customer);  // Customer { Name = Alice, Email = alice@example.com, LoyaltyTier = 3 }

// ✅ Value-based equality
var c1 = new Customer("Bob", "bob@example.com", 1);
var c2 = new Customer("Bob", "bob@example.com", 1);
Console.WriteLine(c1 == c2);  // True (same values)
```

### Record Structs (Value Types)

```csharp
// ✅ Record struct (C# 10+)
public readonly record struct Point(int X, int Y);

var p1 = new Point(1, 2);
var p2 = p1 with { X = 5 };  // ✅ Non-destructive mutation
Console.WriteLine(p1 == p2);  // False
```

### When to Use Records

**✅ Use Records For**:
- DTOs (Data Transfer Objects)
- Value objects (Money, Address, Coordinate)
- Immutable data models
- API request/response models

**❌ Don't Use Records For**:
- Entities with identity (use classes)
- Mutable state (use classes)
- Performance-critical value types (use structs)

---

## Detailed Guidance

Records should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- Records is about newer C# language capabilities. It matters because modern syntax reduces boilerplate and improves intent clarity.
- Use it when updating legacy code to safer and more expressive patterns.

2-minute answer:
- Start with the problem Records solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: new language features vs team familiarity and consistency.
- Close with one failure mode and mitigation: mixing old and new idioms inconsistently across the same codebase.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Records but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Records, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Records and map it to one concrete implementation in this module.
- 3 minutes: compare Records with an alternative, then walk through one failure mode and mitigation.