// ==============================================================================
// NULL OBJECT PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Provides default "do nothing" behavior, eliminating null checks
// BENEFIT: Removes null checking code, prevents NullReferenceException, simplifies client code
// USE WHEN: Need default behavior instead of null, want to eliminate if(obj != null) checks
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