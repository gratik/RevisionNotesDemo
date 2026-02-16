# What is Dependency Injection?

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

