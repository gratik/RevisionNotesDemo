# Configuration and Settings Management

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Dependency injection and app configuration basics
- Related examples: Learning/Configuration/ConfigurationPatterns.cs, Learning/Configuration/FeatureFlags.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: DotNet Concepts
- **When to Study**: Early in Backend/API track before production wiring.
- **Related Files**: `../Learning/Configuration/*.cs`
- **Estimated Time**: 60-90 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Configuration.md)
- **Next Step**: [Web-API-MVC.md](Web-API-MVC.md)
<!-- STUDY-NAV-END -->


## Overview

Configuration in ASP.NET Core is flexible, composable, and environment-aware. This guide covers IConfiguration,
the Options pattern, feature flags, configuration providers, and best practices for managing settings across
development, staging, and production environments.

---

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

## Environment-Specific Configuration

### Configuration Hierarchy

`
1. appsettings.json (base)
2. appsettings.{Environment}.json (override)
3. User Secrets (development only)
4. Environment Variables
5. Command-line arguments
`

**Later sources override earlier ones**

### Environment Files

`json
// appsettings.json (all environments)
{
  "Database": {
    "Timeout": 30
  }
}

// appsettings.Development.json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=Dev",
    "Timeout": 60  // Override
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}

// appsettings.Production.json
{
  "Database": {
    "ConnectionString": "Server=prod-server;Database=Prod"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
`

### Setting Environment

`ash
# Windows
$env:ASPNETCORE_ENVIRONMENT="Development"

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Production

# launchSettings.json
{
  "profiles": {
    "Development": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
`

---

## User Secrets (Development)

### Why User Secrets?

**Problem**: Don't commit sensitive data (passwords, API keys) to source control  
**Solution**: User Secrets store settings outside project directory

`ash
# Initialize user secrets
dotnet user-secrets init

# Set a secret
dotnet user-secrets set "EmailSettings:Password" "mypassword"
dotnet user-secrets set "ApiKeys:Stripe" "sk_test_123456"

# List secrets
dotnet user-secrets list

# Remove secret
dotnet user-secrets remove "ApiKeys:Stripe"

# Clear all
dotnet user-secrets clear
`

### Using User Secrets

`csharp
// ✅ Automatically loaded in Development environment
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
        // Password comes from user secrets in dev
        // Comes from environment variables in production
    }
}
`

---

## Environment Variables

### Reading Environment Variables

`csharp
// ✅ Environment variables override appsettings.json
var value = _configuration["MySetting"];  // Checks env vars first

// ✅ Explicit environment variable read
var envValue = Environment.GetEnvironmentVariable("MY_SETTING");

// ✅ Hierarchical with double underscore
// Environment variable: EmailSettings__SmtpServer
// Maps to: EmailSettings:SmtpServer
`

### Setting Environment Variables

`ash
# Windows
$env:EmailSettings__SmtpServer="smtp.gmail.com"
$env:ConnectionStrings__DefaultConnection="Server=prod;Database=MyDb"

# Linux/Mac
export EmailSettings__SmtpServer=smtp.gmail.com
export ConnectionStrings__DefaultConnection="Server=prod;Database=MyDb"

# Docker
docker run -e EmailSettings__SmtpServer=smtp.gmail.com myapp

# Kubernetes
env:
  - name: EmailSettings__SmtpServer
    value: "smtp.gmail.com"
`

---

## Feature Flags

### Configuration-Based Feature Flags

`csharp
// appsettings.json
{
  "FeatureFlags": {
    "EnableNewDashboard": true,
    "EnableBetaFeatures": false,
    "EnableCache": true
  }
}

// ✅ Feature flag settings class
public class FeatureFlags
{
    public bool EnableNewDashboard { get; set; }
    public bool EnableBetaFeatures { get; set; }
    public bool EnableCache { get; set; }
}

// Register
builder.Services.Configure<FeatureFlags>(
    builder.Configuration.GetSection("FeatureFlags"));

// ✅ Use in service
public class DashboardService
{
    private readonly FeatureFlags _features;
    
    public DashboardService(IOptions<FeatureFlags> options)
    {
        _features = options.Value;
    }
    
    public Dashboard GetDashboard()
    {
        if (_features.EnableNewDashboard)
        {
            return new NewDashboard();
        }
        return new LegacyDashboard();
    }
}
`

### Microsoft.FeatureManagement

`csharp
// ✅ Install package: Microsoft.FeatureManagement.AspNetCore

// appsettings.json
{
  "FeatureManagement": {
    "BetaFeatures": true,
    "PremiumFeatures": {
      "EnabledFor": [
        {
          "Name": "Percentage",
          "Parameters": {
            "Value": 50  // 50% rollout
          }
        }
      ]
    }
  }
}

// Register
builder.Services.AddFeatureManagement();

// ✅ Check feature in controller
public class FeaturesController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    
    public FeaturesController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }
    
    [HttpGet("beta")]
    public async Task<IActionResult> GetBetaFeature()
    {
        if (await _featureManager.IsEnabledAsync("BetaFeatures"))
        {
            return Ok(new { feature = "Beta content" });
        }
        return Ok(new { feature = "Stable content" });
    }
}

// ✅ Use attribute
[FeatureGate("BetaFeatures")]
[HttpGet("beta-only")]
public IActionResult BetaOnlyEndpoint()
{
    return Ok("This is only available when BetaFeatures is enabled");
}
`

---

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

## Best Practices

### ✅ Configuration Management
- Use appsettings.json for non-sensitive defaults
- Use User Secrets for local development secrets
- Use environment variables for production secrets
- Use strongly-typed Options pattern (not IConfiguration directly)
- Validate configuration on startup
- Document required settings in README

### ✅ Security
- Never commit secrets to source control
- Use Azure Key Vault or AWS Secrets Manager in production
- Rotate secrets regularly
- Limit access to production configuration
- Use different connection strings per environment

### ✅ Environment Strategy
- Development: User Secrets + appsettings.Development.json
- Staging: Environment variables + appsettings.Staging.json
- Production: Environment variables + appsettings.Production.json

---

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

## Related Files

- [Configuration/ConfigurationPatterns.cs](../Learning/Configuration/ConfigurationPatterns.cs)
- [Configuration/FeatureFlags.cs](../Learning/Configuration/FeatureFlags.cs)
- [Configuration/OptionsPatternDeepDive.cs](../Learning/Configuration/OptionsPatternDeepDive.cs)

---

## See Also

- [Practical Patterns](Practical-Patterns.md) - Options pattern in practice
- [Security](Security.md) - Secrets management
- [Web API and MVC](Web-API-MVC.md) - Configuration in web applications
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Web-API-MVC.md](Web-API-MVC.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Configuration and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Configuration and I would just follow best practices."
- Strong answer: "For Configuration, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Configuration in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
