using System.Collections.Concurrent;

namespace RevisionNotes.RealTime.SignalR.Infrastructure;

public sealed record ChatMessage(string Sender, string Group, string Message, DateTimeOffset SentAtUtc);

public interface IMessageHistoryStore
{
    Task AppendAsync(ChatMessage message, CancellationToken cancellationToken);
    Task<IReadOnlyList<ChatMessage>> GetRecentAsync(CancellationToken cancellationToken);
}

public sealed class InMemoryMessageHistoryStore : IMessageHistoryStore
{
    private readonly ConcurrentQueue<ChatMessage> _messages = new();
    private const int MaxMessages = 200;

    public Task AppendAsync(ChatMessage message, CancellationToken cancellationToken)
    {
        _messages.Enqueue(message);

        while (_messages.Count > MaxMessages && _messages.TryDequeue(out _))
        {
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChatMessage>> GetRecentAsync(CancellationToken cancellationToken)
    {
        var list = _messages
            .OrderByDescending(x => x.SentAtUtc)
            .Take(50)
            .ToList();
        return Task.FromResult<IReadOnlyList<ChatMessage>>(list);
    }
}
