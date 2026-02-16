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


