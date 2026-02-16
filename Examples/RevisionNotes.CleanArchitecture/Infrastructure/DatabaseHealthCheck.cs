using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RevisionNotes.CleanArchitecture.Infrastructure;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Simulate a database dependency that can be replaced with a real check later.
        return Task.FromResult(HealthCheckResult.Healthy("In-memory persistence is available."));
    }
}
