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


