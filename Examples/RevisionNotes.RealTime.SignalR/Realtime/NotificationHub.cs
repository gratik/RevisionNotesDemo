using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RevisionNotes.RealTime.SignalR.Infrastructure;

namespace RevisionNotes.RealTime.SignalR.Realtime;

[Authorize(Policy = "realtime.user")]
public sealed class NotificationHub(
    IMessageHistoryStore historyStore,
    ILogger<NotificationHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("Client connected {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Client disconnected {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public Task JoinGroup(string groupName) =>
        Groups.AddToGroupAsync(Context.ConnectionId, NormalizeGroup(groupName));

    public async Task SendGroupMessage(string groupName, string message)
    {
        var normalizedGroup = NormalizeGroup(groupName);
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new HubException("Message cannot be empty.");
        }

        var sender = Context.User?.Identity?.Name ?? "anonymous";
        var item = new ChatMessage(
            Sender: sender,
            Group: normalizedGroup,
            Message: message.Trim(),
            SentAtUtc: DateTimeOffset.UtcNow);

        await historyStore.AppendAsync(item, Context.ConnectionAborted);
        await Clients.Group(normalizedGroup).SendAsync("message.received", item, Context.ConnectionAborted);
    }

    private static string NormalizeGroup(string groupName) =>
        string.IsNullOrWhiteSpace(groupName) ? "general" : groupName.Trim().ToLowerInvariant();
}
