# Modern C# Features (Records, Pattern Matching)

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Core C# fundamentals
- Related examples: Learning/ModernCSharp/RecordsAndRecordStructs.cs, Learning/ModernCSharp/PatternMatchingExamples.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#
- **When to Study**: After language fundamentals to modernize style and safety.
- **Related Files**: `../Learning/ModernCSharp/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](OOP-Principles.md)
- **Next Step**: [LINQ-Queries.md](LINQ-Queries.md)
<!-- STUDY-NAV-END -->


## Overview

Modern C# features (C# 7-12) introduce records, pattern matching, nullable reference types,
init-only properties, and more expressive syntax. These features improve code safety, readability,
and maintainability when used appropriately.

---

## Records

### Record Classes (Reference Types)

```csharp
// ✅ Concise immutable data class
public record Customer(string Name, string Email, int LoyaltyTier);

// Compiler generates:
// - Constructor
// - Properties (init-only)
// - Equals/GetHashCode (value-based)
// - ToString
// - Deconstruct
// - Copy (with expressions)

// Usage
var customer = new Customer("Alice", "alice@example.com", 3);
var modified = customer with { LoyaltyTier = 4 };  // ✅ Non-destructive mutation

Console.WriteLine(customer);  // Customer { Name = Alice, Email = alice@example.com, LoyaltyTier = 3 }

// ✅ Value-based equality
var c1 = new Customer("Bob", "bob@example.com", 1);
var c2 = new Customer("Bob", "bob@example.com", 1);
Console.WriteLine(c1 == c2);  // True (same values)
```

### Record Structs (Value Types)

```csharp
// ✅ Record struct (C# 10+)
public readonly record struct Point(int X, int Y);

var p1 = new Point(1, 2);
var p2 = p1 with { X = 5 };  // ✅ Non-destructive mutation
Console.WriteLine(p1 == p2);  // False
```

### When to Use Records

**✅ Use Records For**:
- DTOs (Data Transfer Objects)
- Value objects (Money, Address, Coordinate)
- Immutable data models
- API request/response models

**❌ Don't Use Records For**:
- Entities with identity (use classes)
- Mutable state (use classes)
- Performance-critical value types (use structs)

---

## Pattern Matching

### Type Patterns

```csharp
// ✅ Type pattern with type check and cast
public decimal CalculatePrice(object item)
{
    return item switch
    {
        Book book => book.Price * 0.9m,           // 10% off books
        Electronics e => e.Price * 0.85m,          // 15% off electronics
        Clothing c when c.IsOnSale => c.Price * 0.5m,  // 50% off sale clothing
        Clothing c => c.Price * 0.8m,              // 20% off regular clothing
        _ => throw new ArgumentException("Unknown item type")
    };
}
```

### Property Patterns

```csharp
// ✅ Match on properties
public string GetCustomerTier(Customer customer) => customer switch
{
    { LoyaltyPoints: >= 10000 } => "Platinum",
    { LoyaltyPoints: >= 5000 } => "Gold",
    { LoyaltyPoints: >= 1000 } => "Silver",
    _ => "Bronze"
};

// ✅ Nested property patterns
public string GetShippingCost(Order order) => order switch
{
    { ShippingAddress: { Country: "US" }, Total: > 100 } => "Free",
    { ShippingAddress: { Country: "US" } } => "$10",
    { ShippingAddress: { Country: "CA" } } => "$15",
    _ => "$25"
};
```

### Relational Patterns

```csharp
// ✅ Relational operators in patterns
public string GetAgeGroup(int age) => age switch
{
    < 13 => "Child",
    >= 13 and < 20 => "Teenager",
    >= 20 and < 65 => "Adult",
    >= 65 => "Senior",
    _ => "Unknown"
};
```

### List Patterns (C# 11)

```csharp
// ✅ Match on list structure
public string DescribeList(int[] numbers) => numbers switch
{
    [] => "Empty",
    [var x] => $"Single item: {x}",
    [var first, var second] => $"Two items: {first}, {second}",
    [var first, .., var last] => $"Multiple items from {first} to {last}",
    _ => "Unknown"
};
```

---

## Nullable Reference Types

### Enabling Nullable Context

```xml
<!-- In .csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### Non-Nullable by Default

```csharp
// ✅ Compiler helps prevent NullReferenceException
public class User
{
    public string Name { get; set; }  // ⚠️ Warning: Non-nullable property must contain non-null value
    public string? NickName { get; set; }  // ✅ Explicitly nullable
}

// ✅ Constructor ensures non-null
public class User
{
    public string Name { get; set; }
    
    public User(string name)
    {
        Name = name;  // ✅ Initialized in constructor
    }
}
```

### Null-Forgiving Operator (!)

```csharp
// ✅ Tell compiler: "I know this won't be null"
public void ProcessUser(string? name)
{
    if (name == null)
        throw new ArgumentNullException(nameof(name));
    
    // After null check, but compiler doesn't know
    DoSomething(name!);  // ✅ Suppress warning
}
```

### Nullable Annotations

```csharp
// ✅ Method returns null if not found
public User? FindUserById(int id)
{
    return _users.FirstOrDefault(u => u.Id == id);  // ✅ Returns null if not found
}

// ✅ Parameter can be null
public void Log(string? message)
{
    Console.WriteLine(message ?? "No message");
}
```

---

## Init-Only Properties

```csharp
// ✅ Set during object initialization only
public class User
{
    public int Id { get; init; }  // ✅ Can only set during initialization
    public string Name { get; init; } = string.Empty;
}

// Usage
var user = new User
{
    Id = 1,
    Name = "Alice"
};

user.Id = 2;  // ❌ Compile error: init-only property
```

---

## Other Modern Features

### Top-Level Statements (C# 9)

```csharp
// ✅ No need for Program class and Main method
using System;

Console.WriteLine("Hello, World!");
var result = AddNumbers(3, 5);
Console.WriteLine(result);

int AddNumbers(int a, int b) => a + b;
```

### Global Usings (C# 10)

```csharp
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;

// Now available in all files without explicit using
```

### File-Scoped Namespaces (C# 10)

```csharp
// ❌ Old style: extra indentation
namespace MyApp.Services
{
    public class UserService
    {
        // ...
    }
}

// ✅ New style: less indentation
namespace MyApp.Services;

public class UserService
{
    // ...
}
```

### Required Members (C# 11)

```csharp
// ✅ Compiler ensures property is set
public class User
{
    public required string Name { get; init; }
    public string? Email { get; init; }
}

// Usage
var user1 = new User { Name = "Alice" };  // ✅ OK
var user2 = new User { };  // ❌ Compile error: Name is required
```

### Raw String Literals (C# 11)

```csharp
// ✅ No need to escape quotes
var json = """
    {
        "name": "Alice",
        "email": "alice@example.com"
    }
    """;

var query = """
    SELECT * FROM Users
    WHERE Name = 'Alice'
    """;
```

---

## Best Practices

### ✅ Records
- Use for immutable DTOs and value objects
- Prefer `record` over `class` for data transfer
- Use `with` expressions for non-destructive updates
- Don't use for entities with identity

### ✅ Pattern Matching
- Use `switch` expressions for multiple branches
- Combine with LINQ for powerful queries
- Use property patterns to eliminate null checks
- Prefer pattern matching over multiple if/else

### ✅ Nullable Reference Types
- Enable `<Nullable>enable</Nullable>` in all projects
- Use `?` to explicitly mark nullable references
- Initialize properties in constructors
- Use `required` for mandatory properties (C# 11+)

### ✅ Init-Only Properties
- Use for immutable objects
- Combine with records for clean syntax
- Prefer over constructor-only initialization

---

## Common Pitfalls

### ❌ Using Records for Mutable State

```csharp
// ❌ BAD: Fighting record semantics
public record User(string Name)
{
    public string Name { get; set; } = Name;  // ❌ Mutable record
}

// ✅ GOOD: Use class for mutable state
public class User
{
    public string Name { get; set; } = string.Empty;
}
```

### ❌ Over-Using Pattern Matching

```csharp
// ❌ BAD: Simple if is clearer
var result = value switch
{
    null => "null",
    _ => "not null"
};

// ✅ GOOD: Use simple if
var result = value == null ? "null" : "not null";
```

### ❌ Ignoring Nullable Warnings

```csharp
// ❌ BAD: Suppressing all warnings
#nullable disable
public string GetName(User user)
{
    return user.Name;  // ❌ Could be null!
}

// ✅ GOOD: Handle nulls properly
public string GetName(User? user)
{
    if (user == null)
        throw new ArgumentNullException(nameof(user));
    
    return user.Name ?? "Unknown";
}
```

---

## Related Files

- [ModernCSharp/PatternMatchingExamples.cs](../Learning/ModernCSharp/PatternMatchingExamples.cs)
- [ModernCSharp/NullableReferenceTypes.cs](../Learning/ModernCSharp/NullableReferenceTypes.cs)
- [ModernCSharp/InitOnlyProperties.cs](../Learning/ModernCSharp/InitOnlyProperties.cs)
- [ModernCSharp/RecordsAndRecordStructs.cs](../Learning/ModernCSharp/RecordsAndRecordStructs.cs)

---

## See Also

- [Core C# Features](Core-CSharp.md) - Generics, delegates, extension methods
- [Performance](Performance.md) - Record structs for performance
- [OOP Principles](OOP-Principles.md) - When to use records vs classes
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [LINQ-Queries.md](LINQ-Queries.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Modern CSharp and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Modern CSharp and I would just follow best practices."
- Strong answer: "For Modern CSharp, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Modern CSharp in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
