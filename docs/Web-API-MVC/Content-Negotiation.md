# Content Negotiation

> Subject: [Web-API-MVC](../README.md)

## Content Negotiation

```csharp
// ✅ Return JSON or XML based on Accept header
[HttpGet]
[Produces("application/json", "application/xml")]
public ActionResult<User> Get()
{
    var user = new User { Name = "Alice" };
    return Ok(user);
}

// Request: Accept: application/json → Returns JSON
// Request: Accept: application/xml → Returns XML
```

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Content Negotiation before implementation work begins.
- Keep boundaries explicit so Content Negotiation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Content Negotiation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Content Negotiation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Content Negotiation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Content Negotiation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Content Negotiation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

