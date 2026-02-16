# Configuration

> Subject: [Logging-Observability](../README.md)

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "MyApp.Services": "Debug"
    }
  }
}
```

### Different Levels per Environment

```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"  // More verbose in dev
    }
  }
}

// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"  // Less noise in prod
    }
  }
}
```

---


