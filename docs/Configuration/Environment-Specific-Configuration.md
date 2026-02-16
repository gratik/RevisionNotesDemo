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

## Detailed Guidance

Environment-Specific Configuration guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Environment-Specific Configuration before implementation work begins.
- Keep boundaries explicit so Environment-Specific Configuration decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Environment-Specific Configuration in production-facing code.
- When performance, correctness, or maintainability depends on consistent Environment-Specific Configuration decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Environment-Specific Configuration as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Environment-Specific Configuration is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Environment-Specific Configuration are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

