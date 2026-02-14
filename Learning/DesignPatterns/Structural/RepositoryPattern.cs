// ==============================================================================
// REPOSITORY PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// DEFINITION:
//   Mediates between the domain and data mapping layers using a collection-like
//   interface for accessing domain objects. Provides an abstraction over data
//   access code.
//
// PURPOSE:
//   Centralize data access logic and provide a cleaner API for the business layer.
//   Separates the business logic from data access concerns.
//
// EXAMPLE:
//   Instead of calling DbContext directly everywhere, use IRepository<Customer>
//
// WHEN TO USE:
//   • Abstracting data access logic
//   • Multiple data sources (SQL, NoSQL, files)
//   • Unit testing (mock repositories)
//   • Centralizing data access rules
//   • When you want to swap data stores
//
// BENEFITS:
//   • Testability (easy to mock)
//   • Centralized data access logic
//   • Minimizes duplicate query logic
//   • Loose coupling between business and data layers
//   • Single Responsibility (Repository handles data access)
//
// CAUTIONS:
//   • Can be overkill for simple CRUD applications
//   • Entity Framework's DbContext is already a repository + unit of work
//   • Avoid "repository over repository" (redundant abstraction)
//   • Don't expose IQueryable (leaks data layer concerns)
//
// MODERN .NET CONSIDERATION:
//   Entity Framework Core DbContext already implements Repository and Unit of Work
//   patterns. Adding another repository layer is optional and should be justified.
//
// BEST PRACTICES:
//   • Keep repository interfaces simple
//   • Return domain models, not data entities
//   • Use async methods for I/O operations
//   • Consider generic repository for common operations
//   • Use specific repositories for complex queries
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// ========================================================================
// GENERIC REPOSITORY INTERFACE
// ========================================================================

/// <summary>
/// Generic repository interface for CRUD operations
/// </summary>
public interface IRepository<T> where T : class
{
    // Query operations
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

    // Command operations
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);

    // Aggregate operations
    Task<int> CountAsync();
    Task<bool> ExistsAsync(int id);
}

// ========================================================================
// ENTITY MODEL
// ========================================================================

public class RepoProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;

    public override string ToString() =>
        $"{Name} (${Price:F2}) - Stock: {Stock} - Category: {Category}";
}

// ========================================================================
// IN-MEMORY REPOSITORY IMPLEMENTATION
// ========================================================================

/// <summary>
/// In-memory implementation for demonstration
/// In production, this would interact with a database (EF Core, Dapper, etc.)
/// </summary>
public class InMemoryProductRepository : IRepository<RepoProduct>
{
    private readonly List<RepoProduct> _products = new();
    private int _nextId = 1;

    public Task<RepoProduct?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<RepoProduct>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<RepoProduct>>(_products.ToList());
    }

    public Task<IEnumerable<RepoProduct>> FindAsync(Func<RepoProduct, bool> predicate)
    {
        var results = _products.Where(predicate).ToList();
        return Task.FromResult<IEnumerable<RepoProduct>>(results);
    }

    public Task AddAsync(RepoProduct entity)
    {
        entity.Id = _nextId++;
        _products.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RepoProduct entity)
    {
        var existing = _products.FirstOrDefault(p => p.Id == entity.Id);
        if (existing != null)
        {
            existing.Name = entity.Name;
            existing.Price = entity.Price;
            existing.Stock = entity.Stock;
            existing.Category = entity.Category;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
        return Task.CompletedTask;
    }

    public Task<int> CountAsync()
    {
        return Task.FromResult(_products.Count);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_products.Any(p => p.Id == id));
    }
}

// ========================================================================
// BUSINESS LAYER - Uses repository abstraction
// ========================================================================

public class RepoProductService
{
    private readonly IRepository<RepoProduct> _repository;

    public RepoProductService(IRepository<RepoProduct> repository)
    {
        _repository = repository;
    }

    public async Task<RepoProduct?> GetProductAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<RepoProduct>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _repository.FindAsync(p => p.Stock < threshold);
    }

    public async Task<IEnumerable<RepoProduct>> GetProductsByCategoryAsync(string category)
    {
        return await _repository.FindAsync(p => p.Category == category);
    }

    public async Task AddProductAsync(RepoProduct product)
    {
        // Business logic validation
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required");

        if (product.Price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        await _repository.AddAsync(product);
    }

    public async Task UpdateStockAsync(int productId, int newStock)
    {
        var product = await _repository.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        product.Stock = newStock;
        await _repository.UpdateAsync(product);
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class RepositoryDemo
{
    public static async Task RunDemoAsync()
    {
        Console.WriteLine("\n=== REPOSITORY PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Create repository and service
        IRepository<RepoProduct> repository = new InMemoryProductRepository();
        var productService = new RepoProductService(repository);

        Console.WriteLine("--- 1. Adding Products ---");
        await productService.AddProductAsync(new RepoProduct
        {
            Name = "Laptop",
            Price = 999.99m,
            Stock = 15,
            Category = "Electronics"
        });
        await productService.AddProductAsync(new RepoProduct
        {
            Name = "Mouse",
            Price = 25.99m,
            Stock = 5,
            Category = "Electronics"
        });
        await productService.AddProductAsync(new RepoProduct
        {
            Name = "Desk",
            Price = 299.99m,
            Stock = 3,
            Category = "Furniture"
        });
        await productService.AddProductAsync(new RepoProduct
        {
            Name = "Monitor",
            Price = 399.99m,
            Stock = 8,
            Category = "Electronics"
        });

        var count = await repository.CountAsync();
        Console.WriteLine($"[REPO] ✅ Added {count} products\n");

        // 2. Query operations
        Console.WriteLine("--- 2. Query Operations ---");
        var allProducts = await repository.GetAllAsync();
        Console.WriteLine("[REPO] All products:");
        foreach (var p in allProducts)
            Console.WriteLine($"  [{p.Id}] {p}");
        Console.WriteLine();

        // 3. Search by category
        Console.WriteLine("--- 3. Find by Category ---");
        var electronics = await productService.GetProductsByCategoryAsync("Electronics");
        Console.WriteLine("[REPO] Electronics:");
        foreach (var p in electronics)
            Console.WriteLine($"  [{p.Id}] {p}");
        Console.WriteLine();

        // 4. Find low stock items
        Console.WriteLine("--- 4. Find Low Stock (< 10) ---");
        var lowStock = await productService.GetLowStockProductsAsync(10);
        Console.WriteLine("[REPO] Low stock items:");
        foreach (var p in lowStock)
            Console.WriteLine($"  [{p.Id}] {p} ⚠️");
        Console.WriteLine();

        // 5. Update operation
        Console.WriteLine("--- 5. Update Stock ---");
        var mouseId = 2;
        Console.WriteLine($"[REPO] Restocking Mouse (ID {mouseId})...");
        await productService.UpdateStockAsync(mouseId, 50);
        var updatedMouse = await repository.GetByIdAsync(mouseId);
        Console.WriteLine($"[REPO] ✅ Updated: {updatedMouse}\n");

        // 6. Delete operation
        Console.WriteLine("--- 6. Delete Product ---");
        var deskId = 3;
        Console.WriteLine($"[REPO] Deleting Desk (ID {deskId})...");
        await repository.DeleteAsync(deskId);
        var deskExists = await repository.ExistsAsync(deskId);
        Console.WriteLine($"[REPO] Desk exists: {deskExists} ✅ Deleted\n");

        Console.WriteLine("💡 Repository Pattern Benefits:");
        Console.WriteLine("   ✅ Abstracts data access - business logic doesn't know about DB");
        Console.WriteLine("   ✅ Testable - can mock IRepository<T> for unit tests");
        Console.WriteLine("   ✅ Centralized queries - all data access in one place");
        Console.WriteLine("   ✅ Swappable implementations - in-memory, SQL, NoSQL, etc.");
        Console.WriteLine("   ✅ Follows DIP - depend on abstraction, not concrete DB");
    }
}
