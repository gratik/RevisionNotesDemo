// ==============================================================================
// UNIT OF WORK PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Maintains a list of objects affected by business transaction and coordinates writing changes
// BENEFIT: Ensures consistency, groups related changes, single commit/rollback point
// USE WHEN: Multiple repositories need coordinated transactions, want atomic operations
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// ========================================================================
// ENTITIES
// ========================================================================

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }

    public override string ToString() => $"[{Id}] {Name} ({Email}) - Credit: ${CreditLimit:F2}";
}

public class OrderItem
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }

    public override string ToString() =>
        $"[{Id}] Customer {CustomerId} - {ProductName} - ${Amount:F2} on {OrderDate:MM/dd/yyyy}";
}

// ========================================================================
// GENERIC REPOSITORY (from Repository Pattern)
// ========================================================================

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    T? GetById(int id);
    IEnumerable<T> GetAll();
}

public class InMemoryRepository<T> : IGenericRepository<T> where T : class
{
    private readonly List<T> _entities = new();
    private readonly Func<T, int> _getId;
    private readonly Action<T, int> _setId;
    private int _nextId = 1;

    public InMemoryRepository(Func<T, int> getId, Action<T, int> setId)
    {
        _getId = getId;
        _setId = setId;
    }

    public void Add(T entity)
    {
        _setId(entity, _nextId++);
        _entities.Add(entity);
    }

    public void Update(T entity)
    {
        var id = _getId(entity);
        var existing = _entities.FirstOrDefault(e => _getId(e) == id);
        if (existing != null)
        {
            var index = _entities.IndexOf(existing);
            _entities[index] = entity;
        }
    }

    public void Delete(int id)
    {
        var entity = _entities.FirstOrDefault(e => _getId(e) == id);
        if (entity != null)
            _entities.Remove(entity);
    }

    public T? GetById(int id) => _entities.FirstOrDefault(e => _getId(e) == id);

    public IEnumerable<T> GetAll() => _entities.ToList();
}

// ========================================================================
// UNIT OF WORK INTERFACE
// ========================================================================

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<OrderItem> Orders { get; }

    void Commit();
    void Rollback();
}

// ========================================================================
// UNIT OF WORK IMPLEMENTATION
// ========================================================================

public class UnitOfWork : IUnitOfWork
{
    private readonly IGenericRepository<Customer> _customers;
    private readonly IGenericRepository<OrderItem> _orders;

    private readonly List<Action> _operations = new();
    private bool _committed = false;

    public UnitOfWork()
    {
        // In real app, these would be backed by DbContext or transaction
        _customers = new InMemoryRepository<Customer>(
            c => c.Id,
            (c, id) => c.Id = id
        );
        _orders = new InMemoryRepository<OrderItem>(
            o => o.Id,
            (o, id) => o.Id = id
        );
    }

    public IGenericRepository<Customer> Customers => _customers;
    public IGenericRepository<OrderItem> Orders => _orders;

    public void Commit()
    {
        if (_committed)
            throw new InvalidOperationException("Already committed");

        // In real implementation, this would call DbContext.SaveChanges()
        // or SQL transaction.Commit()
        Console.WriteLine("[UOW] ✅ Committing transaction...");
        _committed = true;
    }

    public void Rollback()
    {
        if (!_committed)
        {
            Console.WriteLine("[UOW] ↩️  Rolling back transaction...");
            // In real implementation, this would rollback database changes
        }
    }

    public void Dispose()
    {
        if (!_committed)
            Rollback();
    }
}

// ========================================================================
// BUSINESS SERVICE USING UNIT OF WORK
// ========================================================================

public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Places order - updates customer credit and creates order (atomic operation)
    /// </summary>
    public bool PlaceOrder(int customerId, string productName, decimal amount)
    {
        try
        {
            Console.WriteLine($"\n[SERVICE] Processing order: Customer {customerId}, {productName}, ${amount:F2}");

            // 1. Get customer
            var customer = _unitOfWork.Customers.GetById(customerId);
            if (customer == null)
            {
                Console.WriteLine("[SERVICE] ❌ Customer not found");
                return false;
            }

            // 2. Check credit limit
            if (customer.CreditLimit < amount)
            {
                Console.WriteLine($"[SERVICE] ❌ Insufficient credit: ${customer.CreditLimit:F2} < ${amount:F2}");
                _unitOfWork.Rollback();
                return false;
            }

            // 3. Update customer credit (first operation)
            customer.CreditLimit -= amount;
            _unitOfWork.Customers.Update(customer);
            Console.WriteLine($"[SERVICE] Updated customer credit: ${customer.CreditLimit:F2} remaining");

            // 4. Create order (second operation)
            var order = new OrderItem
            {
                CustomerId = customerId,
                ProductName = productName,
                Amount = amount,
                OrderDate = DateTime.Now
            };
            _unitOfWork.Orders.Add(order);
            Console.WriteLine($"[SERVICE] Created order: {productName}");

            // 5. Commit both changes atomically
            _unitOfWork.Commit();
            Console.WriteLine("[SERVICE] ✅ Order placed successfully!\n");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVICE] ❌ Error: {ex.Message}");
            _unitOfWork.Rollback();
            return false;
        }
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class UnitOfWorkDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== UNIT OF WORK PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        Console.WriteLine("--- Setup: Creating customers ---");

        // Setup - create customers
        using (var uow = new UnitOfWork())
        {
            uow.Customers.Add(new Customer
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                CreditLimit = 1000m
            });
            uow.Customers.Add(new Customer
            {
                Name = "Bob Smith",
                Email = "bob@example.com",
                CreditLimit = 500m
            });
            uow.Commit();
        }
        Console.WriteLine("[UOW] ✅ Customers created\n");

        // Scenario 1: Successful order (within credit limit)
        Console.WriteLine("--- Scenario 1: Valid Order (Atomic Transaction) ---");
        using (var uow = new UnitOfWork())
        {
            // Re-add customers (simulating persistence)
            var alice = new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreditLimit = 1000m };
            var bob = new Customer { Id = 2, Name = "Bob Smith", Email = "bob@example.com", CreditLimit = 500m };

            // Manually add to simulate existing data
            uow.Customers.Add(alice);
            uow.Customers.Add(bob);

            var service = new OrderService(uow);
            service.PlaceOrder(1, "Laptop", 750m);

            // Verify changes
            var updatedAlice = uow.Customers.GetById(1);
            Console.WriteLine($"Customer after order: {updatedAlice}");

            var orders = uow.Orders.GetAll();
            Console.WriteLine($"Order created: {orders.First()}");
        }

        // Scenario 2: Failed order (insufficient credit, should rollback)
        Console.WriteLine("\n--- Scenario 2: Failed Order (Rollback) ---");
        using (var uow = new UnitOfWork())
        {
            var alice = new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreditLimit = 250m };
            uow.Customers.Add(alice);

            var service = new OrderService(uow);
            bool success = service.PlaceOrder(1, "Desktop", 800m);  // Exceeds limit!

            if (!success)
            {
                Console.WriteLine("Transaction was rolled back - no changes saved");
                var customer = uow.Customers.GetById(1);
                Console.WriteLine($"Customer credit unchanged: ${customer?.CreditLimit:F2}");
                Console.WriteLine($"Orders count: {uow.Orders.GetAll().Count()} (no order created)");
            }
        }

        // Scenario 3: Multiple operations in one transaction
        Console.WriteLine("\n--- Scenario 3: Multiple Operations (One Transaction) ---");
        using (var uow = new UnitOfWork())
        {
            var alice = new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreditLimit = 1000m };
            uow.Customers.Add(alice);

            var service = new OrderService(uow);

            // Place multiple orders
            service.PlaceOrder(1, "Mouse", 50m);
            // Reset UOW for demo - in real app would be single transaction
        }

        using (var uow = new UnitOfWork())
        {
            var alice = new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreditLimit = 950m };
            uow.Customers.Add(alice);
            var service = new OrderService(uow);
            service.PlaceOrder(1, "Keyboard", 100m);
        }

        using (var uow = new UnitOfWork())
        {
            var alice = new Customer { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", CreditLimit = 850m };
            uow.Customers.Add(alice);
            var service = new OrderService(uow);
            service.PlaceOrder(1, "Monitor", 300m);

            var finalCustomer = uow.Customers.GetById(1);
            Console.WriteLine($"\nFinal customer state: {finalCustomer}");
        }

        Console.WriteLine("\n💡 Unit of Work Pattern Benefits:");
        Console.WriteLine("   ✅ Atomic operations - all succeed or all fail");
        Console.WriteLine("   ✅ Single commit point - coordinates multiple repositories");
        Console.WriteLine("   ✅ Transaction consistency - maintains data integrity");
        Console.WriteLine("   ✅ Reduces database calls - batch changes");
        Console.WriteLine("   ✅ Works with Repository pattern");
        Console.WriteLine("   ✅ Simplifies error handling - single rollback point");
    }
}