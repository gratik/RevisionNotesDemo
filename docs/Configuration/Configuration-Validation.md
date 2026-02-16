# Configuration Validation

> Subject: [Configuration](../README.md)

## Configuration Validation

### Validation on Startup

`csharp
// ✅ Validate settings on startup
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    
    [Range(1, 65535)]
    public int Port { get; set; }
    
    [EmailAddress]
    public string FromAddress { get; set; } = string.Empty;
}

// Register with validation
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("EmailSettings"))
    .ValidateDataAnnotations()  // ✅ Validate attributes
    .ValidateOnStart();  // ✅ Fail fast on startup

// ✅ Custom validation
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("EmailSettings"))
    .Validate(settings =>
    {
        return !string.IsNullOrEmpty(settings.SmtpServer);
    }, "SmtpServer cannot be empty")
    .ValidateOnStart();
`

---


