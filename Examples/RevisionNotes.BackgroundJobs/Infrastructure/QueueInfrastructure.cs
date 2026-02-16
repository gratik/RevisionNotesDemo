using System.Collections.Concurrent;
using System.Threading.Channels;
using RevisionNotes.BackgroundJobs.Jobs;

namespace RevisionNotes.BackgroundJobs.Infrastructure;

public interface IBackgroundJobQueue
{
    ValueTask EnqueueAsync(BackgroundJob job, CancellationToken cancellationToken);
    ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken);
    int Depth { get; }
}

public sealed class InMemoryBackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<BackgroundJob> _channel = Channel.CreateUnbounded<BackgroundJob>(
        new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });
    private int _depth;

    public int Depth => Volatile.Read(ref _depth);

    public async ValueTask EnqueueAsync(BackgroundJob job, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(job, cancellationToken);
        Interlocked.Increment(ref _depth);
    }

    public async ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken)
    {
        var job = await _channel.Reader.ReadAsync(cancellationToken);
        Interlocked.Decrement(ref _depth);
        return job;
    }
}

public interface IProcessedJobStore
{
    bool TryStart(string jobId);
    void MarkCompleted(string jobId);
}

public sealed class InMemoryProcessedJobStore : IProcessedJobStore
{
    private readonly ConcurrentDictionary<string, byte> _started = new();
    private readonly ConcurrentDictionary<string, byte> _completed = new();

    public bool TryStart(string jobId)
    {
        if (_completed.ContainsKey(jobId))
        {
            return false;
        }

        return _started.TryAdd(jobId, 0);
    }

    public void MarkCompleted(string jobId)
    {
        _completed[jobId] = 0;
        _started.TryRemove(jobId, out _);
    }
}

public sealed class JobProcessingState
{
    private int _processedCount;
    private int _failureCount;

    public int ProcessedCount => Volatile.Read(ref _processedCount);
    public int FailureCount => Volatile.Read(ref _failureCount);

    public void RecordSuccess() => Interlocked.Increment(ref _processedCount);
    public void RecordFailure() => Interlocked.Increment(ref _failureCount);
}
