# Logging in Different Scenarios

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

