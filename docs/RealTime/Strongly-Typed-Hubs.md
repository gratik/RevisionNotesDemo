# Strongly-Typed Hubs

> Subject: [RealTime](../README.md)

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


