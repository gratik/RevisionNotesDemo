# Common Pitfalls

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


