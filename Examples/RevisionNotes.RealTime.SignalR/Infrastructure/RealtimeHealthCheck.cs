using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RevisionNotes.RealTime.SignalR.Infrastructure;

public sealed class RealtimeHealthCheck(
    IMessageHistoryStore historyStore) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var recent = await historyStore.GetRecentAsync(cancellationToken);
        return HealthCheckResult.Healthy($"RecentMessages={recent.Count}");
    }
}
