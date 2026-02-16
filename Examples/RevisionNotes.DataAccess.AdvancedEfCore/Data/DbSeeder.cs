namespace RevisionNotes.DataAccess.AdvancedEfCore.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (dbContext.Products.Any())
        {
            return;
        }

        dbContext.Products.AddRange(
            new Product
            {
                Name = "Performance Hoodie",
                Price = 89.90m,
                Tags = [new ProductTag { Value = "apparel" }, new ProductTag { Value = "premium" }]
            },
            new Product
            {
                Name = "Cloud Notebook",
                Price = 19.50m,
                Tags = [new ProductTag { Value = "stationery" }]
            });

        await dbContext.SaveChangesAsync();
    }
}
