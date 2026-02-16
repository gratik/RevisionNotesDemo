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

## Detailed Guidance

Configuration Validation guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Configuration Validation before implementation work begins.
- Keep boundaries explicit so Configuration Validation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Configuration Validation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Configuration Validation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Configuration Validation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Configuration Validation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Configuration Validation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

