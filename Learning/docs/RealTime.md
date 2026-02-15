# Real-Time Communication with SignalR

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Web API and async basics
- Related examples: Learning/RealTime/SignalRBasics.cs, Learning/WebAPI/WebSocketsRealTime.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Module Metadata

- **Prerequisites**: Web API and MVC, Async Multithreading
- **When to Study**: After baseline API work; when introducing push/streaming features.
- **Related Files**: `../RealTime/*.cs`, `../WebAPI/WebSocketsRealTime.cs`, `../WebAPI/ServerSentEventsSSE.cs`
- **Estimated Time**: 60-90 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Configuration.md)
- **Next Step**: [gRPC.md](gRPC.md)
<!-- STUDY-NAV-END -->


## Overview

SignalR enables real-time, bidirectional communication between server and clients using WebSockets
(with automatic fallback to Server-Sent Events or Long Polling). This guide covers hubs, groups,
connection management, and production patterns.

---

## What is SignalR?

**SignalR** = Persistent connection for server-to-client push messaging

### Traditional HTTP vs SignalR

| | HTTP | SignalR |
|---|------|---------|
| **Direction** | Client → Server | Bidirectional |
| **Connection** | Request/Response | Persistent |
| **Server Push** | ❌ (need polling) | ✅ Native |
| **Use Cases** | REST APIs | Chat, notifications, live updates |

### Transport Fallback

SignalR automatically chooses best transport:
1. **WebSockets** (best) - Full duplex, real-time
2. **Server-Sent Events** (fallback) - Server to client only
3. **Long Polling** (last resort) - Simulates real-time

---

## Basic Hub

### Creating a Hub

`csharp
// ✅ Simple chat hub
public class ChatHub : Hub
{
    // ✅ Clients can invoke this method
    public async Task SendMessage(string user, string message)
    {
        // ✅ Broadcast to ALL connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

// ✅ Register SignalR
builder.Services.AddSignalR();

// ✅ Map hub endpoint
app.MapHub<ChatHub>("/chatHub");
`

### JavaScript Client

`javascript
// ✅ Connect to hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// ✅ Listen for messages from server
connection.on("ReceiveMessage", (user, message) => {
    console.log(${user}: );
    // Update UI
});

// ✅ Start connection
await connection.start();
console.log("Connected!");

// ✅ Invoke server method
await connection.invoke("SendMessage", "Alice", "Hello World!");

// ✅ Stop connection
await connection.stop();
`

---

## Sending Messages

### Broadcast to All Clients

`csharp
public class NotificationHub : Hub
{
    // ✅ Send to everyone
    public async Task BroadcastNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
`

### Send to Specific Client

`csharp
public class ChatHub : Hub
{
    // ✅ Send to specific connection
    public async Task SendPrivateMessage(string connectionId, string message)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to everyone except caller
    public async Task Broadcast(string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to caller only
    public async Task Echo(string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }
}
`

### Send to Multiple Clients

`csharp
public class ChatHub : Hub
{
    // ✅ Send to list of connections
    public async Task SendToUsers(List<string> connectionIds, string message)
    {
        await Clients.Clients(connectionIds).SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to all except specific connections
    public async Task SendToAllExcept(List<string> excludedConnectionIds, string message)
    {
        await Clients.AllExcept(excludedConnectionIds).SendAsync("ReceiveMessage", message);
    }
}
`

---

## Groups

### Group Management

`csharp
public class ChatHub : Hub
{
    // ✅ Join group
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        
        // Notify room members
        await Clients.Group(roomName).SendAsync(
            "UserJoined", 
            Context.ConnectionId);
    }
    
    // ✅ Leave group
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        
        await Clients.Group(roomName).SendAsync(
            "UserLeft", 
            Context.ConnectionId);
    }
    
    // ✅ Send to group
    public async Task SendToRoom(string roomName, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to group except caller
    public async Task SendToOthersInRoom(string roomName, string message)
    {
        await Clients.OtherGroupMembers(roomName).SendAsync("ReceiveMessage", message);
    }
}
`

### Use Cases for Groups

- **Chat rooms**: Each room is a group
- **User notifications**: Group per user ID
- **Live dashboards**: Group per dashboard
- **Multiplayer games**: Group per game session

---

## Connection Lifecycle

### Connection Events

`csharp
public class ChatHub : Hub
{
    // ✅ Called when client connects
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine(Client connected: {Context.ConnectionId});
        
        // Send welcome message
        await Clients.Caller.SendAsync("Welcome", "Hello!");
        
        // Notify others
        await Clients.Others.SendAsync("UserConnected", Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }
    
    // ✅ Called when client disconnects
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine(Client disconnected: {Context.ConnectionId});
        
        // Notify others
        await Clients.Others.SendAsync("UserDisconnected", Context.ConnectionId);
        
        // Remove from any groups (automatic, but can do cleanup here)
        
        await base.OnDisconnectedAsync(exception);
    }
}
`

### Getting Connection Info

`csharp
public class ChatHub : Hub
{
    public Task<string> GetConnectionId()
    {
        // ✅ Current connection ID
        return Task.FromResult(Context.ConnectionId);
    }
    
    public Task<string?> GetUserId()
    {
        // ✅ User ID (if authenticated)
        return Task.FromResult(Context.User?.Identity?.Name);
    }
    
    public void LogConnectionInfo()
    {
        Console.WriteLine(Connection ID: {Context.ConnectionId});
        Console.WriteLine(User: {Context.User?.Identity?.Name});
        Console.WriteLine(User Agent: {Context.GetHttpContext()?.Request.Headers.UserAgent});
    }
}
`

---

## Strongly-Typed Hubs

### Type-Safe Client Methods

`csharp
// ✅ Define client interface
public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
    Task UserJoined(string connectionId);
    Task UserLeft(string connectionId);
}

// ✅ Use strongly-typed hub
public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string user, string message)
    {
        // ✅ IntelliSense and compile-time checking
        await Clients.All.ReceiveMessage(user, message);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.UserJoined(Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
`

---

## Authentication and Authorization

### Require Authentication

`csharp
// ✅ Require authentication for entire hub
[Authorize]
public class SecureChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        await Clients.All.SendAsync("ReceiveMessage", userName, message);
    }
}

// ✅ Authorize specific methods
public class ChatHub : Hub
{
    [Authorize(Roles = "Admin")]
    public async Task BroadcastAnnouncement(string message)
    {
        await Clients.All.SendAsync("Announcement", message);
    }
}
`

### User-Specific Groups

`csharp
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // ✅ Add to group based on user ID
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        
        await base.OnConnectedAsync();
    }
    
    // ✅ Send to specific user (all their connections)
    public async Task SendToUser(string userId, string message)
    {
        await Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", message);
    }
}
`

---

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

## Configuration

### SignalR Options

`csharp
builder.Services.AddSignalR(options =>
{
    // ✅ Enable detailed errors (development only)
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    
    // ✅ Keep-alive interval
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    
    // ✅ Client timeout
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    
    // ✅ Maximum message size
    options.MaximumReceiveMessageSize = 32 * 1024;  // 32 KB
    
    // ✅ Handshake timeout
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
`

### Scaling with Redis

`csharp
// ✅ Scale across multiple servers with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp";
    });
`

---

## Best Practices

### ✅ Hub Design
- Keep hub methods simple and fast
- Don't do heavy work in hub methods
- Use background services for long-running tasks
- Return Task for all hub methods
- Use strongly-typed hubs when possible

### ✅ Groups
- Clean up groups on disconnect
- Use meaningful group names
- Consider group size (thousands per group is OK)
- Don't store state in hub (use database or cache)

### ✅ Security
- Always authenticate sensitive operations
- Validate all inputs
- Rate limit hub method invocations
- Don't trust client data

### ✅ Performance
- Use Redis for multi-server scenarios
- Limit message size
- Batch messages when possible
- Monitor connection count

---

## Common Pitfalls

### ❌ Storing State in Hub

`csharp
// ❌ BAD: Hub instance is transient
public class ChatHub : Hub
{
    private List<string> _messages = new();  // ❌ Lost on each invocation
    
    public Task AddMessage(string message)
    {
        _messages.Add(message);  // ❌ Won't persist
        return Task.CompletedTask;
    }
}

// ✅ GOOD: Use external storage
public class ChatHub : Hub
{
    private readonly IChatRepository _repository;
    
    public ChatHub(IChatRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddMessage(string message)
    {
        await _repository.AddMessageAsync(message);  // ✅ Persisted
    }
}
`

### ❌ Not Handling Disconnections

`csharp
// ❌ BAD: Not cleaning up
public class GameHub : Hub
{
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        // ❌ What if user disconnects?
    }
}

// ✅ GOOD: Clean up on disconnect
public class GameHub : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // ✅ Clean up game state
        await RemovePlayerFromAllGames(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
`

### ❌ Blocking Hub Methods

`csharp
// ❌ BAD: Blocking call
public Task SendMessage(string message)
{
    Thread.Sleep(5000);  // ❌ Blocks thread!
    return Clients.All.SendAsync("ReceiveMessage", message);
}

// ✅ GOOD: Async all the way
public async Task SendMessage(string message)
{
    await Task.Delay(5000);  // ✅ Async wait
    await Clients.All.SendAsync("ReceiveMessage", message);
}
`

---

## Related Files

- [RealTime/SignalRBasics.cs](../RealTime/SignalRBasics.cs)

---

## See Also

- [Web API and MVC](Web-API-MVC.md) - SignalR in web applications
- [Security](Security.md) - Authentication and authorization
- [Testing](Testing.md) - Testing SignalR hubs
- [Async Programming](Async-Multithreading.md) - Async patterns
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [gRPC.md](gRPC.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers RealTime and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know RealTime and I would just follow best practices."
- Strong answer: "For RealTime, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply RealTime in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
