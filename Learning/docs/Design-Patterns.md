# Design Patterns (Creational, Structural, Behavioral)

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

Design patterns are proven solutions to common software design problems. This guide covers the Gang of Four
patterns plus modern patterns like Repository, Unit of Work, and CQRS. Remember: patterns solve specific
problems - use them when they add value, not for their own sake.

---

## Creational Patterns

**Purpose**: Control object creation mechanisms

### Singleton

**Problem**: Need exactly one instance of a class

```csharp
// ✅ Thread-safe singleton
public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> _instance =
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());
    
    public static ConfigurationManager Instance => _instance.Value;
    
    private ConfigurationManager()
    {
        // Load configuration
    }
    
    public string GetSetting(string key) => /* ... */;
}

// Usage
var config = ConfigurationManager.Instance;
```

**When to Use**: Logging, configuration, caching (but prefer DI)

### Factory Method

**Problem**: Create objects without specifying exact class

```csharp
// ✅ Factory method pattern
public abstract class PaymentProcessor
{
    public abstract IPaymentGateway CreateGateway();
    
    public PaymentResult ProcessPayment(decimal amount)
    {
        var gateway = CreateGateway();
        return gateway.Process(amount);
    }
}

public class StripeProcessor : PaymentProcessor
{
    public override IPaymentGateway CreateGateway() => new StripeGateway();
}

public class PayPalProcessor : PaymentProcessor
{
    public override IPaymentGateway CreateGateway() => new PayPalGateway();
}
```

### Builder

**Problem**: Construct complex objects step by step

```csharp
// ✅ Fluent builder
public class QueryBuilder
{
    private string _select = "*";
    private string _from = "";
    private string _where = "";
    private string _orderBy = "";
    
    public QueryBuilder Select(string columns)
    {
        _select = columns;
        return this;
    }
    
    public QueryBuilder From(string table)
    {
        _from = table;
        return this;
    }
    
    public QueryBuilder Where(string condition)
    {
        _where = condition;
        return this;
    }
    
    public QueryBuilder OrderBy(string column)
    {
        _orderBy = column;
        return this;
    }
    
    public string Build()
    {
        return $"SELECT {_select} FROM {_from} WHERE {_where} ORDER BY {_orderBy}";
    }
}

// Usage
var query = new QueryBuilder()
    .Select("Name, Email")
    .From("Users")
    .Where("IsActive = 1")
    .OrderBy("Name")
    .Build();
```

---

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

**Code Examples**: See [RepositoryPattern.cs](../DesignPatterns/Structural/RepositoryPattern.cs)

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

## Behavioral Patterns

**Purpose**: Manage algorithms and relationships between objects

### Strategy

**Problem**: Select algorithm at runtime

```csharp
// ✅ Strategy pattern
public interface IShippingStrategy
{
    decimal CalculateCost(decimal weight, string destination);
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 2m;
    }
}

public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return weight * 5m;
    }
}

public class ShippingCalculator
{
    private readonly IShippingStrategy _strategy;
    
    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public decimal Calculate(decimal weight, string destination)
    {
        return _strategy.CalculateCost(weight, destination);
    }
}

// Usage
var calculator = new ShippingCalculator(new ExpressShipping());
var cost = calculator.Calculate(10m, "NYC");
```

### Observer

**Problem**: Notify multiple objects about state changes

```csharp
// ✅ Observer pattern (using events)
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    
    private OrderStatus _status;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                OnStatusChanged(new OrderStatusChangedEventArgs { NewStatus = value });
            }
        }
    }
    
    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}

// Observers
public class EmailNotifier
{
    public void Subscribe(Order order)
    {
        order.StatusChanged += OnOrderStatusChanged;
    }
    
    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        // Send email notification
    }
}

public class InventoryUpdater
{
    public void Subscribe(Order order)
    {
        order.StatusChanged += OnOrderStatusChanged;
    }
    
    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        // Update inventory
    }
}
```

### Chain of Responsibility

**Problem**: Pass request along a chain of handlers

```csharp
// ✅ Chain of responsibility
public abstract class ValidationHandler<T>
{
    private ValidationHandler<T>? _next;
    
    public ValidationHandler<T> SetNext(ValidationHandler<T> next)
    {
        _next = next;
        return next;
    }
    
    public virtual ValidationResult Validate(T request)
    {
        var result = ValidateInternal(request);
        if (!result.IsValid)
            return result;
        
        return _next?.Validate(request) ?? ValidationResult.Success();
    }
    
    protected abstract ValidationResult ValidateInternal(T request);
}

public class LengthValidator : ValidationHandler<string>
{
    protected override ValidationResult ValidateInternal(string request)
    {
        if (request.Length < 3)
            return ValidationResult.Failure("Too short");
        return ValidationResult.Success();
    }
}

public class FormatValidator : ValidationHandler<string>
{
    protected override ValidationResult ValidateInternal(string request)
    {
        if (!request.Contains("@"))
            return ValidationResult.Failure("Invalid format");
        return ValidationResult.Success();
    }
}

// Usage
var chain = new LengthValidator();
chain.SetNext(new FormatValidator());

var result = chain.Validate("test@example.com");
```

---

## Modern Patterns

### CQRS (Command Query Responsibility Segregation)

**Problem**: Separate read and write concerns

```csharp
// ✅ CQRS pattern
// Commands (write)
public record CreateUserCommand(string Name, string Email);

public class CreateUserCommandHandler
{
    private readonly IUserRepository _repository;
    
    public async Task<int> HandleAsync(CreateUserCommand command)
    {
        var user = new User { Name = command.Name, Email = command.Email };
        await _repository.AddAsync(user);
        return user.Id;
    }
}

// Queries (read)
public record GetUserQuery(int Id);

public class GetUserQueryHandler
{
    private readonly IUserRepository _repository;
    
    public async Task<UserDto?> HandleAsync(GetUserQuery query)
    {
        var user = await _repository.GetByIdAsync(query.Id);
        return user == null ? null : new UserDto { /* ... */ };
    }
}
```

---

## Best Practices

### ✅ When to Use Patterns
- Use patterns to solve specific problems, not because they exist
- Prefer simple solutions over complex patterns
- Consider maintainability (will others understand this?)
- Don't force patterns where they don't fit

### ✅ Common Patterns in C#/.NET
- **Repository + Unit of Work** for data access
- **Strategy** for interchangeable algorithms
- **Decorator** for adding behavior (middleware)
- **Factory** for object creation
- **Observer** for event handling

---

## Common Pitfalls

### ❌ Pattern Overuse

```csharp
// ❌ BAD: Over-engineered simple logic
public class AdditionStrategyFactory
{
    public ICalculationStrategy CreateStrategy()
    {
        return new AdditionStrategy();
    }
}

// ✅ GOOD: Simple is better
public int Add(int a, int b) => a + b;
```

### ❌ Singleton Abuse

```csharp
// ❌ BAD: Everything is singleton
public class UserService  // Singleton
public class OrderService  // Singleton
public class PaymentService  // Singleton

// ✅ GOOD: Use dependency injection
services.AddScoped<IUserService, UserService>();
services.AddScoped<IOrderService, OrderService>();
```

---

## Related Files

Creational:
- [DesignPatterns/Creational/SingletonPattern.cs](../DesignPatterns/Creational/SingletonPattern.cs)
- [DesignPatterns/Creational/FactoryMethodPattern.cs](../DesignPatterns/Creational/FactoryMethodPattern.cs)
- [DesignPatterns/Creational/AbstractFactoryPattern.cs](../DesignPatterns/Creational/AbstractFactoryPattern.cs)
- [DesignPatterns/Creational/BuilderPattern.cs](../DesignPatterns/Creational/BuilderPattern.cs)
- [DesignPatterns/Creational/PrototypePattern.cs](../DesignPatterns/Creational/PrototypePattern.cs)

Structural:
- [DesignPatterns/Structural/AdapterPattern.cs](../DesignPatterns/Structural/AdapterPattern.cs)
- [DesignPatterns/Structural/BridgePattern.cs](../DesignPatterns/Structural/BridgePattern.cs)
- [DesignPatterns/Structural/CompositePattern.cs](../DesignPatterns/Structural/CompositePattern.cs)
- [DesignPatterns/Structural/CQRSPattern.cs](../DesignPatterns/Structural/CQRSPattern.cs)
- [DesignPatterns/Structural/DecoratorPattern.cs](../DesignPatterns/Structural/DecoratorPattern.cs)
- [DesignPatterns/Structural/FacadePattern.cs](../DesignPatterns/Structural/FacadePattern.cs)
- [DesignPatterns/Structural/FlyweightPattern.cs](../DesignPatterns/Structural/FlyweightPattern.cs)
- [DesignPatterns/Structural/ProxyPattern.cs](../DesignPatterns/Structural/ProxyPattern.cs)
- [DesignPatterns/Structural/RepositoryPattern.cs](../DesignPatterns/Structural/RepositoryPattern.cs)
- [DesignPatterns/Structural/UnitOfWorkPattern.cs](../DesignPatterns/Structural/UnitOfWorkPattern.cs)

Behavioral:
- [DesignPatterns/Behavioral/ChainOfResponsibilityPattern.cs](../DesignPatterns/Behavioral/ChainOfResponsibilityPattern.cs)
- [DesignPatterns/Behavioral/CommandPattern.cs](../DesignPatterns/Behavioral/CommandPattern.cs)
- [DesignPatterns/Behavioral/MediatorPattern.cs](../DesignPatterns/Behavioral/MediatorPattern.cs)
- [DesignPatterns/Behavioral/MementoPattern.cs](../DesignPatterns/Behavioral/MementoPattern.cs)
- [DesignPatterns/Behavioral/NullObjectPattern.cs](../DesignPatterns/Behavioral/NullObjectPattern.cs)
- [DesignPatterns/Behavioral/ObserverPattern.cs](../DesignPatterns/Behavioral/ObserverPattern.cs)
- [DesignPatterns/Behavioral/SpecificationPattern.cs](../DesignPatterns/Behavioral/SpecificationPattern.cs)
- [DesignPatterns/Behavioral/StatePattern.cs](../DesignPatterns/Behavioral/StatePattern.cs)
- [DesignPatterns/Behavioral/StrategyPattern.cs](../DesignPatterns/Behavioral/StrategyPattern.cs)
- [DesignPatterns/Behavioral/TemplateMethodPattern.cs](../DesignPatterns/Behavioral/TemplateMethodPattern.cs)
- [DesignPatterns/Behavioral/VisitorPattern.cs](../DesignPatterns/Behavioral/VisitorPattern.cs)

---

## See Also

- [OOP Principles](OOP-Principles.md) - SOLID principles that guide pattern usage
- [Practical Patterns](Practical-Patterns.md) - Real-world implementation patterns
- [Data Access](Data-Access.md) - Repository pattern in practice
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
