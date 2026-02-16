# Interfaces

> Subject: [Core-CSharp](../README.md)

## Interfaces

### Interface vs Abstract Class

| Feature | Interface | Abstract Class |
|---------|-----------|----------------|
| **Multiple inheritance** | ✅ Yes | ❌ No (single) |
| **Implementation** | Default methods (C# 8+) | Yes |
| **Fields** | ❌ No | ✅ Yes |
| **Constructors** | ❌ No | ✅ Yes |
| **Access modifiers** | Public only | Any |
| **Use case** | Contract | Shared behavior |

### When to Use Each

```csharp
// ✅ Interface: Multiple implementations, no shared state
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(decimal amount);
}

public class StripeProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ProcessAsync(decimal amount) { /* ... */ }
}

public class PayPalProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ProcessAsync(decimal amount) { /* ... */ }
}

// ✅ Abstract class: Shared behavior and state
public abstract class PaymentProcessorBase
{
    protected readonly ILogger _logger;
    
    protected PaymentProcessorBase(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task<PaymentResult> ProcessAsync(decimal amount)
    {
        _logger.LogInformation($"Processing payment of {amount}");
        return await ProcessPaymentAsync(amount);
    }
    
    protected abstract Task<PaymentResult> ProcessPaymentAsync(decimal amount);
}
```

---


