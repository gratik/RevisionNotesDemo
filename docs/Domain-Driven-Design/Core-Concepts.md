# Core Concepts

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core domain modeling concepts and layered architecture familiarity.
- Related examples: docs/Domain-Driven-Design/README.md
> Subject: [Domain-Driven-Design](../README.md)

## Core Concepts

### Entities

**Entities** have a unique identity that persists over time, even if their attributes change.

**Characteristics:**
- Identity-based equality (two entities with same ID are equal)
- Mutable (can change over time)
- Has lifecycle (created, modified, deleted)

**Example:**
```csharp
public class Customer : Entity<CustomerId>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    
    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Name required");
        Name = newName;
    }
}
```

### Value Objects

**Value Objects** have no identity - they are defined entirely by their values.

**Characteristics:**
- Value-based equality (all properties must match)
- Immutable (cannot change after creation)
- Can be shared/reused
- Self-validating

**Example:**
```csharp
public record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount + other.Amount, Currency);
    }
}

public record Email
{
    public string Value { get; }
    
    public Email(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException("Invalid email");
        Value = value;
    }
    
    private static bool IsValid(string email) => email.Contains(''@'');
}
```

### Aggregate Roots

**Aggregates** are clusters of entities and value objects treated as a unit for data changes. The **Aggregate Root** is the only entry point to the aggregate.

**Characteristics:**
- Enforces invariants across multiple entities
- Transaction boundary
- Accessed only through the root
- References other aggregates by ID only

**Example:**
```csharp
public class Order : AggregateRoot<OrderId>
{
    public Money Total { get; private set; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    
    public void AddLine(ProductId productId, Money price, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify submitted order");
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");
        
        var line = OrderLine.Create(productId, price, quantity);
        _lines.Add(line);
        RecalculateTotal();
        
        RaiseDomainEvent(new ProductAddedToOrder(Id, productId, quantity));
    }
    
    private void RecalculateTotal()
    {
        Total = _lines.Aggregate(Money.Zero, (sum, line) => sum.Add(line.LineTotal));
    }
}
```

---


## Interview Answer Block
30-second answer:
- Core Concepts is about domain modeling and bounded-context design. It matters because domain boundaries reduce ambiguity and integration friction.
- Use it when mapping business language into explicit aggregates and workflows.

2-minute answer:
- Start with the problem Core Concepts solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: model purity vs practical delivery constraints.
- Close with one failure mode and mitigation: anemic models and leaky bounded-context boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Core Concepts but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Core Concepts, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Core Concepts and map it to one concrete implementation in this module.
- 3 minutes: compare Core Concepts with an alternative, then walk through one failure mode and mitigation.