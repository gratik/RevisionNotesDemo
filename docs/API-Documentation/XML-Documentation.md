# XML Documentation

> Subject: [API-Documentation](../README.md)

## XML Documentation

### Enable XML Comments

```xml
<!-- In .csproj -->
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn> <!-- Suppress missing XML comment warnings -->
</PropertyGroup>
```

### Configure Swagger to Use XML

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

### Document Your API

```csharp
/// <summary>
/// Retrieves all users from the database
/// </summary>
/// <remarks>
/// Sample request:
///
///     GET /api/users
///
/// Returns paginated results. Use query parameters for filtering.
/// </remarks>
/// <param name="page">Page number (default: 1)</param>
/// <param name="pageSize">Items per page (default: 20, max: 100)</param>
/// <returns>List of users</returns>
/// <response code="200">Returns the list of users</response>
/// <response code="400">Invalid query parameters</response>
/// <response code="401">User is not authenticated</response>
[HttpGet]
[ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<List<UserDto>>> GetUsers(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    // Implementation
}
```

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for XML Documentation before implementation work begins.
- Keep boundaries explicit so XML Documentation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring XML Documentation in production-facing code.
- When performance, correctness, or maintainability depends on consistent XML Documentation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying XML Documentation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where XML Documentation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for XML Documentation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

