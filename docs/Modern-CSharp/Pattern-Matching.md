# Pattern Matching

> Subject: [Modern-CSharp](../README.md)

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


