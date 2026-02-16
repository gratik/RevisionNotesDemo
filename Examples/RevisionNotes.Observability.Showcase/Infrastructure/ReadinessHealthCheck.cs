using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RevisionNotes.Observability.Showcase.Infrastructure;

public sealed class ReadinessHealthCheck(RequestTelemetry telemetry) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var stale = DateTimeOffset.UtcNow - telemetry.LastUpdatedUtc > TimeSpan.FromMinutes(10);
        if (stale)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Telemetry pipeline appears stale"));
        }

        return Task.FromResult(HealthCheckResult.Healthy("Readiness checks passed"));
    }
}