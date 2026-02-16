using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RevisionNotes.Resilience.ChaosDemo.Infrastructure;

public sealed class ChaosHealthCheck(
    ChaosState state) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (state.IsCircuitOpen())
        {
            return Task.FromResult(HealthCheckResult.Degraded("Circuit is currently open."));
        }

        return Task.FromResult(HealthCheckResult.Healthy("Circuit closed."));
    }
}
