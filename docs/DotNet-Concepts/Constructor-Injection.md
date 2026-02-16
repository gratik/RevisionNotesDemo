# Constructor Injection

> Subject: [DotNet-Concepts](../README.md)

## Constructor Injection

### Best Practice

`csharp
// ✅ GOOD: Constructor injection (most common)
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(
        IOrderRepository repository,
        IEmailService emailService,
        ILogger<OrderService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }
}
`

### Required vs Optional Dependencies

`csharp
// ✅ Required dependencies in constructor
public class UserService
{
    private readonly IUserRepository _repository;  // Required
    
    public UserService(IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
}

// ✅ Optional dependencies via property or method
public class NotificationService
{
    public ILogger? Logger { get; set; }  // Optional
    
    public void Send(string message)
    {
        Logger?.LogInformation("Sending: {Message}", message);
    }
}
`

---

## Detailed Guidance

Constructor Injection guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Constructor Injection before implementation work begins.
- Keep boundaries explicit so Constructor Injection decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Constructor Injection in production-facing code.
- When performance, correctness, or maintainability depends on consistent Constructor Injection decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Constructor Injection as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Constructor Injection is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Constructor Injection are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

