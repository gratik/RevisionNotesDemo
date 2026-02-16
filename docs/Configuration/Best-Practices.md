# Best Practices

> Subject: [Configuration](../README.md)

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

## Detailed Guidance

Best Practices guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

