# Common Pitfalls

> Subject: [Configuration](../README.md)

## Common Pitfalls

### ❌ Committing Secrets

`json
// ❌ BAD: Secrets in appsettings.json
{
  "Database": {
    "ConnectionString": "Server=prod;User=sa;Password=MyPassword123"  // ❌ DON'T!
  }
}

// ✅ GOOD: Use placeholders
{
  "Database": {
    "ConnectionString": ""  // ✅ Set via environment variable
  }
}
`

### ❌ Not Using Options Pattern

`csharp
// ❌ BAD: Reading configuration everywhere
public class EmailService
{
    public void SendEmail(IConfiguration config)
    {
        var server = config["EmailSettings:SmtpServer"];  // ❌ Stringly-typed
        var port = config.GetValue<int>("EmailSettings:Port");
    }
}

// ✅ GOOD: Options pattern
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;  // ✅ Strongly-typed
    }
}
`

---


