# API Versioning

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET Core request pipeline and routing fundamentals.
- Related examples: docs/Web-API-MVC/README.md
> Subject: [Web-API-MVC](../README.md)

## API Versioning

### URL Versioning

```csharp
// ✅ Version in URL (most common)
[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
public class UsersV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Version 1");
}

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("2.0")]
public class UsersV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Version 2");
}

// GET /api/v1/users → Version 1
// GET /api/v2/users → Version 2
```

### Header Versioning

```csharp
// ✅ Version in header
[ApiController]
[Route("api/users")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    // Request: GET /api/users
    // Header: api-version: 1.0
}
```

### Query String Versioning

```csharp
// ✅ Version in query string
[HttpGet]
public IActionResult Get([FromQuery] string version)
{
    // GET /api/users?version=1.0
}
```

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for API Versioning before implementation work begins.
- Keep boundaries explicit so API Versioning decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring API Versioning in production-facing code.
- When performance, correctness, or maintainability depends on consistent API Versioning decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying API Versioning as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where API Versioning is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for API Versioning are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- API Versioning is about ASP.NET endpoint architecture patterns. It matters because architecture choices affect testability, throughput, and maintainability.
- Use it when selecting minimal API, controller API, or MVC by problem shape.

2-minute answer:
- Start with the problem API Versioning solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer speed vs explicit control and extensibility.
- Close with one failure mode and mitigation: mixing styles without clear boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines API Versioning but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose API Versioning, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define API Versioning and map it to one concrete implementation in this module.
- 3 minutes: compare API Versioning with an alternative, then walk through one failure mode and mitigation.