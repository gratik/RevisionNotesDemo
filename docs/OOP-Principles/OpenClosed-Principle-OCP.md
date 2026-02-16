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


