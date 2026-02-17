# Global Exception Handling

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core API/service development skills and dependency injection familiarity.
- Related examples: docs/Practical-Patterns/README.md
> Subject: [Practical-Patterns](../README.md)

## Global Exception Handling

### Exception Middleware

```csharp
// ✅ Centralized error handling
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };
        
        context.Response.StatusCode = statusCode;
        
        var response = new
        {
            error = message,
            statusCode
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
}

// Register in Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---

## Detailed Guidance

Global Exception Handling guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Global Exception Handling before implementation work begins.
- Keep boundaries explicit so Global Exception Handling decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Global Exception Handling in production-facing code.
- When performance, correctness, or maintainability depends on consistent Global Exception Handling decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Global Exception Handling as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Global Exception Handling is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Global Exception Handling are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Global Exception Handling is about high-value implementation patterns for day-to-day engineering. It matters because practical patterns reduce repeated design mistakes.
- Use it when standardizing common cross-cutting behaviors in services.

2-minute answer:
- Start with the problem Global Exception Handling solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: pattern reuse vs context-specific customization needs.
- Close with one failure mode and mitigation: copying patterns without validating fit for the current problem.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Global Exception Handling but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Global Exception Handling, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Global Exception Handling and map it to one concrete implementation in this module.
- 3 minutes: compare Global Exception Handling with an alternative, then walk through one failure mode and mitigation.