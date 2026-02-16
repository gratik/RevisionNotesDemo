# Service Location (Anti-Pattern)

> Subject: [DotNet-Concepts](../README.md)

## Service Location (Anti-Pattern)

### Don't Use Service Locator

`csharp
// ❌ BAD: Service locator pattern (anti-pattern)
public class OrderService
{
    private readonly IServiceProvider _serviceProvider;
    
    public OrderService(IServiceProvider serviceProvider)  // ❌ Don't do this
    {
        _serviceProvider = serviceProvider;
    }
    
    public void ProcessOrder()
    {
        var emailService = _serviceProvider.GetService<IEmailService>();  // ❌ Hidden dependency
    }
}

// ✅ GOOD: Explicit constructor injection
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)  // ✅ Clear dependency
    {
        _emailService = emailService;
    }
}
`

**Exception**: Acceptable in factories or when dynamically resolving services

---


