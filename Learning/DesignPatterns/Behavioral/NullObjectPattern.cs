// ==============================================================================
// NULL OBJECT PATTERN - Replace Null Checks with Do-Nothing Object
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE NULL OBJECT PATTERN?
// ---------------------------------
// Provides an object with neutral "do nothing" behavior to represent absence of a
// real object. Instead of using null and checking for it everywhere, use a Null Object
// that implements the interface but does nothing. Eliminates null reference checks.
//
// Think of it as: "Optional class - when student doesn't have email, instead of null,
// use NoEmail object that silently ignores Send() calls. No if(email != null) needed!"
//
// Core Concepts:
//   • Interface: Common contract for real and null objects
//   • Real Object: Normal implementation with behavior
//   • Null Object: Implements interface but does nothing (safe defaults)
//   • Seamless Substitution: Client doesn't know which type it has
//   • Eliminate Conditionals: No null checks needed
//
// WHY IT MATTERS
// --------------
// ✅ NO NULL CHECKS: Eliminate if(obj != null) clutter
// ✅ NO NULLREFERENCEEXCEPTION: Safe to call methods always
// ✅ SIMPLIFIED CODE: Client code cleaner, more readable
// ✅ POLYMORPHISM: Null Object follows same interface
// ✅ DEFAULT BEHAVIOR: Explicit "do nothing" implementation
// ✅ TESTABILITY: Easier to test without mocking null scenarios
//
// WHEN TO USE IT
// --------------
// ✅ Null checks scattered throughout code
// ✅ Default "do nothing" behavior makes sense
// ✅ Want to avoid NullReferenceException
// ✅ Client shouldn't need to know about absence
// ✅ Optional dependencies (logger, notification, cache)
// ✅ Default implementations (no-op strategy)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Null means something important (error condition)
// ❌ Need to distinguish between null and "do nothing"
// ❌ C# nullable reference types handle it well enough
// ❌ Action is required (can't be no-op)
//
// REAL-WORLD EXAMPLE - Logging System
// -----------------------------------
// Application with optional logging:
//   • Production: Log to file
//   • Testing: Log to console
//   • Performance testing: No logging (don't slow down tests)
//
// WITHOUT NULL OBJECT:
//   ❌ class UserService {
//         private readonly ILogger? _logger;
//         
//         public void CreateUser(string username) {
//             if (_logger != null)  // ❌ Check 1
//                 _logger.Log($"Creating user: {username}");
//             
//             // Create user...
//             
//             if (_logger != null)  // ❌ Check 2
//                 _logger.Log($"User created: {username}");
//             
//             // More operations...
//             
//             if (_logger != null)  // ❌ Check 3
//                 _logger.Log("Operation complete");
//         }
//         
//         public void DeleteUser(int id) {
//             if (_logger != null)  // ❌ Check 4
//                 _logger.Log($"Deleting user: {id}");
//             // ...
//         }
//     }
//   ❌ if (logger != null) everywhere!
//   ❌ Easy to forget null check → NullReferenceException
//   ❌ 100 log statements = 100 null checks
//
// WITH NULL OBJECT:
//   ✅ interface ILogger {
//         void Log(string message);
//     }
//   
//   ✅ class FileLogger : ILogger {
//         public void Log(string message) {
//             File.AppendAllText("app.log", $"{DateTime.Now}: {message}\n");
//         }
//     }
//   
//   ✅ class NullLogger : ILogger {  // Null Object!
//         public void Log(string message) {
//             // Do nothing - performance testing mode
//         }
//     }
//   
//   ✅ class UserService {
//         private readonly ILogger _logger;  // Never null!
//         
//         public UserService(ILogger logger) {
//             _logger = logger;  // Guaranteed non-null
//         }
//         
//         public void CreateUser(string username) {
//             _logger.Log($"Creating user: {username}");  // ✅ No null check!
//             // Create user...
//             _logger.Log($"User created: {username}");   // ✅ No null check!
//             // More operations...
//             _logger.Log("Operation complete");          // ✅ No null check!
//         }
//         
//         public void DeleteUser(int id) {
//             _logger.Log($"Deleting user: {id}");        // ✅ No null check!
//             // ...
//         }
//     }
//   
//   ✅ Usage:
//     // Production:
//     var service = new UserService(new FileLogger());
//     
//     // Performance testing:
//     var service = new UserService(new NullLogger());  // Zero logging overhead!
//     
//     // No null checks anywhere in code!
//
// ANOTHER EXAMPLE - Email Notification
// ------------------------------------
// Send notifications, but sometimes users opt out:
//   interface INotificationService {
//       void SendEmail(string to, string subject, string body);
//   }
//   
//   class EmailService : INotificationService {
//       public void SendEmail(string to, string subject, string body) {
//           // Actually send email via SMTP
//       }
//   }
//   
//   class NullNotificationService : INotificationService {
//       public void SendEmail(string to, string subject, string body) {
//           // User opted out - do nothing
//       }
//   }
//   
//   class OrderService {
//       private readonly INotificationService _notification;
//       
//       public void PlaceOrder(Order order) {
//           // Process order...
//           _notification.SendEmail(  // ✅ Always safe to call
//               order.Customer.Email,
//               "Order Confirmation",
//               $"Your order #{order.Id} is confirmed"
//           );
//       }
//   }
//   
//   // User with notifications:
//   var service = new OrderService(new EmailService());
//   
//   // User opted out:
//   var service = new OrderService(new NullNotificationService());
//
// ANOTHER EXAMPLE - Caching
// -------------------------
// Optional caching for development vs production:
//   interface ICache {
//       T? Get<T>(string key);
//       void Set<T>(string key, T value);
//   }
//   
//   class RedisCache : ICache { /* real Redis implementation */ }
//   
//   class NullCache : ICache {  // No caching in development
//       public T? Get<T>(string key) => default;  // Always cache miss
//       public void Set<T>(string key, T value) { }  // Don't store
//   }
//   
//   class ProductService {
//       private readonly ICache _cache;
//       
//       public Product GetProduct(int id) {
//           var cached = _cache.Get<Product>($"product:{id}");
//           if (cached != null) return cached;
//           
//           var product = _database.GetProduct(id);
//           _cache.Set($"product:{id}", product);
//           return product;
//       }
//   }
//   
//   // Production:
//   services.AddSingleton<ICache>(new RedisCache());
//   
//   // Development (no caching needed):
//   services.AddSingleton<ICache>(new NullCache());
//
// NULL OBJECT WITH OPTIONAL
// -------------------------
// Modern C# alternative using Optional<T> type:
//   Optional<ILogger>  logger = Optional.None; // Like Null Object
//   logger.IfPresent(l => l.Log("message")); // Only calls if present
//
// But Null Object is often clearer:
//   ILogger logger = new NullLogger(); // Explicit
//   logger.Log("message"); // Always safe
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Null Object pattern in .NET:
//   • Stream.Null: Discards all writes (like /dev/null)
//   • TextWriter.Null: No-op text writer
//   • CancellationToken.None: Non-cancellable token
//   • Task.CompletedTask: Already-completed task
//
// Example:
//   class DataProcessor {
//       public async Task Process(Stream output) {
//           await output.WriteAsync(data);  // Safe even if Stream.Null
//       }
//   }
//   
//   // Production:
//   await processor.Process(fileStream);
//   
//   // Testing (discard output):
//   await processor.Process(Stream.Null);  // Null Object!
//
// EMPTY COLLECTIONS AS NULL OBJECTS
// ---------------------------------
// Instead of returning null for empty lists:
//   ❌ public List<Product>? GetProducts(string category) {
//         var products = _db.Products.Where(p => p.Category == category);
//         return products.Any() ? products.ToList() : null;  // ❌ Returns null
//     }
//   
//     var products = GetProducts("Electronics");
//     if (products != null) {  // ❌ Null check needed
//         foreach (var p in products) { ... }
//     }
//
//   ✅ public List<Product> GetProducts(string category) {
//         return _db.Products
//                   .Where(p => p.Category == category)
//                   .ToList();  // ✅ Empty list, not null
//     }
//   
//     var products = GetProducts("Electronics");
//     foreach (var p in products) {  // ✅ Safe - empty loop if no products
//         // ...
//     }
//
// SINGLETON FOR NULL OBJECTS
// --------------------------
// Null Objects are often singletons (immutable, stateless):
//   class NullLogger : ILogger {
//       public static readonly NullLogger Instance = new NullLogger();
//       private NullLogger() { }  // Prevent instantiation
//       public void Log(string message) { }
//   }
//   
//   // Usage:
//   ILogger logger = NullLogger.Instance;  // Reuse single instance
//
// BEST PRACTICES
// --------------
// ✅ Use for optional dependencies (logger, cache, notification)
// ✅ Make Null Object immutable and stateless
// ✅ Consider singleton pattern for Null Objects
// ✅ Use descriptive names: NullLogger, NoOpCache, DiscardStream
// ✅ Return empty collections instead of null
// ✅ Document that Null Object is a valid option
// ✅ Prefer Null Object over nullable types when safe defaults exist
//
// NULL OBJECT VS NULLABLE REFERENCE TYPES
// ---------------------------------------
// C# 8+ Nullable Reference Types:
//   ILogger? logger = null;  // Compiler warns about null
//   logger?.Log("message"); // Safe navigation
//
// Null Object Pattern:
//   ILogger logger = new NullLogger();  // Never null
//   logger.Log("message"); // Always safe, no ?
//
// Choose Nullable when:
//   • Null is meaningful (error, missing data)
//   • Explicit checks needed
//
// Choose Null Object when:
//   • Safe default behavior exists
//   • Eliminate null checks
//   • Optional dependencies
//
// NULL OBJECT VS SIMILAR PATTERNS
// -------------------------------
// Null Object vs Strategy:
//   • Null Object: Specific case ("do nothing" strategy)
//   • Strategy: General purpose algorithm selection
//
// Null Object vs Proxy:
//   • Null Object: Removes need for checks, provides defaults
//   • Proxy: Controls access, adds behavior
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// ========================================================================
// WITHOUT NULL OBJECT PATTERN (BAD - lots of null checks)
// ========================================================================

public interface ILogger_Bad
{
    void Log(string message);
}

public class FileLogger_Bad : ILogger_Bad
{
    public void Log(string message)
    {
        Console.WriteLine($"[FILE] {message}");
    }
}

public class UserService_Bad
{
    private readonly ILogger_Bad? _logger;

    public UserService_Bad(ILogger_Bad? logger)
    {
        _logger = logger;
    }

    public void CreateUser(string username)
    {
        if (_logger != null)  // ❌ Null check required
            _logger.Log($"Creating user: {username}");

        // Business logic...

        if (_logger != null)  // ❌ Another null check
            _logger.Log($"User created: {username}");
    }
}

// ========================================================================
// WITH NULL OBJECT PATTERN (GOOD - no null checks)
// ========================================================================

public interface ILogger
{
    void Log(string message);
    void LogError(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[CONSOLE] ✅ {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[CONSOLE] ❌ ERROR: {message}");
    }
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[FILE] 📄 {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[FILE] 📄 ERROR: {message}");
    }
}

/// <summary>
/// Null Object - provides "do nothing" behavior
/// </summary>
public class NullLogger : ILogger
{
    public void Log(string message)
    {
        // Do nothing - silent
    }

    public void LogError(string message)
    {
        // Do nothing - silent
    }
}

public class UserService
{
    private readonly ILogger _logger;

    public UserService(ILogger logger)
    {
        _logger = logger;  // No null check needed - always have a logger
    }

    public void CreateUser(string username)
    {
        _logger.Log($"Creating user: {username}");  // ✅ No null check needed

        // Business logic...
        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogError("Username cannot be empty");
            return;
        }

        _logger.Log($"User created: {username}");  // ✅ No null check needed
    }

    public void DeleteUser(int userId)
    {
        _logger.Log($"Deleting user: {userId}");
        // Business logic...
        _logger.Log($"User {userId} deleted");
    }
}

// ========================================================================
// ANOTHER EXAMPLE - Payment Processing
// ========================================================================

public interface IPaymentGateway
{
    bool ProcessPayment(decimal amount);
    void RefundPayment(string transactionId);
}

public class StripePaymentGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[STRIPE] 💳 Processing ${amount:F2}...");
        return true;
    }

    public void RefundPayment(string transactionId)
    {
        Console.WriteLine($"[STRIPE] ↩️  Refunding transaction {transactionId}");
    }
}

/// <summary>
/// Null Object for testing or when payment is disabled
/// </summary>
public class NullPaymentGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount)
    {
        // Do nothing - no actual payment processed
        return true;  // Returns success to not break flow
    }

    public void RefundPayment(string transactionId)
    {
        // Do nothing - no actual refund
    }
}

public class CheckoutService
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger _logger;

    public CheckoutService(IPaymentGateway paymentGateway, ILogger logger)
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
    }

    public void Checkout(string customerName, decimal amount)
    {
        _logger.Log($"Starting checkout for {customerName}...");

        bool success = _paymentGateway.ProcessPayment(amount);

        if (success)
            _logger.Log($"✅ Checkout completed for {customerName}");
        else
            _logger.LogError($"❌ Payment failed for {customerName}");
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class NullObjectDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== NULL OBJECT PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // BAD: Without Null Object Pattern
        Console.WriteLine("--- ❌ WITHOUT Null Object Pattern (requires null checks) ---");
        var badService = new UserService_Bad(null);  // Null logger
        Console.WriteLine("[BAD] Creating user with null logger (no output, but code needs null checks)");
        badService.CreateUser("john_doe");
        Console.WriteLine();

        // GOOD: With Null Object Pattern
        Console.WriteLine("--- ✅ WITH Null Object Pattern ---\n");

        // Scenario 1: Real logger
        Console.WriteLine("Scenario 1: With ConsoleLogger");
        var service1 = new UserService(new ConsoleLogger());
        service1.CreateUser("alice");
        service1.DeleteUser(123);
        Console.WriteLine();

        // Scenario 2: Null logger (no output, no null checks needed!)
        Console.WriteLine("Scenario 2: With NullLogger (silent mode)");
        var service2 = new UserService(new NullLogger());
        Console.WriteLine("[DEMO] Note: Operations execute but produce no log output");
        service2.CreateUser("bob");
        service2.DeleteUser(456);
        Console.WriteLine("[DEMO] Operations completed silently ✅\n");

        // Scenario 3: Different logger implementation
        Console.WriteLine("Scenario 3: With FileLogger");
        var service3 = new UserService(new FileLogger());
        service3.CreateUser("charlie");
        Console.WriteLine();

        // Scenario 4: Payment gateway example
        Console.WriteLine("--- Payment Gateway Example ---\n");

        Console.WriteLine("With real payment gateway:");
        var checkout1 = new CheckoutService(new StripePaymentGateway(), new ConsoleLogger());
        checkout1.Checkout("Diana", 99.99m);
        Console.WriteLine();

        Console.WriteLine("With null payment gateway (testing mode):");
        var checkout2 = new CheckoutService(new NullPaymentGateway(), new ConsoleLogger());
        checkout2.Checkout("Eve", 149.99m);
        Console.WriteLine("[DEMO] No actual payment processed - perfect for testing!\n");

        // Scenario 5: Silent mode (both null objects)
        Console.WriteLine("Silent mode (NullLogger + NullPaymentGateway):");
        var checkout3 = new CheckoutService(new NullPaymentGateway(), new NullLogger());
        Console.WriteLine("[DEMO] Running checkout in silent mode...");
        checkout3.Checkout("Frank", 199.99m);
        Console.WriteLine("[DEMO] Completed silently ✅\n");

        Console.WriteLine("💡 Null Object Pattern Benefits:");
        Console.WriteLine("   ✅ Eliminates null checks - cleaner code");
        Console.WriteLine("   ✅ Prevents NullReferenceException");
        Console.WriteLine("   ✅ Polymorphic - Null Object implements same interface");
        Console.WriteLine("   ✅ Perfect for testing - disable external dependencies");
        Console.WriteLine("   ✅ Default behavior - graceful degradation");
        Console.WriteLine("   ✅ Follows OCP - add null behavior without changing clients");

        Console.WriteLine("\n💡 When to Use:");
        Console.WriteLine("   ✅ Optional dependencies (logging, analytics, notifications)");
        Console.WriteLine("   ✅ Testing scenarios (disable external services)");
        Console.WriteLine("   ✅ Default behavior instead of null");
        Console.WriteLine("   ✅ Avoiding repetitive null checks");
    }
}