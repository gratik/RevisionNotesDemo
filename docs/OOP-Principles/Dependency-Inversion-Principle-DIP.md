# Dependency Inversion Principle (DIP)

> Subject: [OOP-Principles](../README.md)

## Dependency Inversion Principle (DIP)

**"Depend upon abstractions, not concretions."**

### ❌ Violation

```csharp
// ❌ BAD: High-level class depends on low-level implementation
public class EmailNotification
{
    public void Send(string message)
    {
        // Email logic
    }
}

public class OrderService
{
    private EmailNotification _notification = new EmailNotification();  // ❌ Hard dependency
    
    public void ProcessOrder(Order order)
    {
        // Process...
        _notification.Send("Order processed");
    }
}
// Can't switch to SMS, can't test without sending real emails
```

### ✅ Solution

```csharp
// ✅ GOOD: Depend on abstraction
public interface INotificationService
{
    void Send(string message);
}

public class EmailNotification : INotificationService
{
    public void Send(string message) { /* Email logic */ }
}

public class SmsNotification : INotificationService
{
    public void Send(string message) { /* SMS logic */ }
}

public class OrderService
{
    private readonly INotificationService _notification;
    
    // ✅ Dependency injected
    public OrderService(INotificationService notification)
    {
        _notification = notification;
    }
    
    public void ProcessOrder(Order order)
    {
        // Process...
        _notification.Send("Order processed");
    }
}

// Can swap implementations, easy to test
```

---

## Detailed Guidance

Dependency Inversion Principle (DIP) should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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

