# Backend Patterns (.NET)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: REST API basics and React data-fetching/state management familiarity.
- Related examples: docs/DotNet-API-React/README.md
> Subject: [DotNet-API-React](../README.md)

## Backend Patterns (.NET)

### API contract and error envelope

```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IReadOnlyList<OrderDto>> Get(CancellationToken ct)
        => await _service.GetOrdersAsync(ct);
}
```

### CORS with explicit origins

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaPolicy", policy => policy
        .WithOrigins("http://localhost:5173", "https://app.example.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

app.UseRouting();
app.UseCors("SpaPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Backend Patterns (.NET) before implementation work begins.
- Keep boundaries explicit so Backend Patterns (.NET) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Backend Patterns (.NET) in production-facing code.
- When performance, correctness, or maintainability depends on consistent Backend Patterns (.NET) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Backend Patterns (.NET) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Backend Patterns (.NET) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Backend Patterns (.NET) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Backend Patterns (.NET) is about backend/frontend integration design for React clients. It matters because contract and state decisions affect delivery speed and reliability.
- Use it when building resilient API surfaces consumed by React applications.

2-minute answer:
- Start with the problem Backend Patterns (.NET) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rich backend contracts vs frontend adaptability.
- Close with one failure mode and mitigation: inconsistent API error/validation contracts across endpoints.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Backend Patterns (.NET) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Backend Patterns (.NET), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Backend Patterns (.NET) and map it to one concrete implementation in this module.
- 3 minutes: compare Backend Patterns (.NET) with an alternative, then walk through one failure mode and mitigation.