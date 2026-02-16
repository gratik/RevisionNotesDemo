# Log Scopes and Correlation

> Subject: [Logging-Observability](../README.md)

## Log Scopes and Correlation

### Using Scopes

```csharp
// ✅ Scope adds context to all logs within
using (_logger.BeginScope("Processing order {OrderId}", orderId))
{
    _logger.LogInformation("Validating order");
    _logger.LogInformation("Charging payment");
    _logger.LogInformation("Sending confirmation");
}
// All 3 logs automatically include OrderId
```

### Correlation IDs for Request Tracing

```csharp
// ✅ Track request across services
public class CorrelationMiddleware
{
    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationMiddleware> logger)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
        
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            context.Response.Headers.Add("X-Correlation-ID", correlationId);
            await _next(context);
        }
    }
}

// All logs in this request include CorrelationId
// Can trace entire request flow across microservices
```

---


