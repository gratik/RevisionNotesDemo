using Microsoft.EntityFrameworkCore;
using RevisionNotes.Microservice.CatalogService.Domain;
using RevisionNotes.Microservice.CatalogService.Infrastructure;

namespace RevisionNotes.Microservice.CatalogService.Features.Catalog;

public sealed record CatalogItemResponse(int Id, string Sku, string Name, decimal Price, DateTimeOffset CreatedAtUtc);
public sealed record CreateCatalogItemRequest(string Sku, string Name, decimal Price);
public sealed record LoginRequest(string Username, string Password);

public interface ICatalogRepository
{
    Task<IReadOnlyList<CatalogItemResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<CatalogItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CatalogItemResponse> CreateAsync(CreateCatalogItemRequest request, CancellationToken cancellationToken);
}

public sealed class CatalogRepository(AppDbContext dbContext, IOutboxWriter outboxWriter) : ICatalogRepository
{
    public async Task<IReadOnlyList<CatalogItemResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.CatalogItems
            .OrderBy(x => x.Name)
            .Select(x => new CatalogItemResponse(x.Id, x.Sku, x.Name, x.Price, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<CatalogItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.CatalogItems
            .Where(x => x.Id == id)
            .Select(x => new CatalogItemResponse(x.Id, x.Sku, x.Name, x.Price, x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<CatalogItemResponse> CreateAsync(CreateCatalogItemRequest request, CancellationToken cancellationToken)
    {
        var entity = new CatalogItem
        {
            Sku = request.Sku.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Price = request.Price,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        dbContext.CatalogItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        await outboxWriter.EnqueueAsync(
            "Catalog.ItemCreated",
            new { entity.Id, entity.Sku, entity.Name, entity.Price, entity.CreatedAtUtc },
            cancellationToken);

        return new CatalogItemResponse(entity.Id, entity.Sku, entity.Name, entity.Price, entity.CreatedAtUtc);
    }
}