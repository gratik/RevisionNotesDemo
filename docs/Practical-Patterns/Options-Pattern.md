# Options Pattern

> Subject: [Practical-Patterns](../README.md)

## Options Pattern

### Configuration

```csharp
// appsettings.json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@example.com",
    "FromAddress": "noreply@example.com"
  }
}

// ✅ Strongly-typed configuration
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

// Register in Program.cs
services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ✅ Inject settings
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
    
    public void SendEmail()
    {
        // Use _settings.SmtpServer, etc.
    }
}
```

---


