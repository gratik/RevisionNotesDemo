# Caching Strategies

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core API/service development skills and dependency injection familiarity.
- Related examples: docs/Practical-Patterns/README.md
> Subject: [Practical-Patterns](../README.md)

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


## Interview Answer Block
30-second answer:
- Caching Strategies is about high-value implementation patterns for day-to-day engineering. It matters because practical patterns reduce repeated design mistakes.
- Use it when standardizing common cross-cutting behaviors in services.

2-minute answer:
- Start with the problem Caching Strategies solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: pattern reuse vs context-specific customization needs.
- Close with one failure mode and mitigation: copying patterns without validating fit for the current problem.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Caching Strategies but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Caching Strategies, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Caching Strategies and map it to one concrete implementation in this module.
- 3 minutes: compare Caching Strategies with an alternative, then walk through one failure mode and mitigation.