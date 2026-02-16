# Environment-Specific Configuration

> Subject: [Configuration](../README.md)

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


