// ==============================================================================
// MAPPING PATTERNS
// Reference: Revision Notes - Practical Scenarios
// ==============================================================================
// PURPOSE: Transform objects between different representations (DTO ↔ Entity, ViewModel ↔ Model)
// BENEFIT: Separation of concerns, API contracts, data shaping, security
// USE WHEN: Need to transform between layers, expose DTOs, map domain to persistence
// ==============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

// ========================================================================
// DOMAIN ENTITIES (Internal representation)
// ========================================================================

public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // Sensitive!
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; } = "User";
}

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }  // Internal only!
    public int Stock { get; set; }
    public CategoryEntity? Category { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ========================================================================
// DTOs (Data Transfer Objects - External representation)
// ========================================================================

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // No PasswordHash - security!
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    // No Cost - internal only!
    public int Stock { get; set; }
    public string? CategoryName { get; set; }  // Flattened
    public int DaysSinceCreated { get; set; }  // Computed
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
}

// ========================================================================
// PATTERN 1: MANUAL MAPPING (Full control, no dependencies)
// ========================================================================

public static class ManualMapper
{
    public static UserDto ToDto(UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt,
            LastLoginAt = entity.LastLoginAt,
            IsActive = entity.IsActive
            // PasswordHash deliberately excluded for security
        };
    }

    public static ProductDto ToDto(ProductEntity entity)
    {
        return new ProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            CategoryName = entity.Category?.Name,  // Flatten nested object
            DaysSinceCreated = (DateTime.Now - entity.CreatedAt).Days  // Computed property
            // Cost deliberately excluded
        };
    }

    public static ProductEntity ToEntity(CreateProductDto dto, CategoryEntity category)
    {
        return new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Category = category,
            CreatedAt = DateTime.Now
            // Id and Cost set elsewhere
        };
    }
}

// ========================================================================
// PATTERN 2: EXTENSION METHODS (Fluent API)
// ========================================================================

public static class MappingExtensions
{
    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt,
            LastLoginAt = entity.LastLoginAt,
            IsActive = entity.IsActive
        };
    }

    public static ProductDto ToDto(this ProductEntity entity)
    {
        return new ProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            CategoryName = entity.Category?.Name,
            DaysSinceCreated = (DateTime.Now - entity.CreatedAt).Days
        };
    }

    public static List<UserDto> ToDtoList(this IEnumerable<UserEntity> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static List<ProductDto> ToDtoList(this IEnumerable<ProductEntity> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}

// ========================================================================
// PATTERN 3: MAPPER CLASS (Centralized, testable)
// ========================================================================

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
    List<TDestination> MapList(IEnumerable<TSource> sources);
}

public class UserMapper : IMapper<UserEntity, UserDto>
{
    public UserDto Map(UserEntity source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            CreatedAt = source.CreatedAt,
            LastLoginAt = source.LastLoginAt,
            IsActive = source.IsActive
        };
    }

    public List<UserDto> MapList(IEnumerable<UserEntity> sources)
    {
        return sources.Select(Map).ToList();
    }
}

public class ProductMapper : IMapper<ProductEntity, ProductDto>
{
    public ProductDto Map(ProductEntity source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return new ProductDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            Price = source.Price,
            Stock = source.Stock,
            CategoryName = source.Category?.Name,
            DaysSinceCreated = (DateTime.Now - source.CreatedAt).Days
        };
    }

    public List<ProductDto> MapList(IEnumerable<ProductEntity> sources)
    {
        return sources.Select(Map).ToList();
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class MappingPatternsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== MAPPING PATTERNS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios\n");

        // Sample data
        var userEntity = new UserEntity
        {
            Id = 1,
            Username = "john_doe",
            Email = "john@example.com",
            PasswordHash = "$2a$10$HASHED_PASSWORD_HERE",  // Sensitive!
            CreatedAt = DateTime.Now.AddYears(-2),
            LastLoginAt = DateTime.Now.AddDays(-3),
            IsActive = true,
            Role = "Admin"
        };

        var category = new CategoryEntity { Id = 1, Name = "Electronics" };

        var productEntity = new ProductEntity
        {
            Id = 101,
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse with 6 buttons",
            Price = 29.99m,
            Cost = 15.00m,  // Internal cost - should NOT be exposed!
            Stock = 150,
            Category = category,
            CreatedAt = DateTime.Now.AddDays(-45)
        };

        // Pattern 1: Manual Mapping
        Console.WriteLine("=== PATTERN 1: Manual Mapping ===\n");
        Console.WriteLine("Static methods in mapper class\n");

        var userDto1 = ManualMapper.ToDto(userEntity);
        Console.WriteLine($"[MANUAL] User mapped to DTO:");
        Console.WriteLine($"  Username: {userDto1.Username}");
        Console.WriteLine($"  Email: {userDto1.Email}");
        Console.WriteLine($"  PasswordHash: {(string.IsNullOrEmpty(userDto1.Email) ? "(null)" : "EXCLUDED ✅")}");
        Console.WriteLine($"  IsActive: {userDto1.IsActive}\n");

        var productDto1 = ManualMapper.ToDto(productEntity);
        Console.WriteLine($"[MANUAL] Product mapped to DTO:");
        Console.WriteLine($"  Name: {productDto1.Name}");
        Console.WriteLine($"  Price: ${productDto1.Price}");
        Console.WriteLine($"  Cost: (EXCLUDED ✅)");
        Console.WriteLine($"  Category: {productDto1.CategoryName}");
        Console.WriteLine($"  Days since created: {productDto1.DaysSinceCreated}\n");

        // Pattern 2: Extension Methods
        Console.WriteLine("=== PATTERN 2: Extension Methods ===\n");
        Console.WriteLine("Fluent API using extension methods\n");

        var userDto2 = userEntity.ToDto();  // Fluent!
        Console.WriteLine($"[EXTENSION] User mapped: {userDto2.Username} ({userDto2.Email})");

        var productDto2 = productEntity.ToDto();  // Fluent!
        Console.WriteLine($"[EXTENSION] Product mapped: {productDto2.Name} - ${productDto2.Price}\n");

        // Batch mapping with extension methods
        var users = new List<UserEntity> { userEntity };
        var products = new List<ProductEntity> { productEntity };

        var userDtos = users.ToDtoList();
        var productDtos = products.ToDtoList();

        Console.WriteLine($"[EXTENSION] Batch mapping:");
        Console.WriteLine($"  Mapped {userDtos.Count} users");
        Console.WriteLine($"  Mapped {productDtos.Count} products\n");

        // Pattern 3: Mapper Class
        Console.WriteLine("=== PATTERN 3: Mapper Class (Dependency Injection) ===\n");
        Console.WriteLine("Dedicated mapper classes, easy to inject and test\n");

        var userMapper = new UserMapper();
        var productMapper = new ProductMapper();

        var userDto3 = userMapper.Map(userEntity);
        Console.WriteLine($"[MAPPER CLASS] User: {userDto3.Username}");

        var productDto3 = productMapper.Map(productEntity);
        Console.WriteLine($"[MAPPER CLASS] Product: {productDto3.Name}\n");

        // Demonstrate security benefit
        Console.WriteLine("=== Security Benefit: Sensitive Data Exclusion ===\n");
        Console.WriteLine($"Entity (internal):");
        Console.WriteLine($"  PasswordHash: {userEntity.PasswordHash}");
        Console.WriteLine($"  Cost: ${productEntity.Cost:F2}");
        Console.WriteLine($"\nDTO (exposed via API):");
        Console.WriteLine($"  PasswordHash: (not present) ✅");
        Console.WriteLine($"  Cost: (not present) ✅");
        Console.WriteLine($"\n🔒 Sensitive data protected from external exposure!\n");

        // Demonstrate computed properties
        Console.WriteLine("=== Computed Properties ===\n");
        Console.WriteLine($"Entity CreatedAt: {productEntity.CreatedAt:yyyy-MM-dd}");
        Console.WriteLine($"DTO DaysSinceCreated: {productDto3.DaysSinceCreated} days (computed!)");
        Console.WriteLine($"DTO CategoryName: {productDto3.CategoryName} (flattened!)\n");

        Console.WriteLine("💡 Mapping Pattern Comparison:");
        Console.WriteLine("\n🔹 Manual Mapping (Static Methods):");
        Console.WriteLine("   ✅ Simple and explicit");
        Console.WriteLine("   ✅ No dependencies");
        Console.WriteLine("   ✅ Full control over mapping logic");
        Console.WriteLine("   ❌ Repetitive code");

        Console.WriteLine("\n🔹 Extension Methods:");
        Console.WriteLine("   ✅ Fluent API (obj.ToDto())");
        Console.WriteLine("   ✅ Easy to discover");
        Console.WriteLine("   ✅ Clean call site");
        Console.WriteLine("   ❌ Can clutter namespace");

        Console.WriteLine("\n🔹 Mapper Classes:");
        Console.WriteLine("   ✅ Dependency injection friendly");
        Console.WriteLine("   ✅ Easy to test and mock");
        Console.WriteLine("   ✅ Can inject dependencies into mapper");
        Console.WriteLine("   ✅ Follows SRP");

        Console.WriteLine("\n💡 Popular Libraries:");
        Console.WriteLine("   • AutoMapper - convention-based automatic mapping");
        Console.WriteLine("   • Mapster - fast compile-time mapping");
        Console.WriteLine("   • TinyMapper - lightweight");

        Console.WriteLine("\n💡 When to Use Mapping:");
        Console.WriteLine("   ✅ API responses (Entity → DTO)");
        Console.WriteLine("   ✅ API requests (DTO → Entity)");
        Console.WriteLine("   ✅ Layer separation (Domain → Persistence)");
        Console.WriteLine("   ✅ ViewModels (Entity → ViewModel)");
        Console.WriteLine("   ✅ Exclude sensitive data");
        Console.WriteLine("   ✅ Flatten complex objects");
        Console.WriteLine("   ✅ Add computed properties");

        Console.WriteLine("\n💡 Best Practices:");
        Console.WriteLine("   ✅ Never expose entities directly via API");
        Console.WriteLine("   ✅ Use DTOs to control API surface area");
        Console.WriteLine("   ✅ Map at service/controller layer");
        Console.WriteLine("   ✅ Keep mapping logic centralized");
        Console.WriteLine("   ✅ Write tests for complex mappings");
    }
}