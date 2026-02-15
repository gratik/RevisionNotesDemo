// ==============================================================================
// NULLABLE REFERENCE TYPES - C# 8+ Null Safety
// ==============================================================================
// WHAT IS THIS?
// -------------
// Compiler-enforced nullability annotations and flow analysis.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents NullReferenceException at compile time
// ✅ Documents intent for API contracts
//
// WHEN TO USE
// -----------
// ✅ Any modern C# codebase and public APIs
// ✅ DTOs and models with optional fields
//
// WHEN NOT TO USE
// ---------------
// ❌ Legacy code only when migration is not yet possible
// ❌ Disabling nullability to avoid fixing warnings
//
// REAL-WORLD EXAMPLE
// ------------------
// Mark optional fields as nullable in DTOs.
// ==============================================================================

// GOTCHA: Nullable context can be enabled per-file, per-project, or globally
#nullable enable  // Enable for this file

namespace RevisionNotesDemo.ModernCSharp;

/// <summary>
/// EXAMPLE 1: NULLABLE VS NON-NULLABLE REFERENCE TYPES
/// 
/// THE PROBLEM:
/// Before C# 8, ANY reference type could be null, leading to unexpected NullReferenceException.
/// No way to express "this parameter should never be null".
/// 
/// THE SOLUTION:
/// Enable nullable reference types - reference types are non-nullable by default.
/// Use '?' to explicitly allow null.
/// 
/// WHY IT MATTERS:
/// - Prevents ~80% of NullReferenceExceptions
/// - Documents intent in code
/// - Compiler helps you handle nulls properly
/// - Better IntelliSense and warnings
/// 
/// SETUP:
/// Add to .csproj: <Nullable>enable</Nullable>
/// Or use: #nullable enable
/// </summary>
public static class NullableBasicsExamples
{
    // ❌ BAD: Before nullable reference types (C# 7)
    // Any of these could be null - no compiler help
    public static class OldWay
    {
        public static string GetFullName(string firstName, string lastName)
        {
            // No warning! Could throw NullReferenceException
            return firstName.ToUpper() + " " + lastName.ToUpper();
        }

        public static int GetLength(string value)
        {
            // Defensive null check everywhere
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return value.Length;
        }
    }

    // ✅ GOOD: With nullable reference types (C# 8+)
    public static class NewWay
    {
        // Non-nullable parameters - compiler enforces
        public static string GetFullName(string firstName, string lastName)
        {
            // ✅ Compiler knows these are never null
            return firstName.ToUpper() + " " + lastName.ToUpper();
        }

        // Nullable parameter - must handle null
        public static int GetLength(string? value)
        {
            // ⚠️ Compiler warning if you access without null check
            // return value.Length; // Warning: Dereference of a possibly null reference

            if (value == null)
                return 0;
            return value.Length; // ✅ Compiler knows it's non-null here
        }

        // ✅ Better: Use null-coalescing
        public static int GetLengthBetter(string? value) =>
            value?.Length ?? 0;
    }

    // ✅ GOOD: Expressing different nullability scenarios
    public class UserService
    {
        // Non-nullable - must be initialized
        private readonly ILogger _logger;

        // Nullable - can be null
        private ICache? _cache;

        // Non-nullable with default - never null after construction
        public string ConnectionString { get; init; } = string.Empty;

        public UserService(ILogger logger, ICache? cache = null)
        {
            _logger = logger; // ✅ Must assign
            _cache = cache;   // ✅ OK to be null
        }

        // ✅ Method returning nullable value
        public User? FindById(int id)
        {
            // Explicit return of null is fine
            return id == 0 ? null : new User { Id = id, Name = "Test" };
        }

        // ✅ Method guaranteed to return non-null
        public User GetById(int id)
        {
            var user = FindById(id);
            if (user == null)
                throw new KeyNotFoundException($"User {id} not found");
            return user; // Compiler knows it's non-null
        }
    }
}

/// <summary>
/// EXAMPLE 2: NULLABILITY FLOW ANALYSIS
/// 
/// THE PROBLEM:
/// Need to check null everywhere, even after you've already checked.
/// 
/// THE SOLUTION:
/// Compiler tracks null state through your code flow.
/// 
/// WHY IT MATTERS:
/// - Fewer unnecessary null checks
/// - Compiler understands if/throw/return patterns
/// - Smart enough to know when you've handled null
/// 
/// PERFORMANCE: No runtime cost - purely compile-time analysis
/// </summary>
public static class FlowAnalysisExamples
{
    // ✅ GOOD: Flow analysis after null check
    public static int GetLengthOrDefault(string? input)
    {
        if (input == null)
            return 0;

        // ✅ Compiler knows input is non-null here
        return input.Length; // No warning
    }

    // ✅ GOOD: Flow analysis after throw
    public static string ProcessRequired(string? input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        // ✅ Compiler knows input is non-null (throw exits flow)
        return input.ToUpper();
    }

    // ✅ GOOD: Flow analysis with null-coalescing
    public static string GetDisplayName(User? user)
    {
        // ✅ After null-coalescing, userName is never null
        string userName = user?.Name ?? "Guest";
        return userName.ToUpper(); // No warning
    }

    // ✅ GOOD: Pattern matching affects flow
    public static string Describe(object? obj)
    {
        if (obj is string text)
        {
            // ✅ Compiler knows 'text' is non-null string
            return $"String: {text.ToUpper()}";
        }

        return "Not a string";
    }

    // ⚠️ GOTCHA: Flow analysis limitations
    public static class FlowLimitations
    {
        // ❌ Flow analysis doesn't work across method boundaries
        private static bool IsNotNull(string? value) => value != null;

        public static int BadExample(string? input)
        {
            if (IsNotNull(input))
            {
                // ⚠️ Warning! Compiler doesn't know IsNotNull checked  it
                // return input.Length; // Warning: possible null reference

                // ✅ Need null-forgiving operator or another check
                return input!.Length;
            }
            return 0;
        }

        // ✅ GOOD: Inline check or use attributes (see Example 5)
        public static int GoodExample(string? input)
        {
            if (input != null)
            {
                return input.Length; // ✅ No warning
            }
            return 0;
        }
    }
}

/// <summary>
/// EXAMPLE 3: NULL-FORGIVING OPERATOR (!)
/// 
/// THE PROBLEM:
/// Sometimes YOU know a value isn't null, but the compiler doesn't.
/// Examples: after initialization, dependency injection, reflection, etc.
/// 
/// THE SOLUTION:
/// Use '!' operator to tell compiler "trust me, this is non-null".
/// 
/// WHY IT MATTERS:
/// - Suppress warnings when you know better
/// - Interop with legacy code
/// - Framework patterns (DI, lazy init)
/// 
/// ⚠️ DANGER: You're overriding compiler safety. Use sparingly!
/// If you're wrong, you'll get NullReferenceException at runtime.
/// </summary>
public static class NullForgivingExamples
{
    public class ServiceWithDI
    {
        // ⚠️ DI will initialize this, but compiler doesn't know
        private ILogger _logger = null!; // "I promise this won't be null"

        // ✅ Constructor injection - compiler understands
        public ServiceWithDI() { }

        public void Initialize(ILogger logger)
        {
            _logger = logger; // DI framework calls this
        }

        public void DoWork()
        {
            // ✅ We used null-forgiving in initialization
            _logger.LogInformation("Working..."); // No warning
        }
    }

    // ✅ GOOD: Lazy initialization pattern
    public class LazyService
    {
        private ICache? _cacheInstance;

        private ICache Cache
        {
            get
            {
                if (_cacheInstance == null)
                {
                    _cacheInstance = new MemoryCache();
                }
                // ✅ We know it's non-null after initialization
                return _cacheInstance; // or: return _cacheInstance!;
            }
        }
    }

    // ⚠️ WHEN TO USE '!':
    // ✅ After validation methods you control
    // ✅ Framework initialization patterns (DI, ASP.NET, etc.)
    // ✅ Interop with legacy non-nullable-aware code
    // ❌ To silence warnings when you're not sure
    // ❌ Instead of proper null checks

    // ❌ BAD: Overusing null-forgiving
    public static string BadProcessUser(int id)
    {
        var user = FindUserById(id); // Returns User?
        return user!.Name.ToUpper(); // 💥 Could throw if not found!
    }

    // ✅ GOOD: Proper null handling
    public static string GoodProcessUser(int id)
    {
        var user = FindUserById(id);
        return user?.Name.ToUpper() ?? "Unknown";
    }

    private static User? FindUserById(int id) => null; // Stub
}

/// <summary>
/// EXAMPLE 4: NULLABLE COLLECTIONS AND GENERICS
/// 
/// THE PROBLEM:
/// What about List<string?>? List<string>? Dictionary<string?, User?>?
/// Nullability gets complex with collections.
/// 
/// THE SOLUTION:
/// Understand the difference:
/// - List<string>  = non-nullable list of non-nullable strings
/// - List<string?> = non-nullable list of NULLABLE strings
/// - List<string>? = NULLABLE list of non-nullable strings
/// - List<string?>?= nullable list of nullable strings
/// 
/// WHY IT MATTERS:
/// - Express exact intent
/// - Avoid null checks on collections themselves
/// - Handle null items correctly
/// </summary>
public static class NullableCollectionExamples
{
    // ✅ GOOD: Different collection nullability scenarios
    public class CollectionPatterns
    {
        // Non-nullable list, non-nullable items
        public List<string> RequiredNames { get; set; } = new();

        // Non-nullable list, nullable items (can contain nulls)
        public List<string?> OptionalNames { get; set; } = new();

        // Nullable list, non-nullable items
        public List<User>? CachedUsers { get; set; }

        // Dictionary with nullable value
        public Dictionary<int, User?> UserCache { get; set; } = new();
    }

    // ✅ GOOD: Working with nullable collections
    public static void ProcessNames(List<string?> names)
    {
        foreach (var name in names)
        {
            if (name != null) // ✅ Must check each item
            {
                Console.WriteLine(name.ToUpper());
            }
        }
    }

    // ✅ GOOD: LINQ with nullable items
    public static List<string> GetValidNames(List<string?> names)
    {
        return names
            .Where(n => n != null) // ✅ Filter nulls
            .Select(n => n!) // ✅ Tell compiler we filtered
            .ToList();
    }

    // ✅ BETTER: Use OfType to filter nulls
    public static List<string> GetValidNamesBetter(List<string?> names)
    {
        return names
            .OfType<string>() // ✅ Filters nulls AND casts
            .ToList();
    }

    // ✅ GOOD: Nullable dictionary lookups
    public static string GetUserName(Dictionary<int, User?> cache, int id)
    {
        if (cache.TryGetValue(id, out var user) && user != null)
        {
            return user.Name;
        }
        return "Unknown";
    }
}

/// <summary>
/// EXAMPLE 5: NULLABLE ATTRIBUTES FOR ADVANCED SCENARIOS
/// 
/// THE PROBLEM:
/// Sometimes you need to give compiler more information about your null handling.
/// Custom validation methods, out parameters, etc.
/// 
/// THE SOLUTION:
/// Use nullable attributes to document null behavior.
/// 
/// WHY IT MATTERS:
/// - API library authors need this
/// - Helps flow analysis understand custom patterns
/// - Better warnings for API consumers
/// 
/// ATTRIBUTES:
/// - [NotNull] - Ensures output is non-null
/// - [NotNullWhen(true)] - Parameter is non-null when method returns true
/// - [MaybeNull] - Might return null even if return type is non-nullable
/// - [AllowNull] - Can pass null even if parameter is non-nullable
/// </summary>
public static class NullableAttributeExamples
{
    // ✅ GOOD: NotNullWhen - common for TryGet patterns
    public static bool TryGetUser(int id, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out User? user)
    {
        if (id > 0)
        {
            user = new User { Id = id, Name = "Test" };
            return true;
        }
        user = null;
        return false;
    }

    public static void UseWithFlowAnalysis(int id)
    {
        if (TryGetUser(id, out var user))
        {
            // ✅ Compiler knows 'user' is non-null here thanks to [NotNullWhen(true)]
            Console.WriteLine(user.Name); // No warning
        }
    }

    // ✅ GOOD: MaybeNull for generic constraints
    public static T GetValueOrDefault<T>(Dictionary<string, T> dict, string key)
    {
        return dict.TryGetValue(key, out var value) ? value : default!;
        // default(T) might be null for reference types
    }

    // ✅ GOOD: DoesNotReturn for throw helpers
    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    public static void ThrowIfNull(object? value, string paramName)
    {
        if (value == null)
            throw new ArgumentNullException(paramName);
    }

    public static string UseThrowHelper(string? input)
    {
        ThrowIfNull(input, nameof(input));
        // ✅ Compiler knows we don't return if null
        return input.ToUpper(); // No warning
    }
}

/// <summary>
/// EXAMPLE 6: MIGRATION STRATEGIES
/// 
/// THE PROBLEM:
/// Existing codebase has thousands of potential null warnings.
/// Can't fix everything at once.
/// 
/// THE SOLUTION:
/// Gradual migration with different nullable contexts.
/// 
/// WHY IT MATTERS:
/// - Enable in new code without breaking old code
/// - Improve incrementally
/// - Team can adopt at their own pace
/// 
/// STRATEGIES:
/// 1. Start with #nullable enable in new files
/// 2. Enable warnings only (#nullable enable warnings)
/// 3. Enable per-project gradually
/// 4. Use #nullable restore to interop
/// </summary>
public static class MigrationExamples
{
    // STRATEGY 1: Per-file opt-in (already at top of file)
    // #nullable enable

    // STRATEGY 2: Project-level in .csproj
    // <PropertyGroup>
    //   <Nullable>enable</Nullable>  <!-- OR -->
    //   <Nullable>warnings</Nullable>  <!-- Warnings only -->
    //   <Nullable>annotations</Nullable>  <!-- Annotations only -->
    // </PropertyGroup>

    // STRATEGY 3: Suppress specific warnings when migrating
#nullable disable warnings
    public static string LegacyMethod(string input)
    {
        // No warnings here during migration
        return input.ToUpper();
    }
#nullable restore warnings

    // STRATEGY 4: Interop with non-nullable-aware code
    public class InteropExample
    {
        // ✅ When calling legacy library that returns T
        public string? GetLegacyData()
        {
            // Assume legacy method returns string, could be null
            var result = LegacyLibrary.GetData();
            return result; // Treat as nullable
        }

        // ✅ When implementing interface from legacy library
        public class ModernImplementation : ILegacyInterface
        {
            // Legacy interface has non-nullable string, but implementation knows it can be null
            public string GetValue() => null!; // Use null-forgiving during migration
        }
    }
}

/// <summary>
/// EXAMPLE 7: COMMON PATTERNS AND BEST PRACTICES
/// 
/// THE PROBLEM:
/// How to apply nullable reference types to common scenarios.
/// 
/// THE SOLUTION:
/// Patterns for DTOs, entities, constructors, properties, etc.
/// 
/// WHY IT MATTERS:
/// - Consistency across codebase
/// - Best practices for common scenarios
/// </summary>
public static class CommonPatterns
{
    // ✅ PATTERN 1: DTOs with required properties
    public record CreateUserRequest
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; } // Optional
    }

    // ✅ PATTERN 2: Entity with nullable navigation properties
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        // Nullable - might not be loaded
        public Customer? Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    // ✅ PATTERN 3: Constructor ensuring non-null
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger _logger;

        public UserService(IUserRepository repository, ILogger logger)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(logger);
#else
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
#endif

            _repository = repository;
            _logger = logger;
        }
    }

    // ✅ PATTERN 4: String properties - never null
    public class Product
    {
        public string Name { get; set; } = string.Empty; // Never null, default to empty
        public string? Description { get; set; } // Can be null
    }

    // ✅ PATTERN 5: Async methods returning nullable
    public static async Task<User?> FindUserAsync(int id)
    {
        await Task.Delay(10); // Simulate DB call
        return id > 0 ? new User { Id = id, Name = "Test" } : null;
    }

    // ✅ PATTERN 6: String.IsNullOrEmpty/IsNullOrWhiteSpace
    public static bool ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // ✅ Compiler knows name is non-null here
        return name.Length >= 3;
    }
}

// Supporting types
public interface ILogger
{
    void LogInformation(string message);
}

public interface ICache
{
    object? Get(string key);
    void Set(string key, object value);
}

public class MemoryCache : ICache
{
    public object? Get(string key) => null;
    public void Set(string key, object value) { }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public interface IUserRepository
{
    User? FindById(int id);
}

public interface ILegacyInterface
{
    string GetValue();
}

public static class LegacyLibrary
{
    public static string GetData() => "legacy";
}

#nullable restore // Restore nullable context to project default
