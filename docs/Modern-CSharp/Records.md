# Records

> Subject: [Modern-CSharp](../README.md)

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


