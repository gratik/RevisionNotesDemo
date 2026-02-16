# Event IDs for Categorization

> Subject: [Logging-Observability](../README.md)

## Event IDs for Categorization

```csharp
public static class EventIds
{
    public static readonly EventId OrderCreated = new(1001, "OrderCreated");
    public static readonly EventId OrderShipped = new(1002, "OrderShipped");
    public static readonly EventId PaymentFailed = new(2001, "PaymentFailed");
    public static readonly EventId DatabaseError = new(3001, "DatabaseError");
}

// ✅ Use event IDs for filtering and alerting
_logger.LogInformation(EventIds.OrderCreated, "Order {OrderId} created", orderId);
_logger.LogError(EventIds.PaymentFailed, ex, "Payment failed for order {OrderId}", orderId);

// Can filter/alert on specific event IDs
// Alert on EventId=2001 (PaymentFailed)
// Dashboard for EventId=1001 (OrderCreated)
```

---


