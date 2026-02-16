# Sending from Outside Hub

> Subject: [RealTime](../README.md)

## Sending from Outside Hub

### Using IHubContext

`csharp
// ✅ Inject IHubContext in regular service
public class OrderService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    
    public OrderService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task ProcessOrder(Order order)
    {
        // Process order...
        
        // ✅ Send notification to all clients
        await _hubContext.Clients.All.SendAsync(
            "OrderProcessed", 
            order.Id);
        
        // ✅ Send to specific user
        await _hubContext.Clients.Group($"user-{order.UserId}")
            .SendAsync("OrderProcessed", order.Id);
    }
}

// ✅ With strongly-typed hub
public class OrderService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    
    public async Task ProcessOrder(Order order)
    {
        await _hubContext.Clients.All.OrderProcessed(order.Id);
    }
}
`

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Sending from Outside Hub before implementation work begins.
- Keep boundaries explicit so Sending from Outside Hub decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Sending from Outside Hub in production-facing code.
- When performance, correctness, or maintainability depends on consistent Sending from Outside Hub decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Sending from Outside Hub as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Sending from Outside Hub is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Sending from Outside Hub are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

