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


