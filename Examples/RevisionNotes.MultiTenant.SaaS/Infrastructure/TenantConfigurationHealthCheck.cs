using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RevisionNotes.MultiTenant.SaaS.Tenants;

namespace RevisionNotes.MultiTenant.SaaS.Infrastructure;

public sealed class TenantConfigurationHealthCheck(
    IOptions<TenantCatalogOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var tenantCount = options.Value.AllowedTenantIds.Length;
        return tenantCount == 0
            ? Task.FromResult(HealthCheckResult.Unhealthy("No tenants configured."))
            : Task.FromResult(HealthCheckResult.Healthy($"ConfiguredTenants={tenantCount}"));
    }
}
