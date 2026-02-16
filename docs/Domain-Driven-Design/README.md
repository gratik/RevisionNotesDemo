# Domain-Driven Design (DDD) Tactical Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Entity and aggregate modeling basics
- Related examples: Learning/DomainDrivenDesign/AggregateRootExamples.cs, Learning/DomainDrivenDesign/EntityValueObjectExamples.cs


## Module Metadata

- **Prerequisites**: OOP Principles, Design Patterns
- **When to Study**: When domain complexity starts driving architecture.
- **Related Files**: `../Learning/DomainDrivenDesign/*.cs`, `../Learning/Architecture/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Design-Patterns.md)
- **Next Step**: [Resilience.md](../Resilience.md)
<!-- STUDY-NAV-END -->


## Overview

Domain-Driven Design (DDD) is an approach to software development that focuses on modeling software to match the business domain. This guide covers **tactical patterns** - the building blocks for implementing rich domain models.

---

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

## Rich Domain Models vs Anemic Models

### Anemic Domain Model (Anti-pattern)

❌ **BAD:** Data bags with no behavior

```csharp
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public List<OrderLine> Lines { get; set; }
}

// Business logic scattered in services
public class OrderService
{
    public void AddLine(Order order, OrderLine line)
    {
        order.Lines.Add(line);
        order.Total = order.Lines.Sum(l => l.Total);
        // Rules not enforced consistently!
    }
}
```

### Rich Domain Model (Best Practice)

✅ **GOOD:** Behavior + data encapsulated together

```csharp
public class Order : AggregateRoot<OrderId>
{
    public Money Total { get; private set; }
    private readonly List<OrderLine> _lines = new();
    
    public void AddLine(ProductId productId, Money price, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify submitted order");
        
        var line = OrderLine.Create(productId, price, quantity);
        _lines.Add(line);
        RecalculateTotal();  // Invariant maintained!
    }
}
```

---

## Aggregate Design Rules

### Rule 1: Keep Aggregates Small

✅ **GOOD:** Order + OrderLines (2-3 entities)
❌ **BAD:** Customer → Orders → OrderLines → Products (huge aggregate)

### Rule 2: Reference Other Aggregates by ID

✅ **GOOD:**
```csharp
public class Order
{
    public CustomerId CustomerId { get; private set; }  // ID only
}
```

❌ **BAD:**
```csharp
public class Order
{
    public Customer Customer { get; set; }  // Creates huge aggregate
}
```

### Rule 3: One Repository Per Aggregate Root

```csharp
public interface IOrderRepository
{
    Task<Order?> GetAsync(OrderId id);
    Task SaveAsync(Order order);  // Saves order + lines together
}

// ❌ DON''T: Separate repository for OrderLine
// public interface IOrderLineRepository { }  // NO!
```

### Rule 4: Enforce Invariants Within Aggregate

Invariants (business rules) must always be true within the aggregate boundary.

Example: `Order.Total` must always equal the sum of `OrderLines`.

### Rule 5: Use Eventual Consistency Between Aggregates

Changes between aggregates use domain events for eventual consistency.

```csharp
public void Submit()
{
    Status = OrderStatus.Submitted;
    RaiseDomainEvent(new OrderSubmitted(Id, Total));
}

// Handler updates Inventory (different aggregate) in separate transaction
public class OrderSubmittedHandler
{
    public async Task Handle(OrderSubmitted evt)
    {
        var product = await _productRepo.GetAsync(evt.ProductId);
        product.ReserveStock(evt.Quantity);
        await _productRepo.SaveAsync(product);
    }
}
```

---

## Best Practices

✅ **DO:**
- Use entities for objects with identity
- Use value objects for descriptive attributes
- Make value objects immutable
- Validate in domain, not application layer
- Use factory methods for creation
- Keep aggregates small
- Reference other aggregates by ID
- Use domain events for cross-aggregate updates

❌ **DON''T:**
- Create anemic domain models
- Allow public setters on entities
- Reference aggregates directly
- Create huge aggregates
- Bypass aggregate root to modify children
- Use primitive obsession (use value objects for domain concepts)

---

## Example Files

📁 **Learning/DomainDrivenDesign/EntityValueObjectExamples.cs**
- Anemic vs Rich domain models
- Entity patterns
- Value Object patterns
- Domain validation
- Equality semantics

📁 **Learning/DomainDrivenDesign/AggregateRootExamples.cs**
- Aggregate boundaries
- Enforcing invariants
- Aggregate design rules
- Repository pattern for aggregates

---

## Further Reading

- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [Implementing DDD by Vaughn Vernon](https://vaughnvernon.com/books/)
- [Microsoft DDD Guide](https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Resilience.md](../Resilience.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Domain Driven Design and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Domain Driven Design and I would just follow best practices."
- Strong answer: "For Domain Driven Design, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Domain Driven Design in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Core Concepts](Core-Concepts.md)
- [Rich Domain Models vs Anemic Models](Rich-Domain-Models-vs-Anemic-Models.md)
- [Aggregate Design Rules](Aggregate-Design-Rules.md)
- [Best Practices](Best-Practices.md)
- [Example Files](Example-Files.md)
- [Further Reading](Further-Reading.md)


