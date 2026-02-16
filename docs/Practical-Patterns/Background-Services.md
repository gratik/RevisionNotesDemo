# Background Services

> Subject: [Practical-Patterns](../README.md)

## Background Services

### Hosted Service

```csharp
// ✅ Background job that runs periodically
public class EmailSenderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailSenderService> _logger;
    
    public EmailSenderService(IServiceProvider serviceProvider, ILogger<EmailSenderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email sender service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create scope for scoped dependencies
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await emailService.SendPendingEmailsAsync();
                
                // Wait 5 minutes before next run
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in email sender service");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        _logger.LogInformation("Email sender service stopped");
    }
}

// Register in Program.cs
services.AddHostedService<EmailSenderService>();
```

---


