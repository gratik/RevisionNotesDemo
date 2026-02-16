# Options Pattern

> Subject: [Configuration](../README.md)

## Options Pattern

### Strongly-Typed Configuration

`csharp
// ✅ Create settings class
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// ✅ Register in Program.cs
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// ✅ Inject with IOptions
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
    
    public void SendEmail(string to, string subject, string body)
    {
        // Use _settings.SmtpServer, _settings.Port, etc.
    }
}
`

### IOptions vs IOptionsSnapshot vs IOptionsMonitor

| Interface | Lifetime | Reload | Use Case |
|-----------|----------|--------|----------|
| **IOptions<T>** | Singleton | ❌ Never | Static settings |
| **IOptionsSnapshot<T>** | Scoped | ✅ Per request | Request-scoped settings |
| **IOptionsMonitor<T>** | Singleton | ✅ Real-time | Settings that change at runtime |

`csharp
// ✅ IOptions - Read once at startup
public class MyService
{
    private readonly AppSettings _settings;
    
    public MyService(IOptions<AppSettings> options)
    {
        _settings = options.Value;  // ✅ Read once
    }
}

// ✅ IOptionsSnapshot - Reload per request
public class MyController : ControllerBase
{
    private readonly IOptionsSnapshot<AppSettings> _settings;
    
    public MyController(IOptionsSnapshot<AppSettings> settings)
    {
        _settings = settings;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        var current = _settings.Value;  // ✅ Fresh per request
        return Ok(current);
    }
}

// ✅ IOptionsMonitor - Real-time changes
public class BackgroundService
{
    private readonly IOptionsMonitor<AppSettings> _settings;
    
    public BackgroundService(IOptionsMonitor<AppSettings> settings)
    {
        _settings = settings;
        
        // ✅ React to changes
        _settings.OnChange(newSettings =>
        {
            Console.WriteLine("Settings changed!");
        });
    }
    
    public void DoWork()
    {
        var current = _settings.CurrentValue;  // ✅ Always latest
    }
}
`

---


