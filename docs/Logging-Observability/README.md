# Logging and Application Monitoring

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: ASP.NET logging basics
- Related examples: Learning/Logging/StructuredLogging.cs, Learning/Observability/OpenTelemetrySetup.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Web API and MVC, Async Multithreading
- **When to Study**: Before production rollout and reliability tuning.
- **Related Files**: `../Learning/Logging/*.cs`, `../Learning/Observability/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Configuration.md)
- **Next Step**: [Design-Patterns.md](../Design-Patterns.md)
<!-- STUDY-NAV-END -->


## Overview

Logging is critical for debugging, monitoring, and understanding application behavior in production.
This guide covers structured logging with ILogger, log levels, scopes, correlation IDs, and performance
considerations. Good logging enables fast troubleshooting without bloating logs or impacting performance.

---

## Log Levels

| Level | Value | When to Use | Examples |
|-------|-------|-------------|----------|
| **Trace** | 0 | Detailed diagnostic info | Method entry/exit, variable values |
| **Debug** | 1 | Developer debugging | Query parameters, cache hits |
| **Information** | 2 | General flow | Request started, order processed |
| **Warning** | 3 | Unexpected but handled | Retry attempted, fallback used |
| **Error** | 4 | Failure in operation | Exception caught, operation failed |
| **Critical** | 5 | Application crash | Unrecoverable error, data corruption |

### Choosing the Right Level

```csharp
// ✅ Trace: Very detailed (disabled in production)
_logger.LogTrace("Entering GetUser method with userId={UserId}", userId);

// ✅ Debug: Helpful for debugging (disabled in production)
_logger.LogDebug("Cache hit for key {CacheKey}", key);

// ✅ Information: General flow (enabled in production)
_logger.LogInformation("User {UserId} logged in successfully", userId);

// ✅ Warning: Something unexpected but handled
_logger.LogWarning("API rate limit reached, using fallback data");

// ✅ Error: Operation failed
_logger.LogError(ex, "Failed to save order {OrderId}", orderId);

// ✅ Critical: System is unusable
_logger.LogCritical(ex, "Database connection pool exhausted");
```

---

## Structured Logging

### String Interpolation vs Templates

```csharp
// ❌ BAD: String interpolation (not structured)
_logger.LogInformation($"Order {orderId} processed in {elapsed}ms");
// Logged as: "Order 123 processed in 45ms" (just a string)

// ✅ GOOD: Template with parameters (structured)
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);
// Logged as: { Message: "Order...", OrderId: 123, ElapsedMs: 45 }
// Can query: WHERE OrderId = 123 OR WHERE ElapsedMs > 100
```

### Benefits of Structured Logging

**Queryable**: Search by specific field values
```sql
-- Find all logs for specific order
SELECT * FROM Logs WHERE OrderId = 123

-- Find slow operations
SELECT * FROM Logs WHERE ElapsedMs > 1000
```

**Performance**: No string concatenation
```csharp
// ✅ Template compiled once, parameters passed efficiently
_logger.LogInformation("Processing {Count} items", items.Count);
```

---

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

## Performance Considerations

### Log Guards

```csharp
// ❌ BAD: Expensive operation even if logging disabled
_logger.LogDebug($"User data: {SerializeToJson(user)}");
// SerializeToJson runs even if Debug logging is off!

// ✅ GOOD: Check if enabled first
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("User data: {UserJson}", SerializeToJson(user));
}

// ✅ BETTER: Use LoggerMessage for high-performance logging
private static readonly Action<ILogger, string, Exception?> _logUserData =
    LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(1, "UserData"),
        "User data: {UserJson}");

public void LogUserData(string json)
{
    _logUserData(_logger, json, null);
}
```

### High-Performance Logging with LoggerMessage

```csharp
public static class LogMessages
{
    // ✅ Compiled once, reused (zero allocations)
    private static readonly Action<ILogger, int, int, Exception?> _orderProcessed =
        LoggerMessage.Define<int, int>(
            LogLevel.Information,
            new EventId(1001, "OrderProcessed"),
            "Order {OrderId} processed in {ElapsedMs}ms");
    
    public static void LogOrderProcessed(this ILogger logger, int orderId, int elapsedMs)
    {
        _orderProcessed(logger, orderId, elapsedMs, null);
    }
}

// Usage
_logger.LogOrderProcessed(123, 45);
// 10x faster than regular logging in hot paths
```

---

## Event IDs for Categorization

```csharp
public static class EventIds
{
    public static readonly EventId OrderCreated = new(1001, "OrderCreated");
    public static readonly EventId OrderShipped = new(1002, "OrderShipped");
    public static readonly EventId PaymentFailed = new(2001, "PaymentFailed");
    public static readonly EventId DatabaseError = new(3001, "DatabaseError");
}

// ✅ Use event IDs for filtering and alerting
_logger.LogInformation(EventIds.OrderCreated, "Order {OrderId} created", orderId);
_logger.LogError(EventIds.PaymentFailed, ex, "Payment failed for order {OrderId}", orderId);

// Can filter/alert on specific event IDs
// Alert on EventId=2001 (PaymentFailed)
// Dashboard for EventId=1001 (OrderCreated)
```

---

## Logging in Different Scenarios

### API Request Logging

```csharp
public async Task<IActionResult> GetOrder(int id)
{
    using (_logger.BeginScope("GetOrder {OrderId}", id))
    {
        _logger.LogInformation("Fetching order from database");
        
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
        {
            _logger.LogWarning("Order not found");
            return NotFound();
        }
        
        _logger.LogInformation("Order retrieved successfully");
        return Ok(order);
    }
}
```

### Exception Logging

```csharp
try
{
    await ProcessOrderAsync(order);
}
catch (PaymentException ex)
{
    // ✅ Include exception and context
    _logger.LogError(ex, 
        "Payment processing failed for order {OrderId}, customer {CustomerId}",
        order.Id, order.CustomerId);
    throw;
}
catch (Exception ex)
{
    // ✅ Critical for unexpected exceptions
    _logger.LogCritical(ex, 
        "Unexpected error processing order {OrderId}", order.Id);
    throw;
}
```

### Background Job Logging

```csharp
public class EmailSenderJob : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email sender job started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var count = await SendPendingEmailsAsync();
                _logger.LogInformation("Sent {EmailCount} emails", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending emails");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
        
        _logger.LogInformation("Email sender job stopped");
    }
}
```

---

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "MyApp.Services": "Debug"
    }
  }
}
```

### Different Levels per Environment

```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"  // More verbose in dev
    }
  }
}

// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"  // Less noise in prod
    }
  }
}
```

---

## Best Practices

### ✅ Logging Guidelines
- Use structured logging (templates, not interpolation)
- Include relevant context in log messages
- Use appropriate log levels
- Add correlation IDs for request tracing
- Use log scopes for grouping related logs
- Include timing information for operations

### ✅ Performance
- Use LoggerMessage for hot paths
- Check IsEnabled before expensive operations
- Avoid logging in tight loops
- Don't serialize large objects unless necessary
- Use async logging providers where available

### ✅ Security
- Never log passwords or secrets
- Sanitize PII (personally identifiable information)
- Be careful with sensitive data in exceptions
- Use separate logs for security events

---

## Common Pitfalls

### ❌ String Interpolation

```csharp
// ❌ BAD: Not structured, always allocates string
_logger.LogInformation($"Processing order {orderId}");

// ✅ GOOD: Structured, efficient
_logger.LogInformation("Processing order {OrderId}", orderId);
```

### ❌ Logging Sensitive Data

```csharp
// ❌ DANGER: Logging password!
_logger.LogDebug("Login attempt: {Username}, {Password}", username, password);

// ✅ SAFE: Don't log sensitive data
_logger.LogDebug("Login attempt: {Username}", username);
```

### ❌ Over-Logging

```csharp
// ❌ BAD: Logs in loop (millions of logs)
foreach (var item in items)
{
    _logger.LogDebug("Processing item {ItemId}", item.Id);
}

// ✅ GOOD: Log summary
_logger.LogInformation("Processing {ItemCount} items", items.Count);
```

### ❌ Wrong Log Level

```csharp
// ❌ BAD: Using Error for normal flow
if (user == null)
{
    _logger.LogError("User not found");  // ❌ Not an error!
    return NotFound();
}

// ✅ GOOD: Use Warning or Information
if (user == null)
{
    _logger.LogWarning("User {UserId} not found", userId);
    return NotFound();
}
```

---

## Related Files

- [Logging/ILoggerDeepDive.cs](../../Learning/Logging/ILoggerDeepDive.cs)
- [Logging/StructuredLogging.cs](../../Learning/Logging/StructuredLogging.cs)
- [Logging/LoggingBestPractices.cs](../../Learning/Logging/LoggingBestPractices.cs)
- [Examples/RevisionNotes.Observability.AzureMonitor/Program.cs](../../Examples/RevisionNotes.Observability.AzureMonitor/Program.cs)

---

## See Also

- [Performance](../Performance.md) - High-performance logging patterns
- [Resilience](../Resilience.md) - Logging retry attempts and circuit breaker state
- [Web API and MVC](../Web-API-MVC.md) - Request/response logging
- [Security](../Security.md) - Security event logging
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Design-Patterns.md](../Design-Patterns.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Logging Observability and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Logging Observability and I would just follow best practices."
- Strong answer: "For Logging Observability, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Logging Observability in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Log Levels](Log-Levels.md)
- [Structured Logging](Structured-Logging.md)
- [Log Scopes and Correlation](Log-Scopes-and-Correlation.md)
- [OpenTelemetry vs Application Insights](OpenTelemetry-vs-Application-Insights.md)
- [OpenTelemetry and Application Insights Integration](OpenTelemetry-and-Application-Insights-Integration.md)
- [Correlation ID and W3C Trace Context](Correlation-ID-and-W3C-Trace-Context.md)
- [HTTP and Service Bus Propagation](HTTP-and-ServiceBus-Propagation.md)
- [Application Insights KQL Correlation Queries](Application-Insights-KQL-Correlation-Queries.md)
- [Azure Deployment Topology for Observability](Azure-Deployment-Topology-for-Observability.md)
- [Performance Considerations](Performance-Considerations.md)
- [Event IDs for Categorization](Event-IDs-for-Categorization.md)
- [Logging in Different Scenarios](Logging-in-Different-Scenarios.md)
- [Configuration](../Configuration.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)



