using System.Collections.Concurrent;

namespace RevisionNotes.MultiTenant.SaaS.Infrastructure;

public sealed record TenantProject(Guid Id, string TenantId, string Name, DateTimeOffset CreatedAtUtc);

public interface ITenantProjectStore
{
    Task<IReadOnlyList<TenantProject>> GetProjectsAsync(string tenantId, CancellationToken cancellationToken);
    Task<TenantProject> AddProjectAsync(string tenantId, string name, CancellationToken cancellationToken);
}

public sealed class InMemoryTenantProjectStore : ITenantProjectStore
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, TenantProject>> _tenantBuckets = new();

    public Task<IReadOnlyList<TenantProject>> GetProjectsAsync(string tenantId, CancellationToken cancellationToken)
    {
        var bucket = _tenantBuckets.GetOrAdd(tenantId, _ => new ConcurrentDictionary<Guid, TenantProject>());
        var items = bucket.Values.OrderByDescending(x => x.CreatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<TenantProject>>(items);
    }

    public Task<TenantProject> AddProjectAsync(string tenantId, string name, CancellationToken cancellationToken)
    {
        var bucket = _tenantBuckets.GetOrAdd(tenantId, _ => new ConcurrentDictionary<Guid, TenantProject>());
        var item = new TenantProject(Guid.NewGuid(), tenantId, name.Trim(), DateTimeOffset.UtcNow);
        bucket[item.Id] = item;
        return Task.FromResult(item);
    }
}
