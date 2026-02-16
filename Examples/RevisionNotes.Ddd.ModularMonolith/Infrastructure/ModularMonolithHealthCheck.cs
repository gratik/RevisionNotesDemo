using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.Ddd.ModularMonolith.Modules.Catalog;

namespace RevisionNotes.Ddd.ModularMonolith.Infrastructure;

public sealed class ModularMonolithHealthCheck(
    ICatalogRepository catalogRepository) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var items = await catalogRepository.GetAllAsync(cancellationToken);
        return HealthCheckResult.Healthy($"CatalogItems={items.Count}");
    }
}
