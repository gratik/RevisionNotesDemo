# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [DotNet-Concepts](../README.md)

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


## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

