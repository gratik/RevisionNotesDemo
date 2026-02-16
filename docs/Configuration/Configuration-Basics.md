# Configuration Basics

> Subject: [Configuration](../README.md)

## Configuration Basics

### appsettings.json Structure

`json
{
  "AppName": "MyApplication",
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "FromAddress": "noreply@example.com"
  },
  "Features": {
    "EnableCache": true,
    "MaxItemsPerPage": 50
  }
}
`

### Reading Configuration

`csharp
// ✅ Inject IConfiguration
public class MyService
{
    private readonly IConfiguration _configuration;
    
    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ReadSettings()
    {
        // ✅ Simple values
        string appName = _configuration["AppName"];  // "MyApplication"
        
        // ✅ Nested values with colon notation
        string logLevel = _configuration["Logging:LogLevel:Default"];  // "Information"
        
        // ✅ Strongly-typed with GetValue<T>
        int maxItems = _configuration.GetValue<int>("Features:MaxItemsPerPage");  // 50
        bool cacheEnabled = _configuration.GetValue<bool>("Features:EnableCache");  // true
        
        // ✅ With default values
        int pageSize = _configuration.GetValue<int>("PageSize", 20);  // 20 if not found
        
        // ✅ Connection strings
        string connString = _configuration.GetConnectionString("DefaultConnection");
    }
}
`

---

## Detailed Guidance

Configuration Basics guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Configuration Basics before implementation work begins.
- Keep boundaries explicit so Configuration Basics decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Configuration Basics in production-facing code.
- When performance, correctness, or maintainability depends on consistent Configuration Basics decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Configuration Basics as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Configuration Basics is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Configuration Basics are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

