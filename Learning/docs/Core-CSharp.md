# Core C# Features (Generics, Delegates, Extension Methods)

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Basic C# syntax
- Related examples: Learning/CoreCSharpFeatures/GenericsAndConstraints.cs, Learning/CoreCSharpFeatures/DelegatesAndEvents.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Module Metadata

- **Prerequisites**: OOP Principles
- **When to Study**: After OOP basics; before frameworks and APIs.
- **Related Files**: `../CoreCSharpFeatures/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](OOP-Principles.md)
- **Next Step**: [DotNet-Concepts.md](DotNet-Concepts.md)
<!-- STUDY-NAV-END -->


## Overview

Core C# features like generics, delegates, extension methods, and interfaces are the foundation
of type-safe, reusable, and expressive code. This guide covers these essential language features
with practical examples, constraints, variance, and common patterns.

---

## Generics

### Why Generics?

**Without Generics (Boxing/Unboxing)**:
```csharp
// ❌ BAD: Boxing, no type safety
ArrayList list = new ArrayList();
list.Add(1);       // Boxes int to object
list.Add("two");   // No compile-time error!
int value = (int)list[0];  // Unboxing, runtime cast
```

**With Generics**:
```csharp
// ✅ GOOD: Type-safe, no boxing
List<int> list = new List<int>();
list.Add(1);
list.Add("two");  // ❌ Compile-time error!
int value = list[0];  // No cast needed
```

### Generic Classes

```csharp
// ✅ Generic repository pattern
public class Repository<T> where T : class
{
    private readonly List<T> _items = new();
    
    public void Add(T item) => _items.Add(item);
    
    public T? GetById(int id)
    {
        // Implementation
        return default;
    }
    
    public IEnumerable<T> GetAll() => _items;
}

// Usage
var userRepo = new Repository<User>();
var productRepo = new Repository<Product>();
```

### Generic Constraints

```csharp
// ✅ where T : class (reference type)
public class ReferenceTypeRepo<T> where T : class
{
    public T? GetOrDefault() => default;  // Can be null
}

// ✅ where T : struct (value type)
public class ValueTypeStorage<T> where T : struct
{
    public T GetOrDefault() => default;  // Never null
}

// ✅ where T : new() (parameterless constructor)
public class Factory<T> where T : new()
{
    public T Create() => new T();
}

// ✅ where T : IComparable (interface)
public class Sorter<T> where T : IComparable<T>
{
    public T Max(T a, T b) => a.CompareTo(b) > 0 ? a : b;
}

// ✅ Multiple constraints
public class EntityRepository<T> 
    where T : Entity, IValidatable, new()
{
    public T Create()
    {
        var entity = new T();
        if (!entity.IsValid())
            throw new InvalidOperationException();
        return entity;
    }
}
```

---

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

## Extension Methods

### Basic Extension Methods

```csharp
// ✅ Extend string without modifying the class
public static class StringExtensions
{
    public static bool IsBlank(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
    
    public static string Truncate(this string value, int maxLength)
    {
        if (value.Length <= maxLength) return value;
        return value.Substring(0, maxLength) + "...";
    }
}

// Usage
string name = "  ";
if (name.IsBlank())  // ✅ Reads like an instance method
{
    Console.WriteLine("Name is blank");
}

string longText = "This is a very long text";
string short = longText.Truncate(10);  // "This is a ..."
```

### Extension Methods on Generics

```csharp
// ✅ Extend IEnumerable<T>
public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }
    
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(x => x != null)!;
    }
}

// Usage
var numbers = new List<int>();
if (numbers.IsEmpty())  // ✅ More readable than !numbers.Any()
{
    Console.WriteLine("List is empty");
}
```

---

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

## Covariance and Contravariance

### Covariance (out)

```csharp
// ✅ Covariant interface (output only)
public interface IProducer<out T>
{
    T Produce();
}

public class Animal { }
public class Dog : Animal { }

public class DogProducer : IProducer<Dog>
{
    public Dog Produce() => new Dog();
}

// ✅ Covariance allows this
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer;  // ✅ OK!
```

### Contravariance (in)

```csharp
// ✅ Contravariant interface (input only)
public interface IConsumer<in T>
{
    void Consume(T item);
}

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal animal) { /* ... */ }
}

// ✅ Contravariance allows this
IConsumer<Animal> animalConsumer = new AnimalConsumer();
IConsumer<Dog> dogConsumer = animalConsumer;  // ✅ OK!
```

---

## Best Practices

### ✅ Generics
- Use constraints to make APIs type-safe
- Prefer `List<T>` over `ArrayList` (always)
- Use `IEnumerable<T>` for read-only sequences
- Avoid over-constraining (only add constraints you need)

### ✅ Delegates and Events
- Use `EventHandler<TEventArgs>` for events
- Use `Func<T>` and `Action<T>` instead of custom delegates
- Always check for null before invoking events (`?.Invoke`)
- Unsubscribe from events to prevent memory leaks

### ✅ Extension Methods
- Put in static classes with clear names (`StringExtensions`)
- Don't overuse (prefer instance methods when you own the class)
- Use for "utility" methods on types you don't control
- Make them discoverable (good naming)

### ✅ Interfaces
- Prefer composition over inheritance
- Keep interfaces small (ISP)
- Use interfaces for dependency injection
- Name interfaces descriptively (`IRepository`, not `IRepo`)

---

## Common Pitfalls

### ❌ Not Unsubscribing from Events

```csharp
// ❌ BAD: Memory leak!
public class Subscriber
{
    public Subscriber(Publisher publisher)
    {
        publisher.SomeEvent += OnSomeEvent;  // ❌ Never unsubscribes
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}

// ✅ GOOD: Unsubscribe
public class Subscriber : IDisposable
{
    private readonly Publisher _publisher;
    
    public Subscriber(Publisher publisher)
    {
        _publisher = publisher;
        _publisher.SomeEvent += OnSomeEvent;
    }
    
    public void Dispose()
    {
        _publisher.SomeEvent -= OnSomeEvent;  // ✅ Unsubscribe
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}
```

### ❌ Overusing Extension Methods

```csharp
// ❌ BAD: Should be instance method
public static class UserExtensions
{
    public static void SetPassword(this User user, string password)
    {
        user.PasswordHash = HashPassword(password);  // ❌ Should be in User class
    }
}

// ✅ GOOD: Instance method
public class User
{
    public void SetPassword(string password)
    {
        PasswordHash = HashPassword(password);  // ✅ Encapsulated
    }
}
```

---

## Related Files

- [CoreCSharpFeatures/GenericsAndConstraints.cs](../CoreCSharpFeatures/GenericsAndConstraints.cs)
- [CoreCSharpFeatures/DelegatesAndEvents.cs](../CoreCSharpFeatures/DelegatesAndEvents.cs)
- [CoreCSharpFeatures/ExtensionMethods.cs](../CoreCSharpFeatures/ExtensionMethods.cs)
- [CoreCSharpFeatures/PolymorphismDemo.cs](../CoreCSharpFeatures/PolymorphismDemo.cs)
- [CoreCSharpFeatures/AbstractClassVsInterface.cs](../CoreCSharpFeatures/AbstractClassVsInterface.cs)
- [CoreCSharpFeatures/CovarianceContravariance.cs](../CoreCSharpFeatures/CovarianceContravariance.cs)

---

## See Also

- [Modern C#](Modern-CSharp.md) - Records, pattern matching, nullable reference types
- [OOP Principles](OOP-Principles.md) - SOLID principles
- [LINQ and Queries](LINQ-Queries.md) - LINQ uses extension methods
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

Generated: 2026-02-13

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [DotNet-Concepts.md](DotNet-Concepts.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Core CSharp and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Core CSharp and I would just follow best practices."
- Strong answer: "For Core CSharp, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Core CSharp in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
