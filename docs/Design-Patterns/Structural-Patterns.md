# Structural Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Design-Patterns](../README.md)

## Structural Patterns

**Purpose**: Compose objects to form larger structures

### Repository Pattern

**Purpose**: Mediates between domain and data mapping layers using a collection-like interface

**Problem**: Abstract data access logic and isolate business layer from database technology

**What It Is**: A façade over your database that makes it look like an in-memory collection. It encapsulates queries and data access logic, returning domain entities rather than database records.

**When To Use**:
- ✅ Complex domain models with rich business logic
- ✅ Multiple data sources (SQL + NoSQL + cache)
- ✅ Need to swap storage technologies
- ✅ High test coverage requirements (mockable)
- ✅ Multi-tenant applications

**When NOT To Use**:
- ❌ Simple CRUD apps where EF Core DbContext is sufficient
- ❌ Over-abstracting EF Core (it already implements repository/unit of work)

**Implementations Available**:
1. **In-Memory**: For unit testing
2. **Entity Framework Core**: LINQ queries, change tracking, migrations
3. **Dapper**: High-performance, lightweight micro-ORM
4. **ADO.NET**: Maximum control, lowest-level access

**Code Examples**: See [RepositoryPattern.cs](../../Learning/DesignPatterns/Structural/RepositoryPattern.cs)

```csharp
// ✅ Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// EF Core implementation example
public class ProductRepository : IRepository<Product>
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.AsNoTracking().ToListAsync();
    }
    
    // ... other methods
}

// Business layer uses abstraction
public class ProductService
{
    private readonly IRepository<Product> _repository;
    
    public ProductService(IRepository<Product> repository)
    {
        _repository = repository; // ✅ Depends on interface, not concrete implementation
    }
}
```

**Benefits**:
- ✅ Testability: Mock IRepository for unit tests
- ✅ Flexibility: Swap SQL → NoSQL without changing business logic
- ✅ Separation of Concerns: Business layer independent of data access
- ✅ DRY: Query reusability across services

**Common Anti-Patterns**:
- ❌ Generic repository with 50 methods that don't fit all entities
- ❌ Exposing `IQueryable<T>` (leaky abstraction)
- ❌ Putting business logic in repositories
- ❌ Double abstraction over EF Core unnecessarily

**Performance Comparison**:
- ADO.NET: ~100ms (fastest, most verbose)
- Dapper: ~110ms (5-10% slower, much easier)
- EF Core: ~150ms (30-50% slower, richest features)

💡 **Hybrid Approach**: Use EF Core for writes + complex domains, Dapper for read-heavy queries, In-Memory for tests

### Unit of Work

**Problem**: Manage transactions across multiple repositories

```csharp
// ✅ Unit of work pattern
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Orders = new OrderRepository(context);
    }
    
    public IUserRepository Users { get; }
    public IOrderRepository Orders { get; }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose() => _context.Dispose();
}

// Usage
using var uow = new UnitOfWork(context);
var user = await uow.Users.GetByIdAsync(1);
var order = new Order { UserId = user.Id };
await uow.Orders.AddAsync(order);
await uow.SaveChangesAsync();  // Single transaction
```

### Adapter

**Problem**: Make incompatible interfaces work together

```csharp
// ✅ Adapter pattern
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(decimal amount);
}

// Third-party library with different interface
public class LegacyPaymentGateway
{
    public bool Charge(double amount) { /* ... */ }
}

// Adapter
public class LegacyPaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentGateway _gateway;
    
    public LegacyPaymentAdapter(LegacyPaymentGateway gateway)
    {
        _gateway = gateway;
    }
    
    public Task<PaymentResult> ProcessAsync(decimal amount)
    {
        bool success = _gateway.Charge((double)amount);
        return Task.FromResult(new PaymentResult { Success = success });
    }
}
```

### Decorator

**Problem**: Add behavior to objects dynamically

```csharp
// ✅ Decorator pattern
public interface INotificationService
{
    Task SendAsync(string message);
}

public class EmailNotification : INotificationService
{
    public async Task SendAsync(string message)
    {
        // Send email
    }
}

// Decorator adds logging
public class LoggingNotificationDecorator : INotificationService
{
    private readonly INotificationService _inner;
    private readonly ILogger _logger;
    
    public LoggingNotificationDecorator(INotificationService inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public async Task SendAsync(string message)
    {
        _logger.LogInformation($"Sending notification: {message}");
        await _inner.SendAsync(message);
        _logger.LogInformation("Notification sent");
    }
}

// Decorator adds retry
public class RetryNotificationDecorator : INotificationService
{
    private readonly INotificationService _inner;
    
    public async Task SendAsync(string message)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await _inner.SendAsync(message);
                return;
            }
            catch when (i < 2)
            {
                await Task.Delay(1000);
            }
        }
    }
}

// Usage: Stack decorators
INotificationService service = new EmailNotification();
service = new LoggingNotificationDecorator(service, logger);
service = new RetryNotificationDecorator(service);
```

---



## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

