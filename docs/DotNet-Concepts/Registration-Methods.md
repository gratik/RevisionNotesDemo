# Registration Methods

> Subject: [DotNet-Concepts](../README.md)

## Registration Methods

### Basic Registration

`csharp
// ✅ Interface → Implementation
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// ✅ Concrete class only
builder.Services.AddTransient<EmailService>();
`

### Factory Registration

`csharp
// ✅ Factory method for complex creation
builder.Services.AddSingleton<IPaymentService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["Payment:ApiKey"];
    
    return new StripePaymentService(apiKey);
});

// ✅ Choose implementation based on configuration
builder.Services.AddScoped<INotificationService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var type = config["Notifications:Type"];
    
    return type switch
    {
        "Email" => new EmailNotificationService(),
        "SMS" => new SmsNotificationService(),
        _ => throw new InvalidOperationException()
    };
});
`

### Instance Registration

`csharp
// ✅ Use existing instance (always singleton)
var cache = new MemoryCache(new MemoryCacheOptions());
builder.Services.AddSingleton<IMemoryCache>(cache);

// ✅ Register multiple implementations
builder.Services.AddTransient<INotificationService, EmailNotificationService>();
builder.Services.AddTransient<INotificationService, SmsNotificationService>();

// Inject all implementations
public class NotificationDispatcher
{
    public NotificationDispatcher(IEnumerable<INotificationService> services)
    {
        // Gets EmailNotificationService AND SmsNotificationService
    }
}
`

---

## Detailed Guidance

Registration Methods guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Registration Methods before implementation work begins.
- Keep boundaries explicit so Registration Methods decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Registration Methods in production-facing code.
- When performance, correctness, or maintainability depends on consistent Registration Methods decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Registration Methods as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Registration Methods is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Registration Methods are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

