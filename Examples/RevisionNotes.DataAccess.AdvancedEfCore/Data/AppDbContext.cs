using Microsoft.EntityFrameworkCore;

namespace RevisionNotes.DataAccess.AdvancedEfCore.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(140);
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasQueryFilter(x => !x.IsDeleted);
            entity.HasMany(x => x.Tags)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).HasMaxLength(60);
        });
    }
}

public sealed class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Version { get; set; } = 1;
    public bool IsDeleted { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<ProductTag> Tags { get; set; } = [];
}

public sealed class ProductTag
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public required string Value { get; set; }
    public Product? Product { get; set; }
}
