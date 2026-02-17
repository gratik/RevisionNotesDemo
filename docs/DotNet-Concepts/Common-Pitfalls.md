# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
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
- Common Pitfalls is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.