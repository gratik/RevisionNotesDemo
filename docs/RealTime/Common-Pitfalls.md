# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: WebSocket/SignalR basics, auth context propagation, and scaling fundamentals.
- Related examples: docs/RealTime/README.md
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


## Interview Answer Block
30-second answer:
- Common Pitfalls is about stateful real-time communication patterns. It matters because real-time paths amplify scale and connection-lifecycle concerns.
- Use it when broadcasting live updates to connected clients safely.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: low-latency delivery vs connection/session management overhead.
- Close with one failure mode and mitigation: assuming connection permanence and ignoring reconnection flows.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.