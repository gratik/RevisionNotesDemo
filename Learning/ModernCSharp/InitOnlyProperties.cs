// ==============================================================================
// INIT-ONLY PROPERTIES AND REQUIRED MEMBERS - C# 9-11 Features
// ==============================================================================
// PURPOSE:
//   Demonstrate init-only setters and required members for immutable object initialization.
//   Create objects that can be set during initialization but not modified afterward.
//
// WHY INIT-ONLY AND REQUIRED:
//   - Immutability after construction
//   - Object initializer syntax (readable)
//   - Compile-time enforcement
//   - Better than constructor with 20 parameters
//   - Valid state guaranteed
//
// WHAT YOU'LL LEARN:
//   1. Init-only setters (C# 9)
//   2. Required members (C# 11)
//   3. Combining with records
//   4. Immutability patterns
//   5. Migration from mutable properties
//   6. Best practices for DTOs and configuration
//
// C# VERSIONS:
//   - C# 9: init keyword
//   - C# 11: required keyword
//
// THE BIG IDEA:
//   Set once during initialization, immutable thereafter.
// ==============================================================================

namespace RevisionNotesDemo.ModernCSharp;

/// <summary>
/// EXAMPLE 1: INIT-ONLY SETTERS - Basic Usage
/// 
/// THE PROBLEM:
/// Traditional properties are either:
/// - Fully mutable (can change anytime) - dangerous for DTOs
/// - Constructor-only (verbose 10-parameter constructors)
/// - Readonly fields (can't use object initializers)
/// 
/// THE SOLUTION:
/// Use 'init' keyword - property can be set during object initialization only,
/// then becomes readonly.
/// 
/// WHY IT MATTERS:
/// - Best of both worlds: initializer syntax + immutability
/// - More readable than huge constructors
/// - Enforced immutability (thread-safe)
/// - Great for DTOs, configuration, value objects
/// 
/// C# 9+
/// </summary>
public static class InitOnlyBasics
{
    // ❌ BAD: Fully mutable - can be changed anytime
    public class MutablePerson
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
    
    public static void DangerOfMutability()
    {
        var person = new MutablePerson
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        
        // ❌ Someone can change it later!
        person.FirstName = "Jane"; // This compiles - dangerous!
        person.DateOfBirth = DateTime.Now; // Whoops, changed age!
    }
    
    // ❌ BAD: Constructor-only (works but verbose)
    public class ConstructorOnlyPerson
    {
        public string FirstName { get; }
        public string LastName { get; }
        public DateTime DateOfBirth { get; }
        
        public ConstructorOnlyPerson(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }
    
    public static void VerboseConstruction()
    {
        // ❌ Less readable with many parameters
        var person = new ConstructorOnlyPerson("John", "Doe", new DateTime(1990, 1, 1));
        // What was the second parameter again? Not clear without IntelliSense
    }
    
    // ✅ GOOD: Init-only properties
    public class ImmutablePerson
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        
        // Computed property still works
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
    
    public static void SafeInitialization()
    {
        var person = new ImmutablePerson
        {
            FirstName = "John", // ✅ Clear what each value means
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        
        // ✅ Compiler error if you try to modify!
        // person.FirstName = "Jane"; // ❌ Error: Init-only property can only be assigned in object initializer
        
        // ✅ Thread-safe - no one can mutate it
    }
}

/// <summary>
/// EXAMPLE 2: REQUIRED MEMBERS - Ensuring Complete Initialization
/// 
/// THE PROBLEM:
/// Init-only properties can still be omitted during initialization,
/// leading to incomplete objects with default values.
/// 
/// THE SOLUTION:
/// Use 'required' keyword (C# 11) - compiler enforces you set the property.
/// 
/// WHY IT MATTERS:
/// - Compile-time guarantee all required fields are set
/// - No more validation in constructors
/// - Clear which properties are mandatory
/// - Better than constructor parameters for many fields
/// 
/// C# 11+ (.NET 7+)
/// </summary>
public static class RequiredMembersExample
{
    // ❌ BAD: Init-only but not required - can create incomplete objects
    public class IncompleteProduct
    {
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
    }
    
    public static void IncompleteObject()
    {
        // ❌ Compiles but incomplete!
        var product = new IncompleteProduct(); // All defaults - not valid
        Console.WriteLine(product.Name); // Empty string
        Console.WriteLine(product.Price); // 0
    }
    
    // ✅ GOOD: Required + init - must be set, then immutable
    public class CompleteProduct
    {
        required public string Name { get; init; }
        required public decimal Price { get; init; }
        required public string Category { get; init; }
        
        // Optional properties don't need 'required'
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
    
    public static void CompleteObject()
    {
        // ❌ Compiler error if you forget required properties!
        // var invalid = new CompleteProduct(); // Error: Required member 'Name' must be set
        
        // ✅ Must provide all required properties
        var valid = new CompleteProduct
        {
            Name = "Laptop",
            Price = 999.99m,
            Category = "Electronics",
            Description = "High-performance laptop" // Optional
        };
    }
    
    // ✅ GOOD: SetsRequiredMembers attribute for constructors
    public class ProductWithConstructor
    {
        required public string Name { get; init; }
        required public decimal Price { get; init; }
        
        // ✅ Constructor sets required members
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public ProductWithConstructor(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
        
        // Parameterless constructor still requires setting in initializer
        public ProductWithConstructor() { }
    }
    
    public static void ConstructorSetsRequired()
    {
        // ✅ Constructor satisfies required members
        var product1 = new ProductWithConstructor("Laptop", 999.99m);
        
        // ✅ Initializer also works
        var product2 = new ProductWithConstructor
        {
            Name = "Mouse",
            Price = 29.99m
        };
    }
}

/// <summary>
/// EXAMPLE 3: COMBINING RECORDS WITH INIT AND REQUIRED
/// 
/// THE PROBLEM:
/// Records already use init, but how to make properties required?
/// 
/// THE SOLUTION:
/// Records work perfectly with both init and required keywords.
/// 
/// WHY IT MATTERS:
/// - Records + init + required = perfect immutable DTOs
/// - Value equality built-in
/// - Concise syntax
/// - All the benefits together
/// </summary>
public static class RecordsWithInitAndRequired
{
    // ✅ GOOD: Record with positional syntax (all required by default)
    public record PersonPositional(string FirstName, string LastName, DateTime DateOfBirth);
    
    // ✅ GOOD: Record with nominal syntax + required
    public record PersonNominal
    {
        required public string FirstName { get; init; }
        required public string LastName { get; init; }
        public DateTime DateOfBirth { get; init; } = DateTime.MinValue;
        public string? MiddleName { get; init; } // Optional
    }
    
    // ✅ GOOD: Record with mix of required and optional
    public record Address
    {
        required public string Street { get; init; }
        required public string City { get; init; }
        required public string PostalCode { get; init; }
        public string? Unit { get; init; }
        public string? Province { get; init; }
    }
    
    // ✅ GOOD: Nested records
    public record Customer
    {
        required public string Name { get; init; }
        required public string Email { get; init; }
        required public Address Address { get; init; } // Nested required
        public DateTime? LastLogin { get; init; }
    }
    
    public static void RecordUsage()
    {
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Address = new Address
            {
                Street = "123 Main St",
                City = "Toronto",
                PostalCode = "M5V 1A1",
                Province = "ON"
            },
            LastLogin = DateTime.UtcNow
        };
        
        // ✅ With expression creates modified copy
        var movedCustomer = customer with
        {
            Address = customer.Address with { City = "Vancouver" }
        };
    }
}

/// <summary>
/// EXAMPLE 4: CONFIGURATION AND SETTINGS PATTERNS
/// 
/// THE PROBLEM:
/// Configuration classes need to be:
/// - Immutable after loading
/// - Validated (all required settings present)
/// - Bindable from JSON/appsettings
/// 
/// THE SOLUTION:
/// Use init + required for configuration classes.
/// Perfect for ASP.NET Core options pattern.
/// 
/// WHY IT MATTERS:
/// - Safe configuration (can't accidentally change)
/// - Compile-time checks for required settings
/// - Works great with IOptions<T>
/// - Clear which settings are mandatory
/// </summary>
public static class ConfigurationPatterns
{
    // ✅ GOOD: Application settings with required fields
    public class DatabaseSettings
    {
        required public string ConnectionString { get; init; }
        required public string DatabaseName { get; init; }
        public int MaxPoolSize { get; init; } = 100;
        public int CommandTimeout { get; init; } = 30;
    }
    
    public class CacheSettings
    {
        required public string RedisConnection { get; init; }
        public int DefaultExpiration { get; init; } = 3600;
        public bool Enabled { get; init; } = true;
    }
    
    public class AppSettings
    {
        required public DatabaseSettings Database { get; init; }
        required public CacheSettings Cache { get; init; }
        required public string AppName { get; init; }
        public string? Environment { get; init; }
    }
    
    // ✅ GOOD: Strongly-typed configuration from JSON
    // appsettings.json:
    // {
    //   "AppName": "MyApp",
    //   "Database": {
    //     "ConnectionString": "Server=...",
    //     "DatabaseName": "MyDb"
    //   },
    //   "Cache": {
    //     "RedisConnection": "localhost:6379"
    //   }
    // }
    
    // Usage in ASP.NET Core:
    // services.Configure<AppSettings>(configuration);
    // Then inject IOptions<AppSettings>
}

/// <summary>
/// EXAMPLE 5: DTO PATTERNS FOR APIs
/// 
/// THE PROBLEM:
/// API request/response DTOs should be:
/// - Immutable (don't change during request processing)
/// - Validated (all required fields present)
/// - Serializable (JSON, XML, etc.)
/// 
/// THE SOLUTION:
/// Use records with required init properties.
/// 
/// WHY IT MATTERS:
/// - Thread-safe request handlers
/// - Clear API contracts
/// - Validation at deserialization
/// - Perfect for Web APIs
/// </summary>
public static class DtoPatterns
{
    // ✅ GOOD: API Request DTOs
    public record CreateUserRequest
    {
        required public string Username { get; init; }
        required public string Email { get; init; }
        required public string Password { get; init; }
        public string? PhoneNumber { get; init; }
        public List<string> Roles { get; init; } = new();
    }
    
    public record UpdateUserRequest
    {
        required public int Id { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
    }
    
    // ✅ GOOD: API Response DTOs
    public record UserResponse
    {
        required public int Id { get; init; }
        required public string Username { get; init; }
        required public string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastLogin { get; init; }
    }
    
    public record ApiResponse<T>
    {
        required public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }
        public List<string> ValidationErrors { get; init; } = new();
    }
    
    // Example usage in controller:
    // [HttpPost]
    // public async Task<ApiResponse<UserResponse>> CreateUser(CreateUserRequest request)
    // {
    //     // request is immutable - safe to pass around
    //     var user = await _userService.CreateAsync(request);
    //     return new ApiResponse<UserResponse>
    //     {
    //         Success = true,
    //         Data = new UserResponse
    //         {
    //             Id = user.Id,
    //             Username = user.Username,
    //             Email = user.Email,
    //             CreatedAt = user.CreatedAt
    //         }
    //     };
    // }
}

/// <summary>
/// EXAMPLE 6: MIGRATION FROM MUTABLE TO INIT-ONLY
/// 
/// THE PROBLEM:
/// Existing codebase has mutable properties everywhere.
/// How to gradually adopt init-only?
/// 
/// THE SOLUTION:
/// Incremental migration strategies.
/// 
/// WHY IT MATTERS:
/// - Can't change everything at once
/// - Need backward compatibility
/// - Team adoption path
/// </summary>
public static class MigrationStrategies
{
    // PHASE 1: Old code (fully mutable)
    public class OldUser
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
    // PHASE 2: Add init alongside set (backward compatible)
    public class TransitionalUser
    {
        // Both set and init work
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
    // PHASE 3: Move to init-only for new properties
    public class ModernUser
    {
        // New properties: init-only
        required public string Id { get; init; }
        required public string Username { get; init; }
        
        // Legacy properties: still mutable (for now)
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
    // PHASE 4: Fully init-only + required
    public class FinalUser
    {
        required public string Id { get; init; }
        required public string Username { get; init; }
        required public string Name { get; init; }
        required public string Email { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
    
    // TIP: Use analyzers to detect mutable properties
    // TIP: Start with new code, migrate old code gradually
    // TIP: Record types are easier to migrate to than classes
}

/// <summary>
/// EXAMPLE 7: ADVANCED PATTERNS AND BEST PRACTICES
/// 
/// THE PROBLEM:
/// What about complex scenarios - collections, nested objects, validation?
/// 
/// THE SOLUTION:
/// Combine init/required with other patterns.
/// 
/// WHY IT MATTERS:
/// - Handle real-world complexity
/// - Build robust immutable models
/// </summary>
public static class AdvancedPatterns
{
    // ✅ PATTERN 1: Immutable collections
    public record Order
    {
        required public int Id { get; init; }
        required public string OrderNumber { get; init; }
        
        // ❌ List is mutable even if property is init!
        public List<OrderItem> BadItems { get; init; } = new();
        
        // ✅ Use IReadOnlyList or ImmutableList
        public IReadOnlyList<OrderItem> GoodItems { get; init; } = Array.Empty<OrderItem>();
    }
    
    public static void ImmutableCollectionUsage()
    {
        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD-001",
            GoodItems = new List<OrderItem>
            {
                new() { ProductId = 100, Quantity = 2 },
                new() { ProductId = 101, Quantity = 1 }
            }
        };
        
        // ✅ Can't reassign property
        // order.GoodItems = new List<OrderItem>(); // Error
        
        // ⚠️ But can still modify contents if using List!
        // ((List<OrderItem>)order.BadItems).Add(new OrderItem()); // Works but bad!
        
        // ✅ IReadOnlyList prevents mutation
        // order.GoodItems.Add(new OrderItem()); // Error: no Add method
    }
    
    // ✅ PATTERN 2: Validation in constructors
    public record ValidatedProduct
    {
        required public string Name { get; init; }
        required public decimal Price { get; init; }
        
        // Validation constructor
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public ValidatedProduct(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));
            
            Name = name;
            Price = price;
        }
        
        // Parameterless for deserializers
        public ValidatedProduct() { }
    }
    
    // ✅ PATTERN 3: Builder pattern for complex objects
    public class ComplexObject
    {
        required public string Property1 { get; init; }
        required public string Property2 { get; init; }
        required public string Property3 { get; init; }
        // ... 20 more properties
    }
    
    public class ComplexObjectBuilder
    {
        private string _property1 = string.Empty;
        private string _property2 = string.Empty;
        private string _property3 = string.Empty;
        
        public ComplexObjectBuilder WithProperty1(string value)
        {
            _property1 = value;
            return this;
        }
        
        public ComplexObjectBuilder WithProperty2(string value)
        {
            _property2 = value;
            return this;
        }
        
        public ComplexObjectBuilder WithProperty3(string value)
        {
            _property3 = value;
            return this;
        }
        
        public ComplexObject Build() => new()
        {
            Property1 = _property1,
            Property2 = _property2,
            Property3 = _property3
        };
    }
    
    // ✅ PATTERN 4: With expressions for updates
    public record UserProfile
    {
        required public string Email { get; init; }
        required public string DisplayName { get; init; }
        public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    }
    
    public static UserProfile UpdateEmail(UserProfile profile, string newEmail)
    {
        // ✅ Create modified copy
        return profile with
        {
            Email = newEmail,
            LastUpdated = DateTime.UtcNow
        };
    }
}

// Supporting types
public record OrderItem
{
    required public int ProductId { get; init; }
    required public int Quantity { get; init; }
}

// SUMMARY OF WHEN TO USE:
// 
// Use 'init':
// ✅ DTOs (request/response)
// ✅ Configuration classes
// ✅ Value objects
// ✅ Event data
// ✅ Any data that shouldn't change after creation
//
// Use 'required':
// ✅ Mandatory fields in DTOs
// ✅ Required configuration settings
// ✅ Any property that must be set for valid state
//
// Use both 'required init':
// ✅ Perfect combination for immutable objects with mandatory fields
// ✅ API contracts, configuration, domain events
//
// DON'T use init for:
// ❌ Entities with change tracking (e.g., EF Core entities)
// ❌ Objects that need to be mutated (builders, caches, etc.)
// ❌ Properties set after construction (lazy-loaded, computed)
