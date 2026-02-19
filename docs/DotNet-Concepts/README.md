# Dependency Injection and IoC

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Core C# and dependency injection basics
- Related examples: Learning/DotNetConcepts/DependencyInjectionDemo.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#
- **When to Study**: Before Web/API modules to understand DI and host fundamentals.
- **Related Files**: `../Learning/DotNetConcepts/*.cs`, `../../Program.cs`
- **Estimated Time**: 60-90 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../OOP-Principles.md)
- **Next Step**: [Modern-CSharp.md](../Modern-CSharp.md)
<!-- STUDY-NAV-END -->


## Overview

Dependency Injection (DI) is a fundamental pattern in ASP.NET Core that enables loose coupling, testability,
and maintainability. This guide covers service lifetimes (Singleton, Scoped, Transient), constructor injection,
best practices, and common pitfalls.

---

## What is Dependency Injection?

**Dependency Injection** = Provide dependencies to a class rather than having the class create them

### Without DI (❌ Bad)

`csharp
// ❌ BAD: Hard dependency, can't test, can't swap
public class OrderService
{
    private EmailService _emailService = new EmailService();  // ❌ Tightly coupled
    
    public void ProcessOrder(Order order)
    {
        // Process order...
        _emailService.SendConfirmation(order);
    }
}
`

### With DI (✅ Good)

`csharp
// ✅ GOOD: Dependency injected, testable, flexible
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)  // ✅ Constructor injection
    {
        _emailService = emailService;
    }
    
    public void ProcessOrder(Order order)
    {
        // Process order...
        _emailService.SendConfirmation(order);
    }
}

// Can inject real or mock implementation
var service = new OrderService(new EmailService());  // Real
var service = new OrderService(new MockEmailService());  // Test
`

---

## Service Lifetimes

### Three Lifetimes

| Lifetime | Created | Disposed | Use Case |
|----------|---------|----------|----------|
| **Transient** | Every time requested | After use | Stateless, lightweight services |
| **Scoped** | Once per request/scope | End of request/scope | DbContext, per-request state |
| **Singleton** | Once per application | Application shutdown | Stateless, thread-safe, expensive to create |

### Transient

`csharp
// ✅ Register as Transient
builder.Services.AddTransient<IEmailService, EmailService>();

// New instance EVERY TIME
public class Controller1
{
    public Controller1(IEmailService emailService) { }  // Instance A
}

public class Controller2
{
    public Controller2(IEmailService emailService) { }  // Instance B (different)
}

// Even in same class
public class MyService
{
    public MyService(IEmailService email1, IEmailService email2)
    {
        // email1 and email2 are DIFFERENT instances
    }
}
`

**When to Use**: Lightweight, stateless services, no shared state

### Scoped

`csharp
// ✅ Register as Scoped
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddDbContext<AppDbContext>();  // Scoped by default

// One instance PER REQUEST
public class Controller1
{
    public Controller1(IOrderService orderService) { }  // Instance A for this request
}

public class Controller2
{
    public Controller2(IOrderService orderService) { }  // Same instance A (same request)
}

// Next request gets new instance
// Request 1: Instance A
// Request 2: Instance B
`

**When to Use**: DbContext, per-request state, database transactions

### Singleton

`csharp
// ✅ Register as Singleton
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// One instance FOR ENTIRE APPLICATION
public class Controller1
{
    public Controller1(ICacheService cache) { }  // Instance A
}

public class Controller2
{
    public Controller2(ICacheService cache) { }  // Same instance A
}

// All requests, all classes = same instance
`

**When to Use**: Configuration, caching, logging, stateless utilities

---

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

## Service Location (Anti-Pattern)

### Don't Use Service Locator

`csharp
// ❌ BAD: Service locator pattern (anti-pattern)
public class OrderService
{
    private readonly IServiceProvider _serviceProvider;
    
    public OrderService(IServiceProvider serviceProvider)  // ❌ Don't do this
    {
        _serviceProvider = serviceProvider;
    }
    
    public void ProcessOrder()
    {
        var emailService = _serviceProvider.GetService<IEmailService>();  // ❌ Hidden dependency
    }
}

// ✅ GOOD: Explicit constructor injection
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)  // ✅ Clear dependency
    {
        _emailService = emailService;
    }
}
`

**Exception**: Acceptable in factories or when dynamically resolving services

---

## Scoped Services in Singletons (Captive Dependency)

### The Problem

`csharp
// ❌ BAD: Scoped service injected into Singleton
public class MySingletonService  // Singleton
{
    private readonly AppDbContext _context;  // ❌ Scoped! Will be held forever
    
    public MySingletonService(AppDbContext context)
    {
        _context = context;  // ❌ DbContext never disposed
    }
}
// Memory leak! DbContext should be disposed after each request
`

### The Solution

`csharp
// ✅ GOOD: Use IServiceScopeFactory
public class MySingletonService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public MySingletonService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public async Task DoWorkAsync()
    {
        // ✅ Create scope, get scoped service, dispose scope
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await context.SaveChangesAsync();
    }  // ✅ Scope disposed, DbContext disposed
}
`

---

## Keyed Services (C# 12 / .NET 8+)

### Multiple Implementations

`csharp
// ✅ Register with keys
builder.Services.AddKeyedScoped<IPaymentService, StripePaymentService>("stripe");
builder.Services.AddKeyedScoped<IPaymentService, PayPalPaymentService>("paypal");

// ✅ Inject specific implementation by key
public class CheckoutService
{
    private readonly IPaymentService _paymentService;
    
    public CheckoutService([FromKeyedServices("stripe")] IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
}
`

---

## Validation

### Validate Services on Startup

`csharp
// ✅ Validate DI configuration on startup (Development only)
if (app.Environment.IsDevelopment())
{
    var serviceProvider = app.Services;
    
    // Throws exception if any service can't be resolved
    using var scope = serviceProvider.CreateScope();
    
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
}

// ✅ Better: Use ValidateOnBuild()
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService>();
// ... register all services

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;  // ✅ Detect scope issues
        options.ValidateOnBuild = true;  // ✅ Validate on startup
    });
}
`

---

## Best Practices

### ✅ Lifetime Selection
- **Transient**: Stateless, lightweight (most services)
- **Scoped**: DbContext, HttpContext, per-request state
- **Singleton**: Caching, configuration, expensive initialization

### ✅ Dependency Guidelines
- Prefer constructor injection over property/method injection
- Inject interfaces, not concrete classes
- Keep constructors simple (no logic)
- Don't inject IServiceProvider (service locator anti-pattern)
- Watch for captive dependencies (scoped in singleton)

### ✅ Registration
- Register services in Program.cs
- Use extension methods to organize (AddMyServices())
- Validate on startup in development
- Document required services

---

## Common Pitfalls

### ❌ Captive Dependency

`csharp
// ❌ BAD: Scoped DbContext in Singleton
builder.Services.AddSingleton<MyService>();  // Singleton
// MyService constructor has AppDbContext parameter (Scoped) ❌

// ✅ GOOD: Use IServiceScopeFactory
public class MyService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public async Task DoWork()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
`

### ❌ Disposing Scoped Services Manually

`csharp
// ❌ BAD: Manual disposal of DI-managed service
public class MyController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromServices] AppDbContext context)
    {
        var users = context.Users.ToList();
        context.Dispose();  // ❌ DON'T! DI manages lifetime
        return Ok(users);
    }
}

// ✅ GOOD: Let DI handle disposal
public class MyController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public MyController(AppDbContext context)
    {
        _context = context;  // ✅ DI disposes at end of request
    }
}
`

### ❌ Circular Dependencies

`csharp
// ❌ BAD: Circular dependency
public class ServiceA
{
    public ServiceA(ServiceB b) { }  // Depends on B
}

public class ServiceB
{
    public ServiceB(ServiceA a) { }  // Depends on A ❌ CIRCULAR!
}

// ✅ GOOD: Break with interface or refactor
public interface IServiceA { }

public class ServiceA : IServiceA
{
    public ServiceA(ServiceB b) { }
}

public class ServiceB
{
    public ServiceB(IServiceA a) { }  // ✅ Depends on interface
}
`

---

## Related Files

- [DotNetConcepts/DependencyInjectionDemo.cs](../../Learning/DotNetConcepts/DependencyInjectionDemo.cs)

---

## See Also

- [OOP Principles](../OOP-Principles.md) - Dependency Inversion Principle
- [Testing](../Testing.md) - Mocking dependencies
- [Web API and MVC](../Web-API-MVC.md) - DI in controllers
- [Practical Patterns](../Practical-Patterns.md) - Service patterns
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Modern-CSharp.md](../Modern-CSharp.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers DotNet Concepts and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know DotNet Concepts and I would just follow best practices."
- Strong answer: "For DotNet Concepts, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply DotNet Concepts in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [What is Dependency Injection?](What-is-Dependency-Injection.md)
- [Service Lifetimes](Service-Lifetimes.md)
- [Registration Methods](Registration-Methods.md)
- [Constructor Injection](Constructor-Injection.md)
- [Service Location (Anti-Pattern)](Service-Location-Anti-Pattern.md)
- [Scoped Services in Singletons (Captive Dependency)](Scoped-Services-in-Singletons-Captive-Dependency.md)
- [Keyed Services (C# 12 / .NET 8+)](Keyed-Services-C-12-NET-8.md)
- [Validation](Validation.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)



