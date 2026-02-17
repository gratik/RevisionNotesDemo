# Options Pattern

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET configuration providers and environment layering basics.
- Related examples: docs/Configuration/README.md
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


## Interview Answer Block
30-second answer:
- Options Pattern is about environment-aware application configuration strategy. It matters because configuration errors cause major runtime failures.
- Use it when safely managing settings across local, CI, and production.

2-minute answer:
- Start with the problem Options Pattern solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized config controls vs deployment flexibility.
- Close with one failure mode and mitigation: missing validation and secret handling discipline.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Options Pattern but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Options Pattern, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Options Pattern and map it to one concrete implementation in this module.
- 3 minutes: compare Options Pattern with an alternative, then walk through one failure mode and mitigation.