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

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Authentication and Authorization before implementation work begins.
- Keep boundaries explicit so Authentication and Authorization decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Authentication and Authorization in production-facing code.
- When performance, correctness, or maintainability depends on consistent Authentication and Authorization decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Authentication and Authorization as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Authentication and Authorization is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Authentication and Authorization are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

