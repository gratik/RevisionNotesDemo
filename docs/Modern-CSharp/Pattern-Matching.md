# Pattern Matching

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# concepts, nullable awareness, and common refactoring patterns.
- Related examples: docs/Modern-CSharp/README.md
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

## Detailed Guidance

Pattern Matching should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

### Design Notes
- Use language features to enforce intent at compile time (constraints, nullability, variance).
- Keep APIs narrow and intention-revealing; avoid generic over-engineering.
- Prefer composition and small interfaces over deep inheritance chains.
- Document where performance optimizations justify additional complexity.

### When To Use
- When building reusable libraries or framework-facing APIs.
- When replacing runtime casts/dynamic code with typed contracts.
- When teaching or reviewing core language design tradeoffs.

### Anti-Patterns To Avoid
- Public APIs with too many type parameters and unclear semantics.
- Constraints that do not correspond to required operations.
- Using reflection/dynamic where static typing is sufficient.

## Practical Example

- Start with a concrete implementation and extract generic behavior only when duplication appears.
- Add minimal constraints needed for compile-time guarantees.
- Validate with tests across reference and value type scenarios.

## Validation Checklist

- API signatures are understandable without deep internal context.
- Nullability and constraints match true invariants.
- Type misuse fails at compile time where possible.
- Benchmarks exist for any non-trivial performance optimizations.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Pattern Matching is about newer C# language capabilities. It matters because modern syntax reduces boilerplate and improves intent clarity.
- Use it when updating legacy code to safer and more expressive patterns.

2-minute answer:
- Start with the problem Pattern Matching solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: new language features vs team familiarity and consistency.
- Close with one failure mode and mitigation: mixing old and new idioms inconsistently across the same codebase.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Pattern Matching but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Pattern Matching, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Pattern Matching and map it to one concrete implementation in this module.
- 3 minutes: compare Pattern Matching with an alternative, then walk through one failure mode and mitigation.