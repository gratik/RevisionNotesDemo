// ============================================================================
// SINGLE RESPONSIBILITY PRINCIPLE (SRP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
// DEFINITION: 
//   "A class should have only one reason to change."
//   Each class should focus on doing ONE thing and doing it well.
//
// EXPLANATION:
//   A class should have a single, well-defined responsibility. If a class has multiple
//   responsibilities, changes to one responsibility may affect the others, making the
//   code fragile and hard to maintain. SRP promotes high cohesion within classes.
//
// EXAMPLE:
//   ❌ BAD: A "Customer" class that handles customer data, database operations, and email sending
//   ✅ GOOD: Separate classes: Customer (data), CustomerRepository (database), EmailService (emails)
//
// REAL-WORLD ANALOGY:
//   A restaurant: The chef cooks, the waiter serves, the cashier handles payments.
//   Each has ONE clear job. Nobody does everything.
//
// BENEFITS:
//   • Easier to understand and maintain
//   • Changes are isolated to single classes
//   • Better testability (focused unit tests)
//   • Improved reusability
//   • Reduced coupling
//
// WHEN TO USE:
//   • Always! This is a fundamental principle
//   • When designing new classes
//   • When refactoring God Objects (classes doing too much)
//   • When you find yourself saying "and" describing class responsibility
//
// COMMON VIOLATIONS:
//   • God Objects (classes that do everything)
//   • Classes with multiple unrelated methods
//   • Mixing business logic with infrastructure concerns
//   • Classes that touch multiple layers (UI + database + business logic)
//
// HOW TO IDENTIFY SRP VIOLATIONS:
//   • Can you describe the class responsibility without using "and"?
//   • Does the class have more than one reason to change?
//   • Are there multiple groups of methods that don't interact?
//   • Would changes in one area affect unrelated functionality?
//
// BEST PRACTICES:
//   • Keep classes small and focused
//   • Use descriptive names that reflect single responsibility
//   • Separate concerns across layers (data, business logic, infrastructure)
//   • Follow cohesion - related methods should be together
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
