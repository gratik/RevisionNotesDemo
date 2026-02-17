# Validation Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core API/service development skills and dependency injection familiarity.
- Related examples: docs/Practical-Patterns/README.md
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


## Interview Answer Block
30-second answer:
- Validation Patterns is about high-value implementation patterns for day-to-day engineering. It matters because practical patterns reduce repeated design mistakes.
- Use it when standardizing common cross-cutting behaviors in services.

2-minute answer:
- Start with the problem Validation Patterns solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: pattern reuse vs context-specific customization needs.
- Close with one failure mode and mitigation: copying patterns without validating fit for the current problem.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Validation Patterns but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Validation Patterns, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Validation Patterns and map it to one concrete implementation in this module.
- 3 minutes: compare Validation Patterns with an alternative, then walk through one failure mode and mitigation.