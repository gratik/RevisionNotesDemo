# Authentication and Authorization

> Subject: [RealTime](../README.md)

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


