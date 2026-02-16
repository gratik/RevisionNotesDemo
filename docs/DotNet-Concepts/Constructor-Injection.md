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


