# Keyed Services (C# 12 / .NET 8+)

> Subject: [DotNet-Concepts](../README.md)

## Keyed Services (C# 12 / .NET 8+)

### Multiple Implementations

`csharp
// ✅ Register with keys
builder.Services.AddKeyedScoped<IPaymentService, StripePaymentService>("stripe");
builder.Services.AddKeyedScoped<IPaymentService, PayPalPaymentService>("paypal");

// ✅ Inject specific implementation by key
public class CheckoutService
{
    private readonly IPaymentService _paymentService;
    
    public CheckoutService([FromKeyedServices("stripe")] IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
}
`

---


