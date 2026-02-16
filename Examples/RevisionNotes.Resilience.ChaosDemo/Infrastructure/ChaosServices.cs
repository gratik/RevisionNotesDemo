using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RevisionNotes.Resilience.ChaosDemo.Contracts;

namespace RevisionNotes.Resilience.ChaosDemo.Infrastructure;

public sealed class ChaosSettings
{
    public bool Enabled { get; init; } = true;
    public int FailureRatePercent { get; init; } = 35;
    public int MaxDelayMs { get; init; } = 1200;
}

public sealed class ChaosState(IOptions<ChaosSettings> options)
{
    private readonly object _lock = new();

    public bool Enabled { get; private set; } = options.Value.Enabled;
    public int FailureRatePercent { get; private set; } = options.Value.FailureRatePercent;
    public int MaxDelayMs { get; private set; } = options.Value.MaxDelayMs;
    public DateTimeOffset? CircuitOpenUntilUtc { get; private set; }
    public int ConsecutiveFailures { get; private set; }

    public void Update(bool enabled, int failureRatePercent, int maxDelayMs)
    {
        lock (_lock)
        {
            Enabled = enabled;
            FailureRatePercent = Math.Clamp(failureRatePercent, 0, 100);
            MaxDelayMs = Math.Clamp(maxDelayMs, 50, 5000);
        }
    }

    public bool IsCircuitOpen()
    {
        lock (_lock)
        {
            return CircuitOpenUntilUtc.HasValue && CircuitOpenUntilUtc.Value > DateTimeOffset.UtcNow;
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            ConsecutiveFailures += 1;
            if (ConsecutiveFailures >= 3)
            {
                CircuitOpenUntilUtc = DateTimeOffset.UtcNow.AddSeconds(20);
            }
        }
    }

    public void RecordSuccess()
    {
        lock (_lock)
        {
            ConsecutiveFailures = 0;
            CircuitOpenUntilUtc = null;
        }
    }
}

public sealed class UnstableDependencyService(ChaosState state)
{
    public async Task<string> GetValueAsync(CancellationToken cancellationToken)
    {
        var random = Random.Shared;
        await Task.Delay(random.Next(40, state.MaxDelayMs), cancellationToken);

        if (state.Enabled && random.Next(0, 100) < state.FailureRatePercent)
        {
            throw new InvalidOperationException("Injected chaos failure from downstream dependency.");
        }

        return $"dependency-value-{Guid.NewGuid():N}";
    }
}

public sealed class ResilientValueService(
    UnstableDependencyService dependency,
    ChaosState state,
    IMemoryCache cache,
    ILogger<ResilientValueService> logger)
{
    private const string CacheKey = "resilience:last-good-value";

    public async Task<ResilientValueResponse> GetValueAsync(CancellationToken cancellationToken)
    {
        if (state.IsCircuitOpen())
        {
            if (cache.TryGetValue(CacheKey, out string? fallback) && !string.IsNullOrWhiteSpace(fallback))
            {
                return new ResilientValueResponse(fallback, FromCache: true, Attempts: 0, CircuitOpen: true);
            }

            return new ResilientValueResponse("fallback-unavailable", FromCache: false, Attempts: 0, CircuitOpen: true);
        }

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(700));
                var value = await dependency.GetValueAsync(timeoutCts.Token);
                state.RecordSuccess();
                cache.Set(CacheKey, value, TimeSpan.FromSeconds(20));
                return new ResilientValueResponse(value, FromCache: false, Attempts: attempt, CircuitOpen: false);
            }
            catch (Exception ex) when (attempt < 3 && ex is not OperationCanceledException)
            {
                state.RecordFailure();
                logger.LogWarning(ex, "Transient call failure on attempt {Attempt}", attempt);
                await Task.Delay(TimeSpan.FromMilliseconds(80 * attempt), cancellationToken);
            }
            catch (Exception ex)
            {
                state.RecordFailure();
                logger.LogError(ex, "Failed to get resilient value.");
                break;
            }
        }

        if (cache.TryGetValue(CacheKey, out string? cached) && !string.IsNullOrWhiteSpace(cached))
        {
            return new ResilientValueResponse(cached, FromCache: true, Attempts: 3, CircuitOpen: state.IsCircuitOpen());
        }

        return new ResilientValueResponse("fallback-unavailable", FromCache: false, Attempts: 3, CircuitOpen: state.IsCircuitOpen());
    }
}
