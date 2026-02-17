# Middleware Pipeline

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

