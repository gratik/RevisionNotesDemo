# Common Pitfalls

> Subject: [OOP-Principles](../README.md)

## Common Pitfalls

### ❌ Over-Abstraction

```csharp
// ❌ BAD: Too many layers
public interface IUserFactory { }
public interface IBaseUserFactory : IUserFactory { }
public interface IAdvancedUserFactory : IBaseUserFactory { }
// ... 5 more interfaces

// ✅ GOOD: Simple abstraction
public interface IUserFactory
{
    User Create(string name, string email);
}
```

### ❌ Violating SRP with "God Objects"

```csharp
// ❌ BAD: Does everything
public class OrderManager
{
    public void CreateOrder() { }
    public void ProcessPayment() { }
    public void SendEmail() { }
    public void UpdateInventory() { }
    public void GenerateInvoice() { }
    public void CreateShipment() { }
}

// ✅ GOOD: Separate concerns
public class OrderService { }
public class PaymentService { }
public class EmailService { }
public class InventoryService { }
```

### ❌ Breaking LSP with Exceptions

```csharp
// ❌ BAD: Subclass changes behavior
public class FileReader
{
    public virtual string Read(string path)
    {
        return File.ReadAllText(path);
    }
}

public class SecureFileReader : FileReader
{
    public override string Read(string path)
    {
        throw new UnauthorizedException();  // ❌ Violates LSP!
    }
}
```

---

## Detailed Guidance

Common Pitfalls should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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

