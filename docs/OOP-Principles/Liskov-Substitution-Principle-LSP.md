# Liskov Substitution Principle (LSP)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Classes/interfaces, dependency inversion basics, and unit testing fundamentals.
- Related examples: docs/OOP-Principles/README.md
> Subject: [OOP-Principles](../README.md)

## Liskov Substitution Principle (LSP)

**"Objects of a superclass should be replaceable with objects of a subclass without breaking the application."**

### ❌ Violation

```csharp
// ❌ BAD: Square violates the Rectangle contract
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
    
    public override int Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
}

// Test
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
Console.WriteLine(rect.CalculateArea());  // Expected 50, got 100!
// Square is NOT a valid substitute for Rectangle
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate hierarchies or use composition
public interface IShape
{
    int CalculateArea();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    
    public int CalculateArea() => Side * Side;
}
// Both implement the same contract correctly
```

---

## Detailed Guidance

Liskov Substitution Principle (LSP) should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- Liskov Substitution Principle (LSP) is about object-oriented design boundaries and responsibilities. It matters because good boundaries reduce coupling and improve testability.
- Use it when designing services and entities with clear responsibilities.

2-minute answer:
- Start with the problem Liskov Substitution Principle (LSP) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: extensibility vs added abstraction layers.
- Close with one failure mode and mitigation: applying principles mechanically without considering domain context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Liskov Substitution Principle (LSP) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Liskov Substitution Principle (LSP), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Liskov Substitution Principle (LSP) and map it to one concrete implementation in this module.
- 3 minutes: compare Liskov Substitution Principle (LSP) with an alternative, then walk through one failure mode and mitigation.