# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET configuration providers and environment layering basics.
- Related examples: docs/Configuration/README.md
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

## Detailed Guidance

Common Pitfalls guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about environment-aware application configuration strategy. It matters because configuration errors cause major runtime failures.
- Use it when safely managing settings across local, CI, and production.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized config controls vs deployment flexibility.
- Close with one failure mode and mitigation: missing validation and secret handling discipline.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.