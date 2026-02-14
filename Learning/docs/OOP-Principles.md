# Object-Oriented Programming Principles (SOLID)

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

SOLID principles are five fundamental design guidelines that make object-oriented code more maintainable,
flexible, and testable. This guide covers each principle with concrete examples, common violations, and
practical applications. We also cover complementary principles like KISS, DRY, YAGNI, and Tell-Don't-Ask.

---

## SOLID Overview

| Principle | Key Idea | Benefit |
|-----------|----------|---------|
| **S**ingle Responsibility | One class, one reason to change | Easier maintenance |
| **O**pen/Closed | Open for extension, closed for modification | Safe evolution |
| **L**iskov Substitution | Subtypes must be substitutable | Correct inheritance |
| **I**nterface Segregation | No fat interfaces | Focused contracts |
| **D**ependency Inversion | Depend on abstractions | Loose coupling |

---

## Single Responsibility Principle (SRP)

**"A class should have one, and only one, reason to change."**

### ❌ Violation

```csharp
// ❌ BAD: Multiple responsibilities (data, validation, persistence, email)
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && Email.Contains("@");
    }
    
    public void SaveToDatabase()
    {
        // Database logic here...
    }
    
    public void SendWelcomeEmail()
    {
        // Email logic here...
    }
}
// Changes to database, validation, or email logic all affect this class
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate responsibilities
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserValidator
{
    public bool IsValid(User user)
    {
        return !string.IsNullOrEmpty(user.Name) && user.Email.Contains("@");
    }
}

public class UserRepository
{
    public void Save(User user)
    {
        // Database logic
    }
}

public class EmailService
{
    public void SendWelcomeEmail(User user)
    {
        // Email logic
    }
}
// Each class has one reason to change
```

---

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

## Liskov Substitution Principle (LSP)

**"Objects of a superclass should be replaceable with objects of a subclass without breaking the application."**

### ❌ Violation

```csharp
// ❌ BAD: Square violates the Rectangle contract
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
    
    public override int Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
}

// Test
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
Console.WriteLine(rect.CalculateArea());  // Expected 50, got 100!
// Square is NOT a valid substitute for Rectangle
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate hierarchies or use composition
public interface IShape
{
    int CalculateArea();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    
    public int CalculateArea() => Side * Side;
}
// Both implement the same contract correctly
```

---

## Interface Segregation Principle (ISP)

**"No client should be forced to depend on methods it does not use."**

### ❌ Violation

```csharp
// ❌ BAD: Fat interface forces clients to implement unused methods
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() { /* ... */ }
    public void Eat() { throw new NotImplementedException(); }  // ❌ Doesn't eat!
    public void Sleep() { throw new NotImplementedException(); }  // ❌ Doesn't sleep!
}
```

### ✅ Solution

```csharp
// ✅ GOOD: Segregated interfaces
public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public class Human : IWorkable, IFeedable, ISleepable
{
    public void Work() { /* ... */ }
    public void Eat() { /* ... */ }
    public void Sleep() { /* ... */ }
}

public class Robot : IWorkable
{
    public void Work() { /* ... */ }
    // Only implements what it needs
}
```

---

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

## Other Important Principles

### KISS (Keep It Simple, Stupid)

```csharp
// ❌ BAD: Overengineered
public class StringManipulator
{
    public string ReverseString(string input)
    {
        return new string(input.ToCharArray().Reverse().ToArray());
    }
}

// ✅ GOOD: Simple and clear
public static string Reverse(string input)
{
    char[] chars = input.ToCharArray();
    Array.Reverse(chars);
    return new string(chars);
}
```

### DRY (Don't Repeat Yourself)

```csharp
// ❌ BAD: Duplication
public void SaveUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))
        throw new ArgumentException("Name required");
    // Save...
}

public void UpdateUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))  // ❌ Duplicated validation
        throw new ArgumentException("Name required");
    // Update...
}

// ✅ GOOD: Extract common logic
private void ValidateUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))
        throw new ArgumentException("Name required");
}

public void SaveUser(User user)
{
    ValidateUser(user);
    // Save...
}

public void UpdateUser(User user)
{
    ValidateUser(user);
    // Update...
}
```

### YAGNI (You Aren't Gonna Need It)

```csharp
// ❌ BAD: Speculative features
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }  // ❌ Not needed yet
    public Address ShippingAddress { get; set; }  // ❌ Not needed yet
    public Address BillingAddress { get; set; }  // ❌ Not needed yet
}

// ✅ GOOD: Only what's needed now
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    // Add other properties when actually needed
}
```

### Tell-Don't-Ask

```csharp
// ❌ BAD: Asking for data and making decisions
public void ProcessOrder(Order order)
{
    if (order.Status == OrderStatus.Pending)
    {
        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
    }
}

// ✅ GOOD: Tell the object what to do
public class Order
{
    public OrderStatus Status { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    
    public void Confirm()
    {
        if (Status == OrderStatus.Pending)
        {
            Status = OrderStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
        }
    }
}

// Usage
order.Confirm();  // ✅ Encapsulated behavior
```

---

## Best Practices

### ✅ Applying SOLID
- **SRP**: Each class should do one thing well
- **OCP**: Design for extension (strategy pattern, inheritance)
- **LSP**: Ensure derived classes don't break base class behavior
- **ISP**: Keep interfaces small and focused
- **DIP**: Always inject dependencies via constructor

### ✅ When to Apply
- Use SOLID for **core domain logic** (services, repositories)
- Don't overengineer simple **DTOs or data classes**
- Apply principles when code becomes **hard to test or change**
- Refactor toward SOLID as complexity grows

### ✅ Balance Pragmatism
- SOLID is a guide, not a law
- Start simple, refactor when needed (YAGNI)
- Avoid over-abstraction
- Consider team size and project lifetime

---

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

## Related Files

- [OOPPrinciples/SingleResponsibilityPrinciple.cs](../OOPPrinciples/SingleResponsibilityPrinciple.cs)
- [OOPPrinciples/OpenClosedPrinciple.cs](../OOPPrinciples/OpenClosedPrinciple.cs)
- [OOPPrinciples/LiskovSubstitutionPrinciple.cs](../OOPPrinciples/LiskovSubstitutionPrinciple.cs)
- [OOPPrinciples/InterfaceSegregationPrinciple.cs](../OOPPrinciples/InterfaceSegregationPrinciple.cs)
- [OOPPrinciples/DependencyInversionPrinciple.cs](../OOPPrinciples/DependencyInversionPrinciple.cs)
- [OOPPrinciples/KISSDRYYAGNIExamples.cs](../OOPPrinciples/KISSDRYYAGNIExamples.cs)
- [OOPPrinciples/TellDontAskPrinciple.cs](../OOPPrinciples/TellDontAskPrinciple.cs)

---

## See Also

- [Design Patterns](Design-Patterns.md) - Practical implementations of SOLID
- [Core C# Features](Core-CSharp.md) - Language features supporting OOP
- [Practical Patterns](Practical-Patterns.md) - Patterns in real applications
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
