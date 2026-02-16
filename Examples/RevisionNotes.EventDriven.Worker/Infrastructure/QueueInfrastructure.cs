using System.Threading.Channels;
using RevisionNotes.EventDriven.Worker.Domain;

namespace RevisionNotes.EventDriven.Worker.Infrastructure;

public interface IEventQueue
{
    ValueTask PublishAsync(EventEnvelope envelope, CancellationToken cancellationToken);
    ValueTask<EventEnvelope> DequeueAsync(CancellationToken cancellationToken);
    int Depth { get; }
}

public sealed class InMemoryEventQueue : IEventQueue
{
    private readonly Channel<EventEnvelope> _channel = Channel.CreateUnbounded<EventEnvelope>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public int Depth => _depth;
    private int _depth;

    public async ValueTask PublishAsync(EventEnvelope envelope, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(envelope, cancellationToken);
        Interlocked.Increment(ref _depth);
    }

    public async ValueTask<EventEnvelope> DequeueAsync(CancellationToken cancellationToken)
    {
        var envelope = await _channel.Reader.ReadAsync(cancellationToken);
        Interlocked.Decrement(ref _depth);
        return envelope;
    }
}

public interface IIdempotencyStore
{
    bool TryStart(string eventId);
    void MarkCompleted(string eventId);
    bool IsCompleted(string eventId);
}

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly HashSet<string> _inFlight = [];
    private readonly HashSet<string> _completed = [];
    private readonly object _gate = new();

    public bool TryStart(string eventId)
    {
        lock (_gate)
        {
            if (_completed.Contains(eventId) || _inFlight.Contains(eventId))
            {
                return false;
            }

            _inFlight.Add(eventId);
            return true;
        }
    }

    public void MarkCompleted(string eventId)
    {
        lock (_gate)
        {
            _inFlight.Remove(eventId);
            _completed.Add(eventId);
        }
    }

    public bool IsCompleted(string eventId)
    {
        lock (_gate)
        {
            return _completed.Contains(eventId);
        }
    }
}

public sealed class ProcessingState
{
    public DateTimeOffset LastProcessedAtUtc { get; private set; } = DateTimeOffset.MinValue;
    public int ProcessedCount => _processedCount;
    public int FailureCount => _failureCount;

    private int _processedCount;
    private int _failureCount;

    public void RecordSuccess()
    {
        LastProcessedAtUtc = DateTimeOffset.UtcNow;
        Interlocked.Increment(ref _processedCount);
    }

    public void RecordFailure() => Interlocked.Increment(ref _failureCount);
}