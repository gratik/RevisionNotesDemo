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


