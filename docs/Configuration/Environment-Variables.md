# Environment Variables

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET configuration providers and environment layering basics.
- Related examples: docs/Configuration/README.md
> Subject: [Configuration](../README.md)

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

## Detailed Guidance

Environment Variables guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Environment Variables before implementation work begins.
- Keep boundaries explicit so Environment Variables decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Environment Variables in production-facing code.
- When performance, correctness, or maintainability depends on consistent Environment Variables decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Environment Variables as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Environment Variables is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Environment Variables are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Environment Variables is about environment-aware application configuration strategy. It matters because configuration errors cause major runtime failures.
- Use it when safely managing settings across local, CI, and production.

2-minute answer:
- Start with the problem Environment Variables solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized config controls vs deployment flexibility.
- Close with one failure mode and mitigation: missing validation and secret handling discipline.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Environment Variables but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Environment Variables, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Environment Variables and map it to one concrete implementation in this module.
- 3 minutes: compare Environment Variables with an alternative, then walk through one failure mode and mitigation.