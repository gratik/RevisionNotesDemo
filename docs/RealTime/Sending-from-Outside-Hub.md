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


