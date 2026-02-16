using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.DataAccess.AdvancedEfCore.Data;

namespace RevisionNotes.DataAccess.AdvancedEfCore.Infrastructure;

public sealed class DatabaseHealthCheck(
    AppDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? HealthCheckResult.Healthy("Database connection healthy.")
            : HealthCheckResult.Unhealthy("Cannot connect to database.");
    }
}
