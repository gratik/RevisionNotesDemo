# Connection Lifecycle

> Subject: [RealTime](../README.md)

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


