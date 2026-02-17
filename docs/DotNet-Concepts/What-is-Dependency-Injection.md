# What is Dependency Injection?

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
> Subject: [DotNet-Concepts](../README.md)

## What is Dependency Injection?

**Dependency Injection** = Provide dependencies to a class rather than having the class create them

### Without DI (❌ Bad)

`csharp
// ❌ BAD: Hard dependency, can't test, can't swap
public class OrderService
{
    private EmailService _emailService = new EmailService();  // ❌ Tightly coupled
    
    public void ProcessOrder(Order order)
    {
        // Process order...
        _emailService.SendConfirmation(order);
    }
}
`

### With DI (✅ Good)

`csharp
// ✅ GOOD: Dependency injected, testable, flexible
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)  // ✅ Constructor injection
    {
        _emailService = emailService;
    }
    
    public void ProcessOrder(Order order)
    {
        // Process order...
        _emailService.SendConfirmation(order);
    }
}

// Can inject real or mock implementation
var service = new OrderService(new EmailService());  // Real
var service = new OrderService(new MockEmailService());  // Test
`

---

## Detailed Guidance

What is Dependency Injection? should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- What is Dependency Injection? is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem What is Dependency Injection? solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines What is Dependency Injection? but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose What is Dependency Injection?, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define What is Dependency Injection? and map it to one concrete implementation in this module.
- 3 minutes: compare What is Dependency Injection? with an alternative, then walk through one failure mode and mitigation.