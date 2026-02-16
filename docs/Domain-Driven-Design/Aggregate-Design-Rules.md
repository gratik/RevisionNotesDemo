# Aggregate Design Rules

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


