# Middleware Pipeline

> Subject: [Web-API-MVC](../README.md)

## Middleware Pipeline

### Order Matters!

```csharp
var app = builder.Build();

// ✅ Correct order
app.UseHttpsRedirection();       // 1. Redirect HTTP to HTTPS
app.UseStaticFiles();            // 2. Serve static files early
app.UseRouting();                // 3. Route matching
app.UseCors();                   // 4. CORS after routing
app.UseAuthentication();         // 5. WHO are you?
app.UseAuthorization();          // 6. WHAT can you do?
app.UseRateLimiter();            // 7. Rate limiting
app.MapControllers();            // 8. Execute endpoint
```

### Custom Middleware

```csharp
// ✅ Inline middleware
app.Use(async (context, next) =>
{
    var start = DateTimeOffset.UtcNow;
    await next();  // Call next middleware
    var elapsed = DateTimeOffset.UtcNow - start;
    
    Console.WriteLine($"Request took {elapsed.TotalMilliseconds}ms");
});

// ✅ Class-based middleware
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();
        
        _logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            sw.ElapsedMilliseconds);
    }
}

// Register
app.UseMiddleware<RequestTimingMiddleware>();
```

### Short-Circuiting

```csharp
// ✅ Stop pipeline early
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("OK");
        return;  // ✅ Don't call next()
    }
    
    await next();
});
```

---


