// ============================================================================
// KISS (Keep It Simple, Stupid), DRY (Don't Repeat Yourself), YAGNI (You Aren't Gonna Need It)
// Reference: Revision Notes - Page 2
// ============================================================================
// 
// KISS - KEEP IT SIMPLE, STUPID
// ==============================
// DEFINITION:
//   "Simplicity should be a key goal; avoid unnecessary complexity."
//   Choose the simplest solution that works.
//
// EXPLANATION:
//   Complex code is harder to understand, maintain, test, and debug. Simple code is
//   more maintainable and less bug-prone. Favor readability over cleverness.
//
// BENEFITS:
//   • Easier to understand and maintain
//   • Fewer bugs
//   • Easier to test
//   • Faster onboarding for new developers
//
// GUIDELINES:
//   • Favor readability over cleverness
//   • Use clear naming
//   • Break complex logic into smaller methods
//   • Avoid premature optimization
//   • Write code for humans, not machines
//
// 
// DRY - DON'T REPEAT YOURSELF
// ===========================
// DEFINITION:
//   "Every piece of knowledge should have a single, authoritative representation."
//   Avoid duplicating logic.
//
// EXPLANATION:
//   When you need to change something, you should only need to change it in one place.
//   Duplication leads to maintenance nightmares - fixing bugs in multiple places.
//
// BENEFITS:
//   • Single source of truth
//   • Easier maintenance
//   • Reduces bugs (fix once, fixed everywhere)
//   • Better consistency
//
// GUIDELINES:
//   • Extract common code into methods/classes
//   • Use inheritance or composition appropriately
//   • Create reusable utilities
//   • Avoid copy-paste programming
//   • Use constants for magic numbers/strings
//
//
// YAGNI - YOU AREN'T GONNA NEED IT
// =================================
// DEFINITION:
//   "Don't implement something until it's actually needed."
//   Don't add functionality speculatively.
//
// EXPLANATION:
//   Adding features "just in case" adds complexity and maintenance burden for features
//   that may never be used. Build what's needed now; refactor when requirements change.
//
// BENEFITS:
//   • Reduces complexity
//   • Faster development
//   • Less code to maintain
//   • Focus on actual requirements
//
// GUIDELINES:
//   • Build what's needed now
//   • Refactor when requirements actually change
//   • Avoid speculative generality
//   • Focus on current use cases
//   • Let requirements drive features
//
// REAL-WORLD ANALOGY:
//   Don't build a 10-bedroom mansion when you need a 2-bedroom house.
//
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ============================================================================
// KISS (Keep It Simple, Stupid)
// ============================================================================

// ❌ BAD - Overly complex
public class ComplexDateChecker
{
    public bool IsWeekendComplex(DateTime date)
    {
        // Unnecessarily complex
        var dayOfWeek = date.DayOfWeek;
        var isWeekend = false;

        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                isWeekend = false;
                break;
            case DayOfWeek.Tuesday:
                isWeekend = false;
                break;
            case DayOfWeek.Wednesday:
                isWeekend = false;
                break;
            case DayOfWeek.Thursday:
                isWeekend = false;
                break;
            case DayOfWeek.Friday:
                isWeekend = false;
                break;
            case DayOfWeek.Saturday:
                isWeekend = true;
                break;
            case DayOfWeek.Sunday:
                isWeekend = true;
                break;
        }

        return isWeekend;
    }
}

// ✅ GOOD - Simple and clear (KISS)
public class SimpleDateChecker
{
    public bool IsWeekend(DateTime date)
    {
        // Simple, direct, and clear
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}

// ============================================================================
// DRY (Don't Repeat Yourself)
// ============================================================================

// ❌ BAD - Code repetition
public class UserServiceWithRepetition
{
    public void CreateUser(string name, string email)
    {
        // Validation repeated
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");
        if (!email.Contains("@"))
            throw new ArgumentException("Email must be valid");

        Console.WriteLine($"Creating user: {name}, {email}");
    }

    public void UpdateUser(int id, string name, string email)
    {
        // Same validation repeated!
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");
        if (!email.Contains("@"))
            throw new ArgumentException("Email must be valid");

        Console.WriteLine($"Updating user {id}: {name}, {email}");
    }
}

// ✅ GOOD - DRY principle applied
public class UserServiceDRY
{
    // Single validation method - reused everywhere
    private void ValidateUserData(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");
        if (!email.Contains("@"))
            throw new ArgumentException("Email must be valid");
    }

    public void CreateUser(string name, string email)
    {
        ValidateUserData(name, email); // Reuse validation
        Console.WriteLine($"[DRY] Creating user: {name}, {email}");
    }

    public void UpdateUser(int id, string name, string email)
    {
        ValidateUserData(name, email); // Reuse validation
        Console.WriteLine($"[DRY] Updating user {id}: {name}, {email}");
    }

    public void ImportUser(string name, string email)
    {
        ValidateUserData(name, email); // Reuse validation
        Console.WriteLine($"[DRY] Importing user: {name}, {email}");
    }
}

// ============================================================================
// YAGNI (You Aren't Gonna Need It)
// ============================================================================

// ❌ BAD - Feature bloat
public class OverEngineeredCalculator
{
    // All these might never be needed!
    public double Add(double a, double b) => a + b;
    public double Subtract(double a, double b) => a - b;
    public double Multiply(double a, double b) => a * b;
    public double Divide(double a, double b) => a / b;

    // YAGNI violations - unnecessary features
    public double Power(double baseNum, double exponent) => Math.Pow(baseNum, exponent);
    public double SquareRoot(double num) => Math.Sqrt(num);
    public double Logarithm(double num) => Math.Log(num);
    public double Sine(double angle) => Math.Sin(angle);
    public double Cosine(double angle) => Math.Cos(angle);
    public double Tangent(double angle) => Math.Tan(angle);

    // Even more unnecessary complexity
    public double BinaryToDecimal(string binary) { /* ... */ return 0; }
    public string DecimalToBinary(double num) { /* ... */ return ""; }
    public double Factorial(int n) { /* ... */ return 0; }
    // ... and 20 more methods that might never be used!
}

// ✅ GOOD - YAGNI principle: Only what's needed
public class SimpleCalculator
{
    // Start with only what you actually need
    public double Add(double a, double b)
    {
        Console.WriteLine($"[YAGNI] Adding {a} + {b}");
        return a + b;
    }

    public double Subtract(double a, double b)
    {
        Console.WriteLine($"[YAGNI] Subtracting {a} - {b}");
        return a - b;
    }

    // Add more methods ONLY when actually needed!
    // Don't add features based on speculation
}

// ============================================================================
// Combined Example: KISS + DRY + YAGNI
// ============================================================================

public class OrderProcessor
{
    // KISS: Simple, straightforward logic
    // DRY: Reusable validation
    // YAGNI: Only essential features

    private void ValidateOrder(decimal amount)
    {
        // DRY: Single validation method
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");
    }

    public void ProcessOrder(decimal amount)
    {
        // KISS: Simple flow
        ValidateOrder(amount); // DRY: Reuse validation

        decimal tax = CalculateTax(amount);
        decimal total = amount + tax;

        Console.WriteLine($"[KISS+DRY+YAGNI] Order: ${amount:F2}, Tax: ${tax:F2}, Total: ${total:F2}");
    }

    private decimal CalculateTax(decimal amount)
    {
        // KISS: Simple calculation
        // YAGNI: No complex tax rules until actually needed
        return amount * 0.1m; // 10% flat tax
    }

    // YAGNI: Don't add features like:
    // - Multiple currency support (until needed)
    // - Multiple tax jurisdictions (until needed)
    // - Discount calculations (until needed)
    // - Loyalty points (until needed)
}

// Usage demonstration
public class KISSDRYYAGNIDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== KISS, DRY, YAGNI PRINCIPLES DEMO ===\n");

        // KISS Demo
        Console.WriteLine("--- KISS (Keep It Simple, Stupid) ---");
        var dateChecker = new SimpleDateChecker();
        Console.WriteLine($"[KISS] Is today weekend? {dateChecker.IsWeekend(DateTime.Now)}");
        Console.WriteLine("[KISS] Benefit: Simple code is easier to understand and maintain!\n");

        // DRY Demo
        Console.WriteLine("--- DRY (Don't Repeat Yourself) ---");
        var userService = new UserServiceDRY();
        userService.CreateUser("Alice", "alice@example.com");
        userService.UpdateUser(1, "Alice Smith", "alice@example.com");
        Console.WriteLine("[DRY] Benefit: Validation logic in one place - easy to maintain!\n");

        // YAGNI Demo
        Console.WriteLine("--- YAGNI (You Aren't Gonna Need It) ---");
        var calculator = new SimpleCalculator();
        calculator.Add(5, 3);
        calculator.Subtract(10, 4);
        Console.WriteLine("[YAGNI] Benefit: Clean, focused code without speculative features!\n");

        // Combined Demo
        Console.WriteLine("--- KISS + DRY + YAGNI Combined ---");
        var orderProcessor = new OrderProcessor();
        orderProcessor.ProcessOrder(100m);
        Console.WriteLine("\nBenefit: Simple, maintainable, focused code!");
        Console.WriteLine("From Revision Notes:");
        Console.WriteLine("- KISS: Simplicity is efficiency");
        Console.WriteLine("- DRY: Logic in one place avoids errors");
        Console.WriteLine("- YAGNI: Focus on what's necessary, save time and keep code clean");
    }
}
