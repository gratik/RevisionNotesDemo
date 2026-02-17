# Aggregate Design Rules

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core domain modeling concepts and layered architecture familiarity.
- Related examples: docs/Domain-Driven-Design/README.md
> Subject: [Domain-Driven-Design](../README.md)

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

## Detailed Guidance

DDD guidance focuses on modeling behavior-rich domains with explicit invariants and clear aggregate boundaries.

### Design Notes
- Define success criteria for Aggregate Design Rules before implementation work begins.
- Keep boundaries explicit so Aggregate Design Rules decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Aggregate Design Rules in production-facing code.
- When performance, correctness, or maintainability depends on consistent Aggregate Design Rules decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Aggregate Design Rules as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Aggregate Design Rules is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Aggregate Design Rules are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Aggregate Design Rules is about domain modeling and bounded-context design. It matters because domain boundaries reduce ambiguity and integration friction.
- Use it when mapping business language into explicit aggregates and workflows.

2-minute answer:
- Start with the problem Aggregate Design Rules solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: model purity vs practical delivery constraints.
- Close with one failure mode and mitigation: anemic models and leaky bounded-context boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Aggregate Design Rules but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Aggregate Design Rules, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Aggregate Design Rules and map it to one concrete implementation in this module.
- 3 minutes: compare Aggregate Design Rules with an alternative, then walk through one failure mode and mitigation.