# Validation Patterns

> Subject: [Practical-Patterns](../README.md)

## Validation Patterns

### Data Annotations

```csharp
// ✅ Simple property validation
public class CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Range(18, 120)]
    public int Age { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
}

// Controller automatically validates
[HttpPost]
public IActionResult Create([FromBody] CreateUserRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // request is valid
}
```

### FluentValidation

```csharp
// ✅ Complex validation logic
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be positive");
        
        RuleFor(x => x.OrderDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Order date cannot be in the past");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must have at least one item");
        
        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator());
        
        // Custom rule
        RuleFor(x => x.Total)
            .Must((order, total) => total == order.Items.Sum(i => i.Price * i.Quantity))
            .WithMessage("Total must equal sum of item prices");
    }
}

// Usage
var validator = new CreateOrderRequestValidator();
var result = await validator.ValidateAsync(request);

if (!result.IsValid)
{
    return BadRequest(result.Errors);
}
```

---


