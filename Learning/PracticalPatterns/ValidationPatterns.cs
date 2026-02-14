// ==============================================================================
// VALIDATION PATTERNS
// Reference: Revision Notes - Practical Scenarios
// ==============================================================================
// PURPOSE: Various approaches to validate data and business rules
// BENEFIT: Data integrity, clear validation logic, reusable validators
// USE WHEN: Need input validation, business rule enforcement, data validation
// ==============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

// ========================================================================
// PATTERN 1: DATA ANNOTATIONS (Simple, built-in)
// ========================================================================

public class RegisterUserRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3-50 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)",
        ErrorMessage = "Password must contain uppercase, lowercase, and number")]
    public string Password { get; set; } = string.Empty;

    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int Age { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? Website { get; set; }
}

public class DataAnnotationsValidator
{
    public static CustomValidationResult Validate(object obj)
    {
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);

        return new CustomValidationResult(isValid, results);
    }
}

// ========================================================================
// PATTERN 2: FLUENT VALIDATION (Expressive, composable)
// ========================================================================

public class CreateProductCommand
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public interface IValidator<T>
{
    CustomValidationResult Validate(T instance);
}

public class CreateProductValidator : IValidator<CreateProductCommand>
{
    public CustomValidationResult Validate(CreateProductCommand command)
    {
        var errors = new List<ValidationResult>();

        // Name validation
        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add(new ValidationResult("Name is required"));
        else if (command.Name.Length < 3 || command.Name.Length > 100)
            errors.Add(new ValidationResult("Name must be 3-100 characters"));

        // Price validation
        if (command.Price <= 0)
            errors.Add(new ValidationResult("Price must be greater than zero"));
        else if (command.Price > 10000)
            errors.Add(new ValidationResult("Price cannot exceed $10,000"));

        // Stock validation
        if (command.Stock < 0)
            errors.Add(new ValidationResult("Stock cannot be negative"));

        // Category validation
        var validCategories = new[] { "Electronics", "Clothing", "Books", "Home", "Sports" };
        if (!validCategories.Contains(command.Category))
            errors.Add(new ValidationResult($"Category must be one of: {string.Join(", ", validCategories)}"));

        bool isValid = !errors.Any();
        return new CustomValidationResult(isValid, isValid ? new List<ValidationResult>() : errors);
    }
}

// ========================================================================
// PATTERN 3: SPECIFICATION PATTERN (Business rules)
// ========================================================================

public class OrderValidationCommand
{
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItemValidation> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
}

public class OrderItemValidation
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public interface IValidationRule<T>
{
    bool IsSatisfied(T entity);
    string ErrorMessage { get; }
}

public class OrderMustHaveItemsRule : IValidationRule<OrderValidationCommand>
{
    public string ErrorMessage => "Order must have at least one item";

    public bool IsSatisfied(OrderValidationCommand order)
    {
        return order.Items.Any();
    }
}

public class OrderTotalMustMatchItemsRule : IValidationRule<OrderValidationCommand>
{
    public string ErrorMessage => "Order total must match sum of item prices";

    public bool IsSatisfied(OrderValidationCommand order)
    {
        var calculatedTotal = order.Items.Sum(i => i.Price * i.Quantity);
        return Math.Abs(order.TotalAmount - calculatedTotal) < 0.01m;
    }
}

public class OrderMustHaveShippingAddressRule : IValidationRule<OrderValidationCommand>
{
    public string ErrorMessage => "Shipping address is required";

    public bool IsSatisfied(OrderValidationCommand order)
    {
        return !string.IsNullOrWhiteSpace(order.ShippingAddress);
    }
}

public class OrderValidator : IValidator<OrderValidationCommand>
{
    private readonly List<IValidationRule<OrderValidationCommand>> _rules;

    public OrderValidator()
    {
        _rules = new List<IValidationRule<OrderValidationCommand>>
        {
            new OrderMustHaveItemsRule(),
            new OrderTotalMustMatchItemsRule(),
            new OrderMustHaveShippingAddressRule()
        };
    }

    public CustomValidationResult Validate(OrderValidationCommand order)
    {
        var errors = new List<ValidationResult>();

        foreach (var rule in _rules)
        {
            if (!rule.IsSatisfied(order))
            {
                errors.Add(new ValidationResult(rule.ErrorMessage));
            }
        }

        bool isValid = !errors.Any();
        return new CustomValidationResult(isValid, errors);
    }
}

// ========================================================================
// CUSTOM VALIDATION RESULT
// ========================================================================

public class CustomValidationResult
{
    public bool IsValid { get; }
    public List<System.ComponentModel.DataAnnotations.ValidationResult> Errors { get; }

    public CustomValidationResult(bool isValid, List<System.ComponentModel.DataAnnotations.ValidationResult> errors)
    {
        IsValid = isValid;
        Errors = errors ?? new List<System.ComponentModel.DataAnnotations.ValidationResult>();
    }

    public CustomValidationResult(bool isValid, string errorMessage)
    {
        IsValid = isValid;
        Errors = isValid
            ? new List<System.ComponentModel.DataAnnotations.ValidationResult>()
            : new List<System.ComponentModel.DataAnnotations.ValidationResult>
            {
                new System.ComponentModel.DataAnnotations.ValidationResult(errorMessage)
            };
    }

    public void DisplayErrors()
    {
        if (IsValid)
        {
            Console.WriteLine("  ✅ Validation passed\n");
        }
        else
        {
            Console.WriteLine("  ❌ Validation failed:");
            foreach (var error in Errors)
            {
                Console.WriteLine($"     • {error.ErrorMessage}");
            }
            Console.WriteLine();
        }
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class ValidationPatternsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== VALIDATION PATTERNS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios\n");

        // Pattern 1: Data Annotations
        Console.WriteLine("=== PATTERN 1: Data Annotations ===\n");
        Console.WriteLine("Built-in attributes for simple validation\n");

        // Valid request
        Console.WriteLine("--- Valid Registration ---");
        var validRequest = new RegisterUserRequest
        {
            Username = "john_doe",
            Email = "john@example.com",
            Password = "SecurePass123",
            Age = 25,
            Website = "https://johndoe.com"
        };

        var result1 = DataAnnotationsValidator.Validate(validRequest);
        result1.DisplayErrors();

        // Invalid request
        Console.WriteLine("--- Invalid Registration ---");
        var invalidRequest = new RegisterUserRequest
        {
            Username = "ab",  // Too short
            Email = "invalid-email",  // Invalid format
            Password = "weak",  // Too short, no uppercase/number
            Age = 15,  // Below minimum
            Website = "not-a-url"  // Invalid URL
        };

        var result2 = DataAnnotationsValidator.Validate(invalidRequest);
        result2.DisplayErrors();

        // Pattern 2: Fluent Validation
        Console.WriteLine("=== PATTERN 2: Fluent Validation ===\n");
        Console.WriteLine("Custom validator with explicit rules\n");

        var productValidator = new CreateProductValidator();

        // Valid product
        Console.WriteLine("--- Valid Product ---");
        var validProduct = new CreateProductCommand
        {
            Name = "Gaming Laptop",
            Price = 1299.99m,
            Stock = 50,
            Category = "Electronics",
            Description = "High-performance gaming laptop"
        };

        var result3 = productValidator.Validate(validProduct);
        result3.DisplayErrors();

        // Invalid product
        Console.WriteLine("--- Invalid Product ---");
        var invalidProduct = new CreateProductCommand
        {
            Name = "ab",  // Too short
            Price = -50,  // Negative
            Stock = -10,  // Negative
            Category = "InvalidCategory"  // Not in allowed list
        };

        var result4 = productValidator.Validate(invalidProduct);
        result4.DisplayErrors();

        // Pattern 3: Specification Pattern (Business Rules)
        Console.WriteLine("=== PATTERN 3: Specification Pattern (Business Rules) ===\n");
        Console.WriteLine("Composable business rule validation\n");

        var orderValidator = new OrderValidator();

        // Valid order
        Console.WriteLine("--- Valid Order ---");
        var validOrder = new OrderValidationCommand
        {
            CustomerId = "CUST-001",
            Items = new List<OrderItemValidation>
            {
                new() { ProductId = "PROD-1", Quantity = 2, Price = 50.00m },
                new() { ProductId = "PROD-2", Quantity = 1, Price = 30.00m }
            },
            TotalAmount = 130.00m,  // 2*50 + 1*30
            ShippingAddress = "123 Main St, City, State 12345"
        };

        var result5 = orderValidator.Validate(validOrder);
        result5.DisplayErrors();

        // Invalid order (no items)
        Console.WriteLine("--- Invalid Order (No Items) ---");
        var emptyOrder = new OrderValidationCommand
        {
            CustomerId = "CUST-002",
            Items = new List<OrderItemValidation>(),
            TotalAmount = 0,
            ShippingAddress = "456 Oak Ave"
        };

        var result6 = orderValidator.Validate(emptyOrder);
        result6.DisplayErrors();

        // Invalid order (total mismatch)
        Console.WriteLine("--- Invalid Order (Total Mismatch) ---");
        var mismatchOrder = new OrderValidationCommand
        {
            CustomerId = "CUST-003",
            Items = new List<OrderItemValidation>
            {
                new() { ProductId = "PROD-1", Quantity = 2, Price = 50.00m }
            },
            TotalAmount = 75.00m,  // Wrong! Should be 100
            ShippingAddress = "789 Pine Rd"
        };

        var result7 = orderValidator.Validate(mismatchOrder);
        result7.DisplayErrors();

        Console.WriteLine("💡 Validation Pattern Comparison:");
        Console.WriteLine("\n🔹 Data Annotations:");
        Console.WriteLine("   ✅ Simple and built-in");
        Console.WriteLine("   ✅ Works with model binding in ASP.NET");
        Console.WriteLine("   ❌ Limited to property-level validation");
        Console.WriteLine("   ❌ Hard to unit test complex rules");

        Console.WriteLine("\n🔹 Fluent Validation (Custom Validators):");
        Console.WriteLine("   ✅ Explicit and readable");
        Console.WriteLine("   ✅ Easy to test");
        Console.WriteLine("   ✅ Supports complex cross-property validation");
        Console.WriteLine("   ✅ Can use external dependencies");

        Console.WriteLine("\n🔹 Specification Pattern:");
        Console.WriteLine("   ✅ Reusable business rules");
        Console.WriteLine("   ✅ Composable (can combine rules)");
        Console.WriteLine("   ✅ Testable in isolation");
        Console.WriteLine("   ✅ Represents domain logic clearly");

        Console.WriteLine("\n💡 Recommendation:");
        Console.WriteLine("   • Simple CRUD: Data Annotations");
        Console.WriteLine("   • Complex validation: Fluent Validation (e.g., FluentValidation library)");
        Console.WriteLine("   • Business rules: Specification Pattern");
        Console.WriteLine("   • Combine patterns for best results");
    }
}