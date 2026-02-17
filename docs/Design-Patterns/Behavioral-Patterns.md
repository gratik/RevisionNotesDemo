# Behavioral Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Object-oriented design fundamentals and refactoring familiarity.
- Related examples: docs/Design-Patterns/README.md
> Subject: [Design-Patterns](../README.md)

## Behavioral Patterns

**Purpose**: Manage algorithms and relationships between objects

### Strategy

**Problem**: Select algorithm at runtime

```csharp
// ✅ Strategy pattern
public interface IShippingStrategy
{
    decimal CalculateCost(decimal weight, string destination);
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 2m;
    }
}

public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 5m;
    }
}

public class ShippingCalculator
{
    private readonly IShippingStrategy _strategy;
    
    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public decimal Calculate(decimal weight, string destination)
    {
        return _strategy.CalculateCost(weight, destination);
    }
}

// Usage
var calculator = new ShippingCalculator(new ExpressShipping());
var cost = calculator.Calculate(10m, "NYC");
```

### Observer

**Problem**: Notify multiple objects about state changes

```csharp
// ✅ Observer pattern (using events)
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    
    private OrderStatus _status;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnStatusChanged(new OrderStatusChangedEventArgs { NewStatus = value });
            }
        }
    }
    
    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}

// Observers
public class EmailNotifier
{
    public void Subscribe(Order order)
    {
        order.StatusChanged += OnOrderStatusChanged;
    }
    
    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        // Send email notification
    }
}

public class InventoryUpdater
{
    public void Subscribe(Order order)
    {
        order.StatusChanged += OnOrderStatusChanged;
    }
    
    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        // Update inventory
    }
}
```

### Chain of Responsibility

**Problem**: Pass request along a chain of handlers

```csharp
// ✅ Chain of responsibility
public abstract class ValidationHandler<T>
{
    private ValidationHandler<T>? _next;
    
    public ValidationHandler<T> SetNext(ValidationHandler<T> next)
    {
        _next = next;
        return next;
    }
    
    public virtual ValidationResult Validate(T request)
    {
        var result = ValidateInternal(request);
        if (!result.IsValid)
            return result;
        
        return _next?.Validate(request) ?? ValidationResult.Success();
    }
    
    protected abstract ValidationResult ValidateInternal(T request);
}

public class LengthValidator : ValidationHandler<string>
{
    protected override ValidationResult ValidateInternal(string request)
    {
        if (request.Length < 3)
            return ValidationResult.Failure("Too short");
        return ValidationResult.Success();
    }
}

public class FormatValidator : ValidationHandler<string>
{
    protected override ValidationResult ValidateInternal(string request)
    {
        if (!request.Contains("@"))
            return ValidationResult.Failure("Invalid format");
        return ValidationResult.Success();
    }
}

// Usage
var chain = new LengthValidator();
chain.SetNext(new FormatValidator());

var result = chain.Validate("test@example.com");
```

---


## Interview Answer Block
30-second answer:
- Behavioral Patterns is about reusable design solutions for recurring software problems. It matters because pattern choice shapes long-term extensibility and readability.
- Use it when selecting pattern structure to simplify complex behavior.

2-minute answer:
- Start with the problem Behavioral Patterns solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: architectural consistency vs accidental overengineering.
- Close with one failure mode and mitigation: forcing patterns where straightforward code is enough.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Behavioral Patterns but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Behavioral Patterns, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Behavioral Patterns and map it to one concrete implementation in this module.
- 3 minutes: compare Behavioral Patterns with an alternative, then walk through one failure mode and mitigation.