# Common Pitfalls

> Subject: [Practical-Patterns](../README.md)

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


