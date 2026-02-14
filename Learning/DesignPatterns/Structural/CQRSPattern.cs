// ============================================================================
// CQRS PATTERN (Command Query Responsibility Segregation)
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
// DEFINITION:
//   Separate read and write operations into different models. Commands (writes)
//   and Queries (reads) use different models optimized for their specific purpose.
//
// PURPOSE:
//   "Separating read (query) and write (command) logic."
//   Different models for updating information (commands) and reading information (queries).
//
// KEY INSIGHT FROM NOTES:
//   "Scaling queries is different to scaling writes"
//   Read and write workloads often have very different characteristics and requirements.
//
// CORE CONCEPTS:
//   
//   COMMAND (Write) SIDE:
//     • Handles: Create, Update, Delete operations
//     • Focus: Business logic, validation, domain events
//     • Model: Rich domain model with behavior
//     • Optimized for: Write operations
//   
//   QUERY (Read) SIDE:
//     • Handles: Read operations
//     • Focus: Fast retrieval, denormalized data
//     • Model: Simple DTOs, view models
//     • Optimized for: Read performance
//
// WHEN TO USE:
//   • Complex business logic that differs between reads and writes
//   • Different scalability requirements (reads vs writes)
//   • Event sourcing architectures
//   • Microservices with separate read/write databases
//   • High-performance systems with different read/write patterns
//   • When reads greatly outnumber writes (or vice versa)
//
// BENEFITS:
//   • Independent scaling: Scale reads and writes separately
//   • Optimized models: Each side optimized for its purpose
//   • Better performance: Denormalized read models, rich write models
//   • Clearer separation: Read and write concerns separated
//   • Flexibility: Can use different databases for each side
//
// EXAMPLE SCENARIO:
//   E-commerce platform:
//     • WRITE: Complex order processing, inventory checks, payment validation
//     • READ: Fast product searches, order history display
//     • Solution: Rich domain objects for writes, denormalized views for reads
//
// COMMON IMPLEMENTATIONS:
//   • MediatR library (.NET) - Command/Query handlers
//   • Separate databases (SQL for writes, NoSQL for reads)
//   • Event Sourcing with read projections
//   • Kafka/RabbitMQ for async updates
//
// CAUTIONS:
//   • Increases complexity significantly
//   • Eventual consistency: Read model may lag behind write model
//   • Not suitable for simple CRUD applications
//   • Requires more infrastructure (message queues, sync mechanisms)
//   • Overkill for most applications
//
// EVENTUAL CONSISTENCY:
//   Write happens → Event published → Read model updated
//   There's a delay between write and read model update.
//
// WHEN NOT TO USE:
//   • Simple CRUD applications
//   • When read and write models are similar
//   • Small applications
//   • When strong consistency is required
//   • You don't have different scalability needs
//
// BEST PRACTICES:
//   • Start simple - don't use CQRS unless needed
//   • Use MediatR or similar library for structure
//   • Design commands and queries as separate objects
//   • Keep commands small and focused
//   • Use DTOs for query results
//   • Handle eventual consistency in UI
//   • Monitor sync lag between write and read models
//
// REAL-WORLD EXAMPLES:
//   • Social media: Complex post creation (write), fast feed reads
//   • Banking: Strict transaction processing (write), fast balance queries (read)
//   • E-commerce: Order processing (write), product catalog (read)
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Domain model
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

// ============================================================================
// COMMAND SIDE (Writes) - Optimized for data modification
// ============================================================================

// Commands - represent intentions to change state
public record CreateProductCommand(string Name, decimal Price, int Stock, string Category);
public record UpdateProductPriceCommand(int ProductId, decimal NewPrice);
public record UpdateProductStockCommand(int ProductId, int Quantity);

// Command handlers - execute commands and modify state
public class ProductCommandHandler
{
    private readonly List<Product> _writeStore; // Could be database

    public ProductCommandHandler(List<Product> writeStore)
    {
        _writeStore = writeStore;
    }

    public int Handle(CreateProductCommand command)
    {
        Console.WriteLine($"[CQRS] COMMAND: Creating product '{command.Name}'");

        var product = new Product
        {
            Id = _writeStore.Any() ? _writeStore.Max(p => p.Id) + 1 : 1,
            Name = command.Name,
            Price = command.Price,
            Stock = command.Stock,
            Category = command.Category
        };

        _writeStore.Add(product);
        Console.WriteLine($"[CQRS] Product created with ID: {product.Id}");

        // In real CQRS, this might publish an event to update read models
        return product.Id;
    }

    public void Handle(UpdateProductPriceCommand command)
    {
        Console.WriteLine($"[CQRS] COMMAND: Updating price for product {command.ProductId}");

        var product = _writeStore.FirstOrDefault(p => p.Id == command.ProductId);
        if (product != null)
        {
            product.Price = command.NewPrice;
            Console.WriteLine($"[CQRS] Price updated to ${command.NewPrice:F2}");
        }
    }

    public void Handle(UpdateProductStockCommand command)
    {
        Console.WriteLine($"[CQRS] COMMAND: Updating stock for product {command.ProductId}");

        var product = _writeStore.FirstOrDefault(p => p.Id == command.ProductId);
        if (product != null)
        {
            product.Stock += command.Quantity;
            Console.WriteLine($"[CQRS] Stock updated. New stock: {product.Stock}");
        }
    }
}

// ============================================================================
// QUERY SIDE (Reads) - Optimized for data retrieval
// ============================================================================

// Read models - optimized for specific queries
public class ProductListItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string FormattedPrice => $"${Price:F2}";
}

public class ProductDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsInStock => Stock > 0;
    public string StockStatus => Stock > 10 ? "In Stock" : Stock > 0 ? "Low Stock" : "Out of Stock";
}

public class CategorySummaryDTO
{
    public string Category { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal AveragePrice { get; set; }
    public int TotalStock { get; set; }
}

// Queries - represent requests for data
public record GetAllProductsQuery();
public record GetProductByIdQuery(int ProductId);
public record GetProductsByCategoryQuery(string Category);
public record GetCategorySummariesQuery();

// Query handlers - optimized for reading
public class ProductQueryHandler
{
    private readonly List<Product> _readStore; // Could be optimized read database/cache

    public ProductQueryHandler(List<Product> readStore)
    {
        _readStore = readStore;
    }

    public List<ProductListItemDTO> Handle(GetAllProductsQuery query)
    {
        Console.WriteLine("[CQRS] QUERY: Getting all products");

        return _readStore.Select(p => new ProductListItemDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        }).ToList();
    }

    public ProductDetailDTO? Handle(GetProductByIdQuery query)
    {
        Console.WriteLine($"[CQRS] QUERY: Getting product details for ID {query.ProductId}");

        var product = _readStore.FirstOrDefault(p => p.Id == query.ProductId);
        if (product == null) return null;

        return new ProductDetailDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category
        };
    }

    public List<ProductListItemDTO> Handle(GetProductsByCategoryQuery query)
    {
        Console.WriteLine($"[CQRS] QUERY: Getting products in category '{query.Category}'");

        return _readStore
            .Where(p => p.Category == query.Category)
            .Select(p => new ProductListItemDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            }).ToList();
    }

    public List<CategorySummaryDTO> Handle(GetCategorySummariesQuery query)
    {
        Console.WriteLine("[CQRS] QUERY: Getting category summaries");

        return _readStore
            .GroupBy(p => p.Category)
            .Select(g => new CategorySummaryDTO
            {
                Category = g.Key,
                ProductCount = g.Count(),
                AveragePrice = g.Average(p => p.Price),
                TotalStock = g.Sum(p => p.Stock)
            }).ToList();
    }
}

// Service coordinating commands and queries
public class ProductService
{
    private readonly ProductCommandHandler _commandHandler;
    private readonly ProductQueryHandler _queryHandler;

    public ProductService(ProductCommandHandler commandHandler, ProductQueryHandler queryHandler)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
    }

    public int CreateProduct(CreateProductCommand command) => _commandHandler.Handle(command);
    public void UpdatePrice(UpdateProductPriceCommand command) => _commandHandler.Handle(command);
    public void UpdateStock(UpdateProductStockCommand command) => _commandHandler.Handle(command);

    public List<ProductListItemDTO> GetAllProducts() => _queryHandler.Handle(new GetAllProductsQuery());
    public ProductDetailDTO? GetProductDetails(int id) => _queryHandler.Handle(new GetProductByIdQuery(id));
    public List<ProductListItemDTO> GetProductsByCategory(string category) => _queryHandler.Handle(new GetProductsByCategoryQuery(category));
    public List<CategorySummaryDTO> GetCategorySummaries() => _queryHandler.Handle(new GetCategorySummariesQuery());
}

// Usage demonstration
public class CQRSDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== CQRS PATTERN DEMO ===\n");

        // Shared data store (in real CQRS, these might be separate)
        var dataStore = new List<Product>();

        var commandHandler = new ProductCommandHandler(dataStore);
        var queryHandler = new ProductQueryHandler(dataStore);
        var productService = new ProductService(commandHandler, queryHandler);

        Console.WriteLine("--- Command Side: Creating Products ---\n");

        productService.CreateProduct(new CreateProductCommand("Laptop", 999.99m, 10, "Electronics"));
        productService.CreateProduct(new CreateProductCommand("Mouse", 29.99m, 50, "Electronics"));
        productService.CreateProduct(new CreateProductCommand("Desk", 299.99m, 5, "Furniture"));
        productService.CreateProduct(new CreateProductCommand("Chair", 199.99m, 8, "Furniture"));

        Console.WriteLine("\n--- Query Side: Reading Data ---\n");

        var allProducts = productService.GetAllProducts();
        Console.WriteLine($"\n[CQRS] All Products ({allProducts.Count}):");
        foreach (var p in allProducts)
        {
            Console.WriteLine($"[CQRS]   {p.Id}. {p.Name} - {p.FormattedPrice}");
        }

        Console.WriteLine("\n--- Query Side: Product Details ---\n");
        var productDetails = productService.GetProductDetails(1);
        if (productDetails != null)
        {
            Console.WriteLine($"[CQRS] Product: {productDetails.Name}");
            Console.WriteLine($"[CQRS] Price: ${productDetails.Price:F2}");
            Console.WriteLine($"[CQRS] Stock: {productDetails.Stock} ({productDetails.StockStatus})");
            Console.WriteLine($"[CQRS] Category: {productDetails.Category}");
        }

        Console.WriteLine("\n--- Query Side: Category Summaries ---\n");
        var summaries = productService.GetCategorySummaries();
        foreach (var summary in summaries)
        {
            Console.WriteLine($"[CQRS] Category: {summary.Category}");
            Console.WriteLine($"[CQRS]   Products: {summary.ProductCount}");
            Console.WriteLine($"[CQRS]   Avg Price: ${summary.AveragePrice:F2}");
            Console.WriteLine($"[CQRS]   Total Stock: {summary.TotalStock}\n");
        }

        Console.WriteLine("--- Command Side: Updating Data ---\n");
        productService.UpdatePrice(new UpdateProductPriceCommand(1, 899.99m));
        productService.UpdateStock(new UpdateProductStockCommand(1, -2));

        Console.WriteLine("\n--- Query Side: Verifying Updates ---\n");
        var updatedProduct = productService.GetProductDetails(1);
        if (updatedProduct != null)
        {
            Console.WriteLine($"[CQRS] Updated Price: ${updatedProduct.Price:F2}");
            Console.WriteLine($"[CQRS] Updated Stock: {updatedProduct.Stock}");
        }

        Console.WriteLine("\n💡 Benefit: Separate optimization for reads and writes");
        Console.WriteLine("💡 Benefit: Scales independently (queries vs commands)");
        Console.WriteLine("💡 Benefit: Different models for reading and writing");
        Console.WriteLine("💡 From Revision Notes: 'Scaling queries is different to scaling writes'");
    }
}
