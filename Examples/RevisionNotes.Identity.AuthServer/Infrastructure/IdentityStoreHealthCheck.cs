using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.Identity.AuthServer.Security;

namespace RevisionNotes.Identity.AuthServer.Infrastructure;

public sealed class IdentityStoreHealthCheck(
    IRefreshTokenStore refreshTokenStore) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(refreshTokenStore is null
            ? HealthCheckResult.Unhealthy("Refresh token store unavailable.")
            : HealthCheckResult.Healthy("Refresh token store is operational."));
    }
}
