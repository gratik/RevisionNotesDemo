# Caching Strategies

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


