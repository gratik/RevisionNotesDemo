# User Secrets (Development)

> Subject: [Configuration](../README.md)

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

## Detailed Guidance

User Secrets (Development) guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for User Secrets (Development) before implementation work begins.
- Keep boundaries explicit so User Secrets (Development) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring User Secrets (Development) in production-facing code.
- When performance, correctness, or maintainability depends on consistent User Secrets (Development) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying User Secrets (Development) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where User Secrets (Development) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for User Secrets (Development) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

