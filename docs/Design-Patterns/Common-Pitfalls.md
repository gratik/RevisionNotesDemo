# Common Pitfalls

> Subject: [Design-Patterns](../README.md)

## Common Pitfalls

### ❌ Pattern Overuse

```csharp
// ❌ BAD: Over-engineered simple logic
public class AdditionStrategyFactory
{
    public ICalculationStrategy CreateStrategy()
    {
        return new AdditionStrategy();
    }
}

// ✅ GOOD: Simple is better
public int Add(int a, int b) => a + b;
```

### ❌ Singleton Abuse

```csharp
// ❌ BAD: Everything is singleton
public class UserService  // Singleton
public class OrderService  // Singleton
public class PaymentService  // Singleton

// ✅ GOOD: Use dependency injection
services.AddScoped<IUserService, UserService>();
services.AddScoped<IOrderService, OrderService>();
```

---


