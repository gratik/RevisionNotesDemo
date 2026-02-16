# Sending Messages

> Subject: [RealTime](../README.md)

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


