# Practical Implementation Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Web API and DI basics
- Related examples: Learning/PracticalPatterns/CachingImplementation.cs, Learning/PracticalPatterns/GlobalExceptionHandling.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Web API and MVC, Data Access
- **When to Study**: After core backend patterns to harden real projects.
- **Related Files**: `../Learning/PracticalPatterns/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Configuration.md)
- **Next Step**: [RealTime.md](../RealTime.md)
<!-- STUDY-NAV-END -->


## Overview

Practical patterns solve common real-world problems: caching, validation, mapping, error handling,
background jobs, and configuration. These patterns are less about theory and more about getting
production applications working reliably.

---

## Caching Strategies

### In-Memory Caching

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly IProductRepository _repository;
    
    public ProductService(IMemoryCache cache, IProductRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }
    
    // ✅ Cache-aside pattern
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        string cacheKey = $"product_{id}";
        
        // Try cache first
        if (_cache.TryGetValue(cacheKey, out Product? product))
            return product;
        
        // Cache miss - fetch from database
        product = await _repository.GetByIdAsync(id);
        
        if (product != null)
        {
            // Cache for 5 minutes
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            
            _cache.Set(cacheKey, product, cacheOptions);
        }
        
        return product;
    }
    
    // ✅ Invalidate cache on update
    public async Task UpdateProductAsync(Product product)
    {
        await _repository.UpdateAsync(product);
        _cache.Remove($"product_{product.Id}");
    }
}
```

### Distributed Caching (Redis)

```csharp
public class ProductService
{
    private readonly IDistributedCache _cache;
    
    public async Task<Product?> GetProductAsync(int id)
    {
        string cacheKey = $"product_{id}";
        
        // Try cache
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
            return JsonSerializer.Deserialize<Product>(cachedData);
        
        // Fetch from DB
        var product = await _repository.GetByIdAsync(id);
        
        if (product != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                options);
        }
        
        return product;
    }
}
```

---

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

## Mapping Patterns

### Manual Mapping

```csharp
// ✅ Explicit and clear
public class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
    
    public static User ToEntity(CreateUserRequest request)
    {
        return new User
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

### AutoMapper

```csharp
// ✅ Configuration
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Simple mapping
        CreateMap<User, UserDto>();
        
        // Custom mapping
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.ItemCount,
                opt => opt.MapFrom(src => src.Items.Count));
        
        // Reverse mapping
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

// Usage
public class UserService
{
    private readonly IMapper _mapper;
    
    public UserDto GetUser(User user)
    {
        return _mapper.Map<UserDto>(user);
    }
}
```

---

## Global Exception Handling

### Exception Middleware

```csharp
// ✅ Centralized error handling
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };
        
        context.Response.StatusCode = statusCode;
        
        var response = new
        {
            error = message,
            statusCode
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
}

// Register in Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---

## Background Services

### Hosted Service

```csharp
// ✅ Background job that runs periodically
public class EmailSenderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailSenderService> _logger;
    
    public EmailSenderService(IServiceProvider serviceProvider, ILogger<EmailSenderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email sender service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create scope for scoped dependencies
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await emailService.SendPendingEmailsAsync();
                
                // Wait 5 minutes before next run
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in email sender service");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        _logger.LogInformation("Email sender service stopped");
    }
}

// Register in Program.cs
services.AddHostedService<EmailSenderService>();
```

---

## Options Pattern

### Configuration

```csharp
// appsettings.json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@example.com",
    "FromAddress": "noreply@example.com"
  }
}

// ✅ Strongly-typed configuration
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

// Register in Program.cs
services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ✅ Inject settings
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
    
    public void SendEmail()
    {
        // Use _settings.SmtpServer, etc.
    }
}
```

---

## Serialization

### System.Text.Json

```csharp
// ✅ Serialize/Deserialize
var user = new User { Name = "Alice", Email = "alice@example.com" };

// To JSON
string json = JsonSerializer.Serialize(user, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
});

// From JSON
var deserializedUser = JsonSerializer.Deserialize<User>(json);

// ✅ Custom converter for DateTime
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }
    
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
    }
}
```

---

## Best Practices

### ✅ Caching
- Cache expensive operations (database queries, API calls)
- Set appropriate expiration times
- Invalidate cache on updates
- Use distributed cache for multi-server scenarios
- Monitor cache hit rates

### ✅ Validation
- Validate at API boundary (controllers)
- Use FluentValidation for complex rules
- Return clear error messages
- Validate business rules in domain layer

### ✅ Mapping
- Use AutoMapper for simple mappings
- Manual mapping for complex scenarios
- Don't map directly to/from database entities in API
- Keep mapping logic centralized

### ✅ Error Handling
- Use global exception middleware
- Log all exceptions
- Return appropriate status codes
- Don't expose internal errors to clients

### ✅ Background Services
- Use scoped dependencies correctly
- Handle cancellation tokens
- Log service lifecycle events
- Implement retry logic

---

## Common Pitfalls

### ❌ Over-Caching

```csharp
// ❌ BAD: Caching everything
_cache.Set("all_users", await GetAllUsersAsync(), TimeSpan.FromDays(1));
// Data becomes stale, memory usage increases

// ✅ GOOD: Cache specific, frequently accessed items
_cache.Set($"user_{id}", user, TimeSpan.FromMinutes(5));
```

### ❌ Forgetting to Invalidate Cache

```csharp
// ❌ BAD: Update without invalidation
public async Task UpdateUserAsync(User user)
{
    await _repository.UpdateAsync(user);  // ❌ Cache still has old data
}

// ✅ GOOD: Invalidate on update
public async Task UpdateUserAsync(User user)
{
    await _repository.UpdateAsync(user);
    _cache.Remove($"user_{user.Id}");
}
```

### ❌ Not Using Scopes in Background Services

```csharp
// ❌ BAD: Injecting scoped service in singleton
public class BackgroundService
{
    private readonly IUserRepository _repository;  // ❌ Scoped in singleton!
    
    public BackgroundService(IUserRepository repository)
    {
        _repository = repository;
    }
}

// ✅ GOOD: Create scope
public class BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    }
}
```

---

## Related Files

- [PracticalPatterns/ApiOptimization.cs](../../Learning/PracticalPatterns/ApiOptimization.cs)
- [PracticalPatterns/CachingImplementation.cs](../../Learning/PracticalPatterns/CachingImplementation.cs)
- [PracticalPatterns/GlobalExceptionHandling.cs](../../Learning/PracticalPatterns/GlobalExceptionHandling.cs)
- [PracticalPatterns/ValidationPatterns.cs](../../Learning/PracticalPatterns/ValidationPatterns.cs)
- [PracticalPatterns/MappingPatterns.cs](../../Learning/PracticalPatterns/MappingPatterns.cs)
- [PracticalPatterns/SerializationExamples.cs](../../Learning/PracticalPatterns/SerializationExamples.cs)
- [PracticalPatterns/OptionsPatternDemo.cs](../../Learning/PracticalPatterns/OptionsPatternDemo.cs)
- [PracticalPatterns/BackgroundServicesDemo.cs](../../Learning/PracticalPatterns/BackgroundServicesDemo.cs)

---

## See Also

- [Web API and MVC](../Web-API-MVC.md) - API patterns
- [Testing](../Testing.md) - Testing background services
- [Design Patterns](../Design-Patterns.md) - Structural patterns
- [Performance](../Performance.md) - Caching strategies
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [RealTime.md](../RealTime.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Practical Patterns and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Practical Patterns and I would just follow best practices."
- Strong answer: "For Practical Patterns, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Practical Patterns in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Caching Strategies](Caching-Strategies.md)
- [Validation Patterns](Validation-Patterns.md)
- [Mapping Patterns](Mapping-Patterns.md)
- [Global Exception Handling](Global-Exception-Handling.md)
- [Background Services](Background-Services.md)
- [Options Pattern](Options-Pattern.md)
- [Serialization](Serialization.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)



