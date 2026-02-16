using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.NativeAot.Api.Models;

namespace RevisionNotes.NativeAot.Api.Infrastructure;

public interface IProductStore
{
    IReadOnlyList<ProductResponse> GetAll();
    ProductResponse Add(string name, decimal price);
}

public sealed class InMemoryProductStore : IProductStore
{
    private readonly ConcurrentDictionary<int, ProductResponse> _products = new();
    private int _id = 2;

    public InMemoryProductStore()
    {
        _products[1] = new ProductResponse(1, "AOT Notebook", 29.0m, DateTimeOffset.UtcNow);
        _products[2] = new ProductResponse(2, "AOT Keyboard", 79.0m, DateTimeOffset.UtcNow);
    }

    public IReadOnlyList<ProductResponse> GetAll() =>
        _products.Values.OrderBy(x => x.Id).ToList();

    public ProductResponse Add(string name, decimal price)
    {
        var id = Interlocked.Increment(ref _id);
        var item = new ProductResponse(id, name.Trim(), price, DateTimeOffset.UtcNow);
        _products[id] = item;
        return item;
    }
}

public sealed class StoreHealthCheck(
    IProductStore store) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy($"ProductCount={store.GetAll().Count}"));
}
