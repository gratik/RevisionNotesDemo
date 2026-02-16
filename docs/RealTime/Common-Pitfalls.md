# Common Pitfalls

> Subject: [RealTime](../README.md)

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


