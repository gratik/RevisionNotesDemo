# Logging in Different Scenarios

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
> Subject: [Logging-Observability](../README.md)

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


## Interview Answer Block
30-second answer:
- Logging in Different Scenarios is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Logging in Different Scenarios solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Logging in Different Scenarios but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Logging in Different Scenarios, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Logging in Different Scenarios and map it to one concrete implementation in this module.
- 3 minutes: compare Logging in Different Scenarios with an alternative, then walk through one failure mode and mitigation.