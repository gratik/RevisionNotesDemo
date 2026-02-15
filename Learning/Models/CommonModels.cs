// ============================================================================
// COMMON MODELS
// Reference: Revision Notes - Various Pages
// ============================================================================
// WHAT IS THIS?
// -------------
// A shared model catalog for entities, DTOs, value objects, and response
// patterns reused across architecture, API, data, and testing examples.
//
// WHY IT MATTERS
// --------------
// Consistent model semantics reduce accidental drift between examples and make
// cross-topic demonstrations easier to understand and extend.
//
// WHEN TO USE
// -----------
// Use these models when a demo needs representative business data structures
// without redefining domain contracts in every topic folder.
//
// PURPOSE: Shared models and DTOs used across multiple demonstrations
// ============================================================================

namespace RevisionNotesDemo.Models;

// ============================================================================
// DOMAIN MODELS - Represent business entities
// ============================================================================

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

// ============================================================================
// DTOs - Data Transfer Objects for API responses
// ============================================================================

public record CustomerDto(
    int Id,
    string Name,
    string Email,
    bool IsActive
);

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    string FormattedPrice,
    bool InStock
);

public record OrderSummaryDto(
    int OrderId,
    string CustomerName,
    int ItemCount,
    decimal TotalAmount,
    string Status,
    DateTime OrderDate
);

// ============================================================================
// REQUEST MODELS - For API commands
// ============================================================================

public record CreateCustomerRequest(
    string Name,
    string Email
);

public record UpdateProductStockRequest(
    int ProductId,
    int Quantity
);

public record PlaceOrderRequest(
    int CustomerId,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(
    int ProductId,
    int Quantity
);

// ============================================================================
// RESPONSE MODELS - API responses
// ============================================================================

public record ApiResponse<T>(
    bool Success,
    T? Data,
    string Message,
    List<string>? Errors = null
);

public record PagedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);

// ============================================================================
// VALUE OBJECTS - Immutable objects defined by their attributes
// ============================================================================

public record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "USD") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public string Formatted => $"{Amount:F2} {Currency}";
}

public record Address(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
)
{
    public string FullAddress => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}

public record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Invalid email address", nameof(email));

        Value = email.Trim().ToLowerInvariant();
    }

    public static implicit operator string(EmailAddress email) => email.Value;
}

// ============================================================================
// RESULT PATTERN - For operation results
// ============================================================================

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default, error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
}

// ============================================================================
// COMMON INTERFACES
// ============================================================================

public interface IEntity
{
    int Id { get; set; }
}

public interface IAuditable
{
    DateTime CreatedDate { get; set; }
    DateTime? ModifiedDate { get; set; }
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
}

// ============================================================================
// DEMONSTRATION
// ============================================================================

public class CommonModelsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== COMMON MODELS DEMO ===\n");

        // Domain Model Example
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            CreatedDate = DateTime.Now,
            IsActive = true
        };
        Console.WriteLine($"[MODEL] Customer: {customer.Name} ({customer.Email})");

        // DTO Example
        var customerDto = new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.IsActive
        );
        Console.WriteLine($"[DTO] CustomerDto: {customerDto}");

        // Value Object Example
        var price = new Money(99.99m, "USD");
        var discount = new Money(10.00m, "USD");
        var finalPrice = price.Add(discount.Amount > 0 ? new Money(-discount.Amount, "USD") : Money.Zero());
        Console.WriteLine($"\n[VALUE_OBJECT] Price: {price.Formatted}");
        Console.WriteLine($"[VALUE_OBJECT] After discount: {finalPrice.Formatted}");

        // Result Pattern Example
        Console.WriteLine("\n[RESULT] Result Pattern:");
        var successResult = Result<int>.Success(42);
        var failureResult = Result<int>.Failure("Operation failed");

        var message1 = successResult.Match(
            onSuccess: value => $"Success! Value: {value}",
            onFailure: error => $"Error: {error}"
        );
        Console.WriteLine($"[RESULT] Success case: {message1}");

        var message2 = failureResult.Match(
            onSuccess: value => $"Success! Value: {value}",
            onFailure: error => $"Error: {error}"
        );
        Console.WriteLine($"[RESULT] Failure case: {message2}");

        Console.WriteLine("\n💡 Common Model Patterns:");
        Console.WriteLine("   ✅ Domain Models - Business entities");
        Console.WriteLine("   ✅ DTOs - Data transfer objects");
        Console.WriteLine("   ✅ Value Objects - Immutable, attribute-based");
        Console.WriteLine("   ✅ Result Pattern - Explicit error handling");
        Console.WriteLine("   ✅ Records - Immutable data structures");
    }
}
