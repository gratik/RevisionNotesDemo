using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RevisionNotes.Testing.Pyramid.Infrastructure;

public sealed class StoreHealthCheck(
    IOrderReadStore store) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(store is null
            ? HealthCheckResult.Unhealthy("Order store unavailable.")
            : HealthCheckResult.Healthy("Order store available."));
    }
}
