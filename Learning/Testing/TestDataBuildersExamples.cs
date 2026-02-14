// ==============================================================================
// TEST DATA BUILDERS - Builder Pattern for Test Data
// Reference: Revision Notes - Unit Testing Best Practices
// ==============================================================================
// PURPOSE: Demonstrate test data builders for cleaner, more maintainable tests
// KEY CONCEPTS: Builder pattern, fluent API, Object Mother pattern, sensible defaults
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace RevisionNotesDemo.Testing;

// Domain entities
public class TestCustomer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TestOrder> Orders { get; set; } = new();
}

public class TestOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TestDataBuildersExamples
{
    /// <summary>
    /// PROBLEM: Tests with complex object creation
    /// </summary>
    public static void ProblemExample()
    {
        Console.WriteLine("\n=== PROBLEM: Tedious Object Creation ===");
        
        // Test 1
        var customer1 = new TestCustomer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "555-1234",
            IsActive = true,
            CreatedAt = DateTime.Now,
            Orders = new List<TestOrder>
            {
                new TestOrder { Id = 1, OrderDate = DateTime.Now, Total = 99.99m, Status = "Completed" }
            }
        };
        
        // Test 2 - almost identical!
        var customer2 = new TestCustomer
        {
            Id = 2,
            Name = "Jane Smith",
            Email = "jane@example.com",
            PhoneNumber = "555-5678",
            IsActive = true,
            CreatedAt = DateTime.Now,
            Orders = new List<TestOrder>
            {
                new TestOrder { Id = 2, OrderDate = DateTime.Now, Total = 149.99m, Status = "Completed" }
            }
        };
        
        Console.WriteLine("❌ Problems: Repetition, brittle, hard to maintain");
    }
    
    /// <summary>
    /// SOLUTION: Customer Builder
    /// </summary>
    public class CustomerBuilder
    {
        private int _id = 1;
        private string _name = "John Doe";
        private string _email = "john@example.com";
        private string _phoneNumber = "555-1234";
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.Now;
        private List<TestOrder> _orders = new();
        
        // Fluent methods
        public CustomerBuilder WithId(int id)
        {
            _id = id;
            return this;
        }
        
        public CustomerBuilder WithName(string name)
        {
            _name = name;
            return this;
        }
        
        public CustomerBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }
        
        public CustomerBuilder IsInactive()
        {
            _isActive = false;
            return this;
        }
        
        public CustomerBuilder WithOrder(TestOrder order)
        {
            _orders.Add(order);
            return this;
        }
        
        public CustomerBuilder WithOrders(params TestOrder[] orders)
        {
            _orders.AddRange(orders);
            return this;
        }
        
        public TestCustomer Build()
        {
            return new TestCustomer
            {
                Id = _id,
                Name = _name,
                Email = _email,
                PhoneNumber = _phoneNumber,
                IsActive = _isActive,
                CreatedAt = _createdAt,
                Orders = _orders
            };
        }
        
        // Implicit conversion for convenience
        public static implicit operator TestCustomer(CustomerBuilder builder) => builder.Build();
    }
    
    /// <summary>
    /// EXAMPLE 1: Using Customer Builder
    /// </summary>
    public static void CustomerBuilderExample()
    {
        Console.WriteLine("\n=== SOLUTION: Customer Builder ===");
        
        // Test 1 - Default customer
        var customer1 = new CustomerBuilder()
            .WithId(1)
            .Build();
        
        // Test 2 - Inactive customer
        var customer2 = new CustomerBuilder()
            .WithId(2)
            .WithName("Jane Smith")
            .IsInactive()
            .Build();
        
        // Test 3 - Customer with orders
        var customer3 = new CustomerBuilder()
            .WithId(3)
            .WithOrders(
                new TestOrder { Id = 1, OrderDate = DateTime.Now, Total = 99.99m, Status = "Completed" },
                new TestOrder { Id = 2, OrderDate = DateTime.Now, Total = 149.99m, Status = "Pending" }
            )
            .Build();
        
        Console.WriteLine("✅ Advantages:");
        Console.WriteLine("   • Sensible defaults");
        Console.WriteLine("   • Fluent API (readable)");
        Console.WriteLine("   • Only specify what matters for each test");
        Console.WriteLine("   • Easy to maintain");
    }
    
    /// <summary>
    /// EXAMPLE 2: Object Mother Pattern (Named Scenarios)
    /// </summary>
    public class CustomerMother
    {
        public static TestCustomer CreateDefault() => new CustomerBuilder().Build();
        
        public static TestCustomer CreatePremium() => new CustomerBuilder()
            .WithEmail("premium@example.com")
            .WithOrders(
                new TestOrder { Total = 1000m, Status = "Completed" },
                new TestOrder { Total = 2000m, Status = "Completed" }
            )
            .Build();
        
        public static TestCustomer CreateInactive() => new CustomerBuilder()
            .IsInactive()
            .Build();
        
        public static TestCustomer CreateWithManyOrders() => new CustomerBuilder()
            .WithOrders(Enumerable.Range(1, 10).Select(i => 
                new TestOrder { Id = i, OrderDate = DateTime.Now, Total = i * 10m, Status = "Completed" }
            ).ToArray())
            .Build();
    }
    
    public static void ObjectMotherExample()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Object Mother Pattern ===");
        
        // Clear intent - what kind of customer are we testing?
        var defaultCustomer = CustomerMother.CreateDefault();
        var premiumCustomer = CustomerMother.CreatePremium();
        var inactiveCustomer = CustomerMother.CreateInactive();
        
        Console.WriteLine($"Default customer: {defaultCustomer.Name}");
        Console.WriteLine($"Premium customer orders: {premiumCustomer.Orders.Count}");
        Console.WriteLine($"Inactive customer active: {inactiveCustomer.IsActive}");
        
        Console.WriteLine("\n✅ Object Mother: Named test scenarios for clarity");
    }
    
    /// <summary>
    /// EXAMPLE 3: Auto-Incrementing IDs
    /// </summary>
    public class CustomerBuilderWithAutoIncrement
    {
        private static int _nextId = 1;
        
        private int _id = _nextId++;
        private string _name = "Customer";
        
        public CustomerBuilderWithAutoIncrement WithName(string name)
        {
            _name = name;
            return this;
        }
        
        public TestCustomer Build() => new TestCustomer
        {
            Id = _id,
            Name = $"{_name} {_id}",
            Email = $"customer{_id}@example.com",
            IsActive = true,
            CreatedAt = DateTime.Now
        };
    }
    
    public static void AutoIncrementExample()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Auto-Incrementing IDs ===");
        
        var customer1 = new CustomerBuilderWithAutoIncrement().Build();
        var customer2 = new CustomerBuilderWithAutoIncrement().Build();
        var customer3 = new CustomerBuilderWithAutoIncrement().Build();
        
        Console.WriteLine($"Customer 1 ID: {customer1.Id}, Email: {customer1.Email}");
        Console.WriteLine($"Customer 2 ID: {customer2.Id}, Email: {customer2.Email}");
        Console.WriteLine($"Customer 3 ID: {customer3.Id}, Email: {customer3.Email}");
        
        Console.WriteLine("\n✅ Auto-increment: Unique IDs without specifying them");
    }
    
    /// <summary>
    /// EXAMPLE 4: Collection Builder
    /// </summary>
    public class CustomerCollectionBuilder
    {
        private int _count = 1;
        private Action<CustomerBuilder>? _configure;
        
        public CustomerCollectionBuilder WithCount(int count)
        {
            _count = count;
            return this;
        }
        
        public CustomerCollectionBuilder Configure(Action<CustomerBuilder> configure)
        {
            _configure = configure;
            return this;
        }
        
        public List<TestCustomer> Build()
        {
            return Enumerable.Range(1, _count).Select(i =>
            {
                var builder = new CustomerBuilder().WithId(i);
                _configure?.Invoke(builder);
                return builder.Build();
            }).ToList();
        }
    }
    
    public static void CollectionBuilderExample()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Collection Builder ===");
        
        // Create 5 inactive customers
        var inactiveCustomers = new CustomerCollectionBuilder()
            .WithCount(5)
            .Configure(b => b.IsInactive())
            .Build();
        
        Console.WriteLine($"Created {inactiveCustomers.Count} inactive customers");
        Console.WriteLine($"All inactive: {inactiveCustomers.All(c => !c.IsActive)}");
        
        Console.WriteLine("\n✅ Collection Builder: Create multiple test objects easily");
    }
    
    /// <summary>
    /// EXAMPLE 5: Alternatives (Bogus, AutoFixture)
    /// </summary>
    public static void AlternativesExample()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Alternatives ===");
        
        Console.WriteLine("\n📦 BOGUS (Fake Data Library):");
        Console.WriteLine("   dotnet add package Bogus");
        Console.WriteLine("   var faker = new Faker<TestCustomer>()");
        Console.WriteLine("       .RuleFor(c => c.Name, f => f.Name.FullName())");
        Console.WriteLine("       .RuleFor(c => c.Email, f => f.Internet.Email());");
        Console.WriteLine("   var customer = faker.Generate();");
        
        Console.WriteLine("\n📦 AUTOFIXTURE (Auto-Generation):");
        Console.WriteLine("   dotnet add package AutoFixture");
        Console.WriteLine("   var fixture = new Fixture();");
        Console.WriteLine("   var customer = fixture.Create<TestCustomer>();");
        Console.WriteLine("   Automatically fills all properties!");
    }
    
    /// <summary>
    /// Best Practices
    /// </summary>
    public static void BestPractices()
    {
        Console.WriteLine("\n=== TEST DATA BUILDERS - BEST PRACTICES ===");
        Console.WriteLine("✅ Use builders for complex objects");
        Console.WriteLine("✅ Provide sensible defaults");
        Console.WriteLine("✅ Make intent clear (only override what matters)");
        Console.WriteLine("✅ Use Object Mother for common scenarios");
        Console.WriteLine("✅ Consider Bogus for realistic fake data");
        Console.WriteLine("✅ Consider AutoFixture for auto-generation");
        Console.WriteLine("✅ Keep builders in test project");
        Console.WriteLine("✅ Update builders when model changes");
    }
    
    public static void RunAllExamples()
    {
        Console.WriteLine("\n=== TEST DATA BUILDERS EXAMPLES ===\n");
        ProblemExample();
        CustomerBuilderExample();
        ObjectMotherExample();
        AutoIncrementExample();
        CollectionBuilderExample();
        AlternativesExample();
        BestPractices();
        Console.WriteLine("\nTest Data Builders examples completed!\n");
    }
}
