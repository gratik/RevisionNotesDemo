using Microsoft.EntityFrameworkCore;
using RevisionNotes.DataAccess.AdvancedEfCore.Contracts;

namespace RevisionNotes.DataAccess.AdvancedEfCore.Data;

public enum UpdateResult
{
    Success,
    NotFound,
    Conflict
}

public sealed class ProductRepository(AppDbContext dbContext)
{
    private static readonly Func<AppDbContext, decimal, IAsyncEnumerable<ProductResponse>> CompiledProductsByMinPrice =
        EF.CompileAsyncQuery((AppDbContext db, decimal minPrice) =>
            db.Products
                .AsNoTracking()
                .Where(x => x.Price >= minPrice)
                .OrderByDescending(x => x.UpdatedAtUtc)
                .Select(x => new ProductResponse(
                    x.Id,
                    x.Name,
                    x.Price,
                    x.Version,
                    x.Tags.OrderBy(t => t.Value).Select(t => t.Value).ToArray())));

    public async Task<IReadOnlyList<ProductResponse>> GetProductsAsync(decimal minPrice, CancellationToken cancellationToken)
    {
        var list = new List<ProductResponse>();
        await foreach (var item in CompiledProductsByMinPrice(dbContext, minPrice).WithCancellation(cancellationToken))
        {
            list.Add(item);
        }

        return list;
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Tags)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return product is null ? null : Map(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            Tags = request.Tags
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new ProductTag { Value = x.Trim().ToLowerInvariant() })
                .ToList()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(product);
    }

    public async Task<UpdateResult> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            return UpdateResult.NotFound;
        }

        if (product.Version != request.ExpectedVersion)
        {
            return UpdateResult.Conflict;
        }

        product.Name = request.Name.Trim();
        product.Price = request.Price;
        product.Version += 1;
        product.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        product.IsDeleted = true;
        product.Version += 1;
        product.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductResponse Map(Product product) =>
        new(product.Id, product.Name, product.Price, product.Version, product.Tags.Select(x => x.Value).ToArray());
}
