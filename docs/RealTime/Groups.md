# Groups

> Subject: [RealTime](../README.md)

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


