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

## Detailed Guidance

Event IDs for Categorization guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Event IDs for Categorization before implementation work begins.
- Keep boundaries explicit so Event IDs for Categorization decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Event IDs for Categorization in production-facing code.
- When performance, correctness, or maintainability depends on consistent Event IDs for Categorization decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Event IDs for Categorization as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Event IDs for Categorization is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Event IDs for Categorization are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

