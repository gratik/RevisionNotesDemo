# Delegates and Events

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# syntax, object-oriented fundamentals, and basic collection usage.
- Related examples: docs/Core-CSharp/README.md
> Subject: [Core-CSharp](../README.md)

## Delegates and Events

### Delegates (Type-Safe Function Pointers)

```csharp
// ✅ Delegate declaration
public delegate int BinaryOperation(int a, int b);

public class Calculator
{
    public int Execute(BinaryOperation operation, int a, int b)
    {
        return operation(a, b);
    }
}

// Usage
int Add(int a, int b) => a + b;
int Multiply(int a, int b) => a * b;

var calc = new Calculator();
var result1 = calc.Execute(Add, 3, 5);       // 8
var result2 = calc.Execute(Multiply, 3, 5);  // 15
var result3 = calc.Execute((a, b) => a - b, 10, 3);  // 7 (lambda)
```

### Built-in Delegates: Func, Action, Predicate

```csharp
// ✅ Func<T, TResult> - returns a value
Func<int, int, int> add = (a, b) => a + b;
int sum = add(3, 5);  // 8

// ✅ Action<T> - returns void
Action<string> log = message => Console.WriteLine(message);
log("Hello");  // Prints "Hello"

// ✅ Predicate<T> - returns bool
Predicate<int> isEven = n => n % 2 == 0;
bool result = isEven(4);  // true
```

### Events

```csharp
// ✅ Publisher
public class Order
{
    // Event declaration
    public event EventHandler<OrderEventArgs>? OrderPlaced;
    
    public void PlaceOrder()
    {
        // Process order...
        
        // Raise event
        OrderPlaced?.Invoke(this, new OrderEventArgs { OrderId = 123 });
    }
}

public class OrderEventArgs : EventArgs
{
    public int OrderId { get; set; }
}

// ✅ Subscriber
public class EmailService
{
    public void Subscribe(Order order)
    {
        order.OrderPlaced += OnOrderPlaced;
    }
    
    private void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"Sending email for order {e.OrderId}");
    }
}

// Usage
var order = new Order();
var emailService = new EmailService();
emailService.Subscribe(order);
order.PlaceOrder();  // Triggers email
```

---

## Detailed Guidance

Delegates and events guidance focuses on decoupled behavior composition, safe invocation patterns, and leak-free subscription lifecycles.

### Design Notes
- Define success criteria for Delegates and Events before implementation work begins.
- Keep boundaries explicit so Delegates and Events decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Delegates and Events in production-facing code.
- When performance, correctness, or maintainability depends on consistent Delegates and Events decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Delegates and Events as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Delegates and Events is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Delegates and Events are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Delegates and Events is about core C# language features and API design. It matters because it directly affects correctness, readability, and maintainability.
- Use it when designing reusable domain and application abstractions.

2-minute answer:
- Start with the problem Delegates and Events solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: flexibility vs API complexity.
- Close with one failure mode and mitigation: over-abstracting simple code paths; keep public contracts intentional.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Delegates and Events but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Delegates and Events, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Delegates and Events and map it to one concrete implementation in this module.
- 3 minutes: compare Delegates and Events with an alternative, then walk through one failure mode and mitigation.