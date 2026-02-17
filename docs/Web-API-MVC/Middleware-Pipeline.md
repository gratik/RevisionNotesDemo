# Middleware Pipeline

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET Core request pipeline and routing fundamentals.
- Related examples: docs/Web-API-MVC/README.md
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


## Interview Answer Block
30-second answer:
- Middleware Pipeline is about ASP.NET endpoint architecture patterns. It matters because architecture choices affect testability, throughput, and maintainability.
- Use it when selecting minimal API, controller API, or MVC by problem shape.

2-minute answer:
- Start with the problem Middleware Pipeline solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer speed vs explicit control and extensibility.
- Close with one failure mode and mitigation: mixing styles without clear boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Middleware Pipeline but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Middleware Pipeline, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Middleware Pipeline and map it to one concrete implementation in this module.
- 3 minutes: compare Middleware Pipeline with an alternative, then walk through one failure mode and mitigation.