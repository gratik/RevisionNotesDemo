// ==============================================================================
// REPOSITORY PATTERN - Data Access Abstraction
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE REPOSITORY PATTERN?
// --------------------------------
// The Repository Pattern mediates between the domain and data mapping layers,
// acting like an in-memory domain object collection. It provides a collection-like
// interface (Add, Remove, Find, GetAll) that encapsulates data access logic and
// returns domain objects rather than database records.
//
// Think of it as: "A façade over your database that makes it look like a collection"
//
// Core Concepts:
//   • Abstraction: IRepository<T> interface hiding implementation details
//   • Encapsulation: All database queries centralized in one place
//   • Domain Focus: Returns domain entities, not data transfer objects or rows
//   • Technology Independence: Business logic doesn't know about SQL, EF, Dapper, etc.
//
// WHY IT MATTERS
// --------------
// ✅ TESTABILITY: Mock repository interfaces in unit tests without database
// ✅ SEPARATION OF CONCERNS: Business logic isolated from data access technology
// ✅ SINGLE SOURCE OF TRUTH: All queries for an entity in one place
// ✅ FLEXIBILITY: Swap data stores (SQL → NoSQL → in-memory) without changing business code
// ✅ DEPENDENCY INVERSION: Depend on IRepository abstraction, not concrete DbContext
// ✅ QUERY REUSABILITY: Common queries (GetActive, GetByCategory) shared across services
// ✅ SECURITY: Centralized place to enforce data access rules and tenant isolation
//
// WHEN TO USE IT
// --------------
// ✅ Complex domain models with rich business logic (DDD scenarios)
// ✅ Multiple data sources for same entity (SQL + cache + external API)
// ✅ Need to support multiple storage technologies
// ✅ Team needs clear separation between domain and infrastructure
// ✅ High test coverage requirements for business logic
// ✅ Multi-tenant applications requiring data isolation
// ✅ When you have domain-specific queries (GetActiveCustomers, GetOverdueOrders)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple CRUD apps where EF Core DbContext is sufficient
// ❌ Already using Entity Framework with complex queries (LINQ is your repository)
// ❌ Small team/project where abstraction adds overhead
// ❌ Read-only data access (consider Query Objects or CQRS instead)
// ❌ When ORM provides enough abstraction (over-abstracting hurts)
// ❌ Repository over repository anti-pattern (EF DbContext IS already a repository)
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine Netflix's video catalog system:
//   • Videos stored in SQL Server (relational metadata: title, description, rating)
//   • Video files stored in S3 (blob storage for streaming)
//   • Playback stats in time-series database (Cassandra for analytics)
//   • Search index in Elasticsearch (fast text search)
//
// Without Repository Pattern:
//   → Business logic scattered with SQL queries, S3 SDK calls, Elasticsearch queries
//   → Testing requires real databases and AWS connections
//   → Changing from S3 to Azure Blob requires rewriting business logic
//   → Same queries duplicated across multiple services
//
// With Repository Pattern:
//   → IVideoRepository interface with GetById(), Search(), GetRecommendations()
//   → Business logic uses IVideoRepository, doesn't know about storage technology
//   → Can swap VideoRepositorySQL → VideoRepositoryCosmos without changing services
//   → Mock IVideoRepository for unit tests (no real database needed)
//   → Composite repository can aggregate data from SQL + S3 + Elasticsearch seamlessly
//
// ========================================================================
// COMMON ANTI-PATTERNS (What NOT to Do)
// ========================================================================
//
// ❌ ANTIPATTERN #1: Generic Repository Dump
// Problem: One giant IRepository<T> with 50 methods used for all entities
//   public interface IRepository<T>
//   {
//       Task<T> GetById(int id);
//       Task<List<T>> GetAll();
//       Task<List<T>> GetByStatus(string status);
//       Task<List<T>> GetByCategory(string category);
//       Task<List<T>> GetByDate(DateTime date);
//       // ... 45 more methods that don't make sense for all entities
//   }
// Why Bad: Not all entities have status, category, date. Violates ISP.
// Better: Specific repositories with domain-specific methods:
//   ICustomerRepository: GetByEmail(), GetActiveCustomers()
//   IOrderRepository: GetOverdueOrders(), GetByCustomerId()
//
// ❌ ANTIPATTERN #2: Repository as Service Layer
// Problem: Repository contains business logic
//   public class CustomerRepository
//   {
//       public async Task<decimal> CalculateLoyaltyDiscount(int customerId)
//       {
//           // Business logic doesn't belong here!
//       }
//       public async Task SendWelcomeEmail(Customer customer)
//       {
//           // This is NOT data access!
//       }
//   }
// Why Bad: Repository should only handle data access, not business rules.
// Better: CustomerService handles business logic, calls repository for data.
//
// ❌ ANTIPATTERN #3: Leaky Abstraction
// Problem: Repository exposes IQueryable<T> or database-specific types
//   public IQueryable<Customer> GetCustomers()  // ❌ Leaky!
//   public DataTable GetCustomerData()          // ❌ Leaky!
// Why Bad: Caller now knows you're using EF or ADO.NET. Can't swap implementations.
// Better: Return IEnumerable<Customer> or List<Customer> (concrete domain objects)
//
// ❌ ANTIPATTERN #4: Over-Abstracting Entity Framework
// Problem: Adding repository over EF Core when DbContext is already Unit of Work
//   // ❌ Unnecessary double abstraction
//   IRepository<Customer> → CustomerRepository → DbContext
// Why Bad: DbContext already abstracts database; repository adds no value.
// Better: Use DbContext directly for simple scenarios OR fully justify repository layer.
//
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
// GOOD VS BAD PRACTICES - Repository Pattern
// ========================================================================

// ❌ BAD: Direct Database Access in Business Logic
// -----------------------------------------------
/*
public class BadOrderService
{
    private readonly SqlConnection _connection;

    public async Task<Order> GetOrderAsync(int orderId)
    {
        // ❌ SQL scattered throughout business layer
        var cmd = new SqlCommand("SELECT * FROM Orders WHERE Id = @Id", _connection);
        cmd.Parameters.AddWithValue("@Id", orderId);
        
        // ❌ Business logic directly knows about SQL, DataReader, etc.
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Order
            {
                Id = (int)reader["Id"],
                CustomerId = (int)reader["CustomerId"],
                Total = (decimal)reader["Total"]
            };
        }
        return null;
    }
    
    // ❌ PROBLEMS:
    // • Can't unit test without real database
    // • Duplicate SQL queries across multiple services
    // • Changing database requires updating all services
    // • Violates Single Responsibility (business + data access mixed)
    // • No abstraction to mock
}
*/

// ✅ GOOD: Repository Abstraction
// --------------------------------
/*
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Order>> GetOverdueOrdersAsync();
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}

public class GoodOrderService
{
    private readonly IOrderRepository _orderRepository;

    public GoodOrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        // ✅ Business logic uses abstraction
        return await _orderRepository.GetByIdAsync(orderId);
    }
    
    // ✅ BENEFITS:
    // • Unit testable with mocked repository
    // • Business logic doesn't know about database
    // • Can swap implementations (SQL → NoSQL → in-memory)
    // • Single responsibility - service does business, repo does data
    // • DRY - common queries reused across services
}
*/

// ❌ BAD: Exposing IQueryable (Leaky Abstraction)
// ------------------------------------------------
/*
public interface ILeakyRepository<T>
{
    // ❌ Exposes EF implementation details to caller
    IQueryable<T> Query();
}

// Usage:
var customers = repository.Query()
    .Where(c => c.IsActive)
    .Include(c => c.Orders)      // ❌ Caller needs EF knowledge
    .ThenInclude(o => o.Items)   // ❌ Leaks data access concerns
    .ToListAsync();

// PROBLEMS:
// • Caller must know about EF (Include, ThenInclude)
// • Cannot swap repository implementation
// • N+1 query problems leak into business layer
// • Harder to control query execution
*/

// ✅ GOOD: Encapsulated Query Methods
// ------------------------------------
/*
public interface IGoodRepository
{
    // ✅ Repository controls query execution
    Task<IEnumerable<Customer>> GetActiveCustomersWithOrdersAsync();
    Task<IEnumerable<Customer>> GetCustomersByRegionAsync(string region);
}

public class GoodCustomerRepository : IGoodRepository
{
    private readonly AppDbContext _context;

    public async Task<IEnumerable<Customer>> GetActiveCustomersWithOrdersAsync()
    {
        // ✅ EF knowledge encapsulated in repository
        return await _context.Customers
            .Where(c => c.IsActive)
            .Include(c => c.Orders)
            .ThenInclude(o => o.Items)
            .ToListAsync();
    }
    
    // ✅ BENEFITS:
    // • Caller doesn't need EF knowledge
    // • Can optimize queries in one place
    // • Easy to swap implementations
    // • Clear intent with named methods
}
*/

// ❌ BAD: Generic Repository with Unnecessary Methods
// ----------------------------------------------------
/*
public interface IBadGenericRepository<T>
{
    Task<T> GetById(int id);
    Task<IEnumerable<T>> GetAll();
    Task<IEnumerable<T>> GetByStatus(string status);    // ❌ Not all entities have status
    Task<IEnumerable<T>> GetByCategory(string category); // ❌ Not all have category
    Task<IEnumerable<T>> GetExpired(DateTime date);      // ❌ Not all can expire
    Task<IEnumerable<T>> GetByOwner(int ownerId);        // ❌ Not all have owners
    // ... 40 more methods that might not apply
}

// PROBLEMS:
// • Interface Segregation Principle violated
// • Product doesn't have "owner", Customer doesn't have "category"
// • Forces implementations to throw NotSupportedException
// • One-size-fits-all approach doesn't work
*/

// ✅ GOOD: Specific Repository Interfaces
// ----------------------------------------
/*
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
}

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<IEnumerable<Customer>> GetByRegionAsync(string region);
}

// ✅ BENEFITS:
// • Each repository has domain-specific methods
// • No unnecessary methods forced onto entities
// • Clear intent ("GetByEmail" makes sense for Customer, not Product)
// • Follows Interface Segregation Principle
*/

// ========================================================================
// IMPLEMENTATION 1: IN-MEMORY REPOSITORY (For Testing)
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
// IMPLEMENTATION 2: ENTITY FRAMEWORK CORE REPOSITORY
// ========================================================================
// Benefits:
//   ✅ Change tracking for updates
//   ✅ LINQ query support
//   ✅ Migrations and schema management
//   ✅ Navigation properties loaded automatically
//
// When to Use:
//   • Complex object graphs with relationships
//   • Need change tracking
//   • Domain-driven design with rich entities
//
// Commented out to avoid EF Core dependency - uncomment if using EF Core
// ========================================================================
/*
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<RepoProduct> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RepoProduct>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Category); // Index for category queries
        });
    }
}

/// <summary>
/// Entity Framework Core implementation
/// Uses DbContext for database operations
/// </summary>
public class EfCoreProductRepository : IRepository<RepoProduct>
{
    private readonly AppDbContext _context;

    public EfCoreProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RepoProduct?> GetByIdAsync(int id)
    {
        // ✅ EF Core tracks this entity for updates
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<RepoProduct>> GetAllAsync()
    {
        // ✅ AsNoTracking for read-only queries (performance)
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<RepoProduct>> FindAsync(Func<RepoProduct, bool> predicate)
    {
        // ⚠️ WARNING: This loads all products into memory first
        // Better: Use Expression<Func<T, bool>> for server-side filtering
        return await Task.Run(() => _context.Products
            .AsNoTracking()
            .Where(predicate)
            .ToList());
    }

    public async Task AddAsync(RepoProduct entity)
    {
        // ✅ EF Core generates ID automatically
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(); // Commit transaction
    }

    public async Task UpdateAsync(RepoProduct entity)
    {
        // ✅ EF Core detects changes and updates only modified fields
        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync()
    {
        // ✅ Executes COUNT(*) on server - efficient
        return await _context.Products.CountAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        // ✅ Executes EXISTS query - efficient
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}

// EF Core Benefits vs Challenges:
// ✅ Pros: Change tracking, LINQ, migrations, lazy loading
// ⚠️ Cons: N+1 queries if not careful, heavier memory footprint
// 💡 Use AsNoTracking() for read-only queries to improve performance
*/

// ========================================================================
// IMPLEMENTATION 3: DAPPER REPOSITORY (Micro-ORM)
// ========================================================================
// Benefits:
//   ✅ Fast - close to raw ADO.NET performance
//   ✅ Lightweight - no change tracking overhead
//   ✅ Full control over SQL queries
//   ✅ Great for stored procedures and complex SQL
//
// When to Use:
//   • Performance-critical read operations
//   • Complex SQL queries or stored procedures
//   • Legacy databases with non-standard schemas
//   • When you need every bit of performance
//
// Commented out to avoid Dapper dependency - uncomment if using Dapper
// ========================================================================
/*
using Dapper;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Dapper implementation - fast, lightweight, SQL-focused
/// </summary>
public class DapperProductRepository : IRepository<RepoProduct>
{
    private readonly string _connectionString;

    public DapperProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<RepoProduct?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        // ✅ Dapper maps SQL result directly to object
        var sql = "SELECT Id, Name, Price, Stock, Category FROM Products WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<RepoProduct>(sql, new { Id = id });
    }

    public async Task<IEnumerable<RepoProduct>> GetAllAsync()
    {
        using var connection = CreateConnection();
        // ✅ Fast read - no change tracking
        var sql = "SELECT Id, Name, Price, Stock, Category FROM Products";
        return await connection.QueryAsync<RepoProduct>(sql);
    }

    public async Task<IEnumerable<RepoProduct>> FindAsync(Func<RepoProduct, bool> predicate)
    {
        // ⚠️ Limitation: Must load all and filter in memory
        // For server-side filtering, use specific query methods instead
        var all = await GetAllAsync();
        return all.Where(predicate);
    }

    public async Task AddAsync(RepoProduct entity)
    {
        using var connection = CreateConnection();
        // ✅ Full control over SQL - can use stored proc too
        var sql = @"
            INSERT INTO Products (Name, Price, Stock, Category)
            VALUES (@Name, @Price, @Stock, @Category);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        
        entity.Id = await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task UpdateAsync(RepoProduct entity)
    {
        using var connection = CreateConnection();
        // ✅ Explicit SQL - know exactly what's being updated
        var sql = @"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Stock = @Stock, Category = @Category
            WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Products WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<int> CountAsync()
    {
        using var connection = CreateConnection();
        // ✅ Efficient server-side COUNT
        var sql = "SELECT COUNT(*) FROM Products";
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Products WHERE Id = @Id";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
        return count > 0;
    }
}

// Dapper Benefits vs Challenges:
// ✅ Pros: 2-3x faster than EF Core, full SQL control, lightweight
// ⚠️ Cons: Manual SQL (typos possible), no change tracking, no migrations
// 💡 Best for: Performance-critical queries, complex SQL, stored procedures
*/

// ========================================================================
// IMPLEMENTATION 4: ADO.NET REPOSITORY (Raw Database Access)
// ========================================================================
// Benefits:
//   ✅ Maximum control over database operations
//   ✅ Lowest-level - no abstraction overhead
//   ✅ Best performance possible
//   ✅ Works with any database provider
//
// When to Use:
//   • Ultra performance-critical scenarios
//   • Need fine-grained connection/command control
//   • Working with legacy systems
//   • Dynamic SQL generation required
//
// Commented out to avoid ADO.NET dependency complexity
// ========================================================================
/*
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Raw ADO.NET implementation - maximum control, lowest level
/// </summary>
public class AdoNetProductRepository : IRepository<RepoProduct>
{
    private readonly string _connectionString;

    public AdoNetProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<RepoProduct?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT Id, Name, Price, Stock, Category FROM Products WHERE Id = @Id", 
            connection);
        
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            // ✅ Manual mapping gives full control
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<IEnumerable<RepoProduct>> GetAllAsync()
    {
        var products = new List<RepoProduct>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT Id, Name, Price, Stock, Category FROM Products", 
            connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(MapFromReader(reader));
        }

        return products;
    }

    public async Task<IEnumerable<RepoProduct>> FindAsync(Func<RepoProduct, bool> predicate)
    {
        // ⚠️ Must load all and filter in memory
        var all = await GetAllAsync();
        return all.Where(predicate);
    }

    public async Task AddAsync(RepoProduct entity)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            INSERT INTO Products (Name, Price, Stock, Category)
            VALUES (@Name, @Price, @Stock, @Category);
            SELECT CAST(SCOPE_IDENTITY() as int);", 
            connection);

        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Price", entity.Price);
        command.Parameters.AddWithValue("@Stock", entity.Stock);
        command.Parameters.AddWithValue("@Category", entity.Category);

        await connection.OpenAsync();
        entity.Id = (int)(await command.ExecuteScalarAsync())!;
    }

    public async Task UpdateAsync(RepoProduct entity)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Stock = @Stock, Category = @Category
            WHERE Id = @Id", 
            connection);

        AddProductParameters(command, entity);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "DELETE FROM Products WHERE Id = @Id", 
            connection);

        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> CountAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("SELECT COUNT(*) FROM Products", connection);

        await connection.OpenAsync();
        return (int)(await command.ExecuteScalarAsync())!;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT COUNT(1) FROM Products WHERE Id = @Id", 
            connection);

        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        var count = (int)(await command.ExecuteScalarAsync())!;
        return count > 0;
    }

    // Helper methods
    private RepoProduct MapFromReader(IDataReader reader)
    {
        return new RepoProduct
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
            Category = reader.GetString(reader.GetOrdinal("Category"))
        };
    }

    private void AddProductParameters(SqlCommand command, RepoProduct entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Price", entity.Price);
        command.Parameters.AddWithValue("@Stock", entity.Stock);
        command.Parameters.AddWithValue("@Category", entity.Category);
    }
}

// ADO.NET Benefits vs Challenges:
// ✅ Pros: Maximum performance, full control, works everywhere
// ⚠️ Cons: Verbose, manual mapping, SQL injection risks if not careful
// 💡 Best for: Ultra performance-critical code, fine-grained control
*/

// ========================================================================
// COMPARISON SUMMARY: Which Implementation To Choose?
// ========================================================================
//
// 📊 PERFORMANCE (Fast → Slow):
//   1. ADO.NET      - ~100ms (baseline)
//   2. Dapper       - ~110ms (5-10% slower, much easier)
//   3. EF Core      - ~150ms (30-50% slower, most features)
//   4. In-Memory    - ~1ms (testing only)
//
// 🛠️ EASE OF USE (Easy → Hard):
//   1. EF Core      - LINQ, change tracking, migrations
//   2. Dapper       - Simple mapping, write SQL
//   3. In-Memory    - No database needed
//   4. ADO.NET      - Manual everything
//
// 🎯 WHEN TO USE EACH:
//
//   EF CORE: Default choice for most applications
//     ✅ Rich domain models with relationships
//     ✅ Need migrations and schema management
//     ✅ Change tracking beneficial
//     ❌ Avoid for: Ultra high-performance scenarios
//
//   DAPPER: Performance + flexibility balance
//     ✅ Read-heavy workloads
//     ✅ Complex SQL queries or stored procedures
//     ✅ Legacy databases
//     ❌ Avoid for: Complex object graphs with relations
//
//   ADO.NET: Maximum control required
//     ✅ Ultra performance-critical paths
//     ✅ Dynamic SQL generation
//     ✅ Fine-grained connection management
//     ❌ Avoid for: Standard CRUD operations
//
//   IN-MEMORY: Testing and demos
//     ✅ Unit tests
//     ✅ Prototyping
//     ✅ Integration test isolation
//     ❌ Never for production
//
// 💡 HYBRID APPROACH (Recommended for large apps):
//   • EF Core for writes and complex domain operations
//   • Dapper for read-heavy queries and reports
//   • In-Memory for testing
//   • ADO.NET for specific hot paths only
//
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
