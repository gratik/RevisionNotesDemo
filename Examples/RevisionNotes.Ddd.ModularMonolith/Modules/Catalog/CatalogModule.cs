using System.Collections.Concurrent;
using RevisionNotes.Ddd.ModularMonolith.Modules.Shared;

namespace RevisionNotes.Ddd.ModularMonolith.Modules.Catalog;

public sealed record CatalogItem(Guid Id, string Name, decimal Price, DateTimeOffset CreatedAtUtc);

public sealed record CatalogItemCreatedEvent(Guid CatalogItemId, string ItemName, decimal Price, DateTimeOffset OccurredAtUtc) : IModularEvent
{
    public string Name => "CatalogItemCreated";
}

public interface ICatalogRepository
{
    Task<IReadOnlyList<CatalogItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<CatalogItem> AddAsync(string name, decimal price, CancellationToken cancellationToken);
}

public sealed class InMemoryCatalogRepository : ICatalogRepository
{
    private readonly ConcurrentDictionary<Guid, CatalogItem> _items = new();

    public Task<IReadOnlyList<CatalogItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        var values = _items.Values
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<CatalogItem>>(values);
    }

    public Task<CatalogItem> AddAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        var item = new CatalogItem(Guid.NewGuid(), name, price, DateTimeOffset.UtcNow);
        _items[item.Id] = item;
        return Task.FromResult(item);
    }
}

public sealed class CatalogService(
    ICatalogRepository repository,
    IModularEventBus eventBus)
{
    public Task<IReadOnlyList<CatalogItem>> GetItemsAsync(CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);

    public async Task<CatalogItem> CreateItemAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        var item = await repository.AddAsync(name, price, cancellationToken);
        await eventBus.PublishAsync(
            new CatalogItemCreatedEvent(item.Id, item.Name, item.Price, DateTimeOffset.UtcNow),
            cancellationToken);
        return item;
    }
}
