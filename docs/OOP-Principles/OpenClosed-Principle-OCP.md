# Open/Closed Principle (OCP)

> Subject: [OOP-Principles](../README.md)

## Open/Closed Principle (OCP)

**"Software entities should be open for extension, but closed for modification."**

### ❌ Violation

```csharp
// ❌ BAD: Must modify class to add new discount types
public class DiscountCalculator
{
    public decimal Calculate(decimal amount, string discountType)
    {
        if (discountType == "PERCENTAGE")
            return amount * 0.9m;
        else if (discountType == "FIXED")
            return amount - 10m;
        else if (discountType == "BOGO")  // New type added
            return amount * 0.5m;
        
        return amount;
    }
}
// Adding new discount types requires modifying this class
```

### ✅ Solution

```csharp
// ✅ GOOD: Extend with new classes, don't modify existing
public interface IDiscountStrategy
{
    decimal Calculate(decimal amount);
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percentage;
    public PercentageDiscount(decimal percentage) => _percentage = percentage;
    
    public decimal Calculate(decimal amount) => amount * (1 - _percentage);
}

public class FixedDiscount : IDiscountStrategy
{
    private readonly decimal _amount;
    public FixedDiscount(decimal amount) => _amount = amount;
    
    public decimal Calculate(decimal amount) => amount - _amount;
}

public class BogoDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal amount) => amount * 0.5m;
}

// Usage
IDiscountStrategy discount = new PercentageDiscount(0.1m);
var finalPrice = discount.Calculate(100m);
// Adding new discount types: just create a new class
```

---

## Detailed Guidance

Open/Closed Principle (OCP) should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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

