// ============================================================================
// SINGLE RESPONSIBILITY PRINCIPLE (SRP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
//
// WHAT IS SRP?
// ------------
// A class should have only one reason to change. A class should do one job
// and do it well. If a class mixes responsibilities, changes in one area
// can break another, making code fragile and hard to maintain.
//
// WHY IT MATTERS
// --------------
// - Changes are isolated to one place
// - Classes are easier to test and understand
// - Fewer merge conflicts and regressions
// - Better reuse with fewer side effects
// - Clearer ownership of behavior
//
// WHEN TO USE
// -----------
// - YES: Always, especially when designing new classes
// - YES: When a class has multiple reasons to change
// - YES: When a class spans multiple layers (UI + data + logic)
// - YES: When you describe a class with "and" in the same sentence
//
// WHEN NOT TO USE
// ---------------
// - NO: Do not split a cohesive responsibility into tiny classes that add
//       indirection without clarity
// - NO: Do not separate responsibilities that must change together
//
// REAL-WORLD EXAMPLE
// ------------------
// Online store:
// - Order class holds order data
// - OrderRepository saves and loads orders
// - OrderEmailService sends confirmation emails
// When the email template changes, only OrderEmailService changes. When the
// database changes, only OrderRepository changes.
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates SRP
// This class has multiple responsibilities: customer data, database operations, and email notifications
public class CustomerBad
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Responsibility 1: Database operations
    public void SaveToDatabase()
    {
        // Database logic here
        Console.WriteLine($"Saving {Name} to database...");
    }

    // Responsibility 2: Email notifications
    public void SendWelcomeEmail()
    {
        // Email logic here
        Console.WriteLine($"Sending welcome email to {Email}...");
    }

    // Responsibility 3: Data validation
    public bool ValidateCustomer()
    {
        return !string.IsNullOrEmpty(Name) && Email.Contains("@");
    }
}

// ✅ GOOD EXAMPLE - Follows SRP
// Each class has a single, well-defined responsibility

// Responsibility: Hold customer data
public class Customer
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

// Responsibility: Handle database operations for customers
public class CustomerRepository
{
    public void Save(Customer customer)
    {
        Console.WriteLine($"[SRP] Saving customer {customer.Name} to database...");
        // Single responsibility: Database operations only
    }

    public Customer? GetById(int id)
    {
        Console.WriteLine($"[SRP] Retrieving customer with ID {id}...");
        return null; // Simplified
    }
}

// Responsibility: Send customer-related emails
public class CustomerEmailService
{
    public void SendWelcomeEmail(Customer customer)
    {
        Console.WriteLine($"[SRP] Sending welcome email to {customer.Email}...");
        // Single responsibility: Email notifications only
    }

    public void SendPasswordResetEmail(Customer customer)
    {
        Console.WriteLine($"[SRP] Sending password reset email to {customer.Email}...");
    }
}

// Responsibility: Validate customer data
public class CustomerValidator
{
    public bool Validate(Customer customer)
    {
        // Single responsibility: Validation logic only
        bool isValid = !string.IsNullOrEmpty(customer.Name) &&
                       customer.Email.Contains("@");

        Console.WriteLine($"[SRP] Customer validation result: {isValid}");
        return isValid;
    }
}

// Usage demonstration
public class SRPDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== SINGLE RESPONSIBILITY PRINCIPLE DEMO ===\n");

        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com"
        };

        // Each service has one job and does it well
        var validator = new CustomerValidator();
        var repository = new CustomerRepository();
        var emailService = new CustomerEmailService();

        if (validator.Validate(customer))
        {
            repository.Save(customer);
            emailService.SendWelcomeEmail(customer);
        }

        Console.WriteLine("\nBenefit: Easy to test, maintain, and modify each responsibility independently!");
    }
}
