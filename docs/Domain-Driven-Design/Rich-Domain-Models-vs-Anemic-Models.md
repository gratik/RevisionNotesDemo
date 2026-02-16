# Rich Domain Models vs Anemic Models

> Subject: [Domain-Driven-Design](../README.md)

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

## Detailed Guidance

DDD guidance focuses on modeling behavior-rich domains with explicit invariants and clear aggregate boundaries.

### Design Notes
- Define success criteria for Rich Domain Models vs Anemic Models before implementation work begins.
- Keep boundaries explicit so Rich Domain Models vs Anemic Models decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Rich Domain Models vs Anemic Models in production-facing code.
- When performance, correctness, or maintainability depends on consistent Rich Domain Models vs Anemic Models decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Rich Domain Models vs Anemic Models as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Rich Domain Models vs Anemic Models is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Rich Domain Models vs Anemic Models are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

