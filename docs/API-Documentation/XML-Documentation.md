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


