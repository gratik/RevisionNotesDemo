# Scoped Services in Singletons (Captive Dependency)

> Subject: [DotNet-Concepts](../README.md)

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


