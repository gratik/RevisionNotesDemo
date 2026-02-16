# Advanced Configurations

> Subject: [API-Documentation](../README.md)

## Advanced Configurations

### Group Endpoints by Tags

```csharp
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "users")]  // Custom grouping
public class UsersController : ControllerBase
{
    [HttpGet]
    [Tags("Users", "Public")]  // Multiple tags
    public IActionResult GetUsers() => Ok();
}
```

### Hide Endpoints from Swagger

```csharp
[HttpGet("internal/metrics")]
[ApiExplorerSettings(IgnoreApi = true)]  // ✅ Not shown in Swagger
public IActionResult GetInternalMetrics() => Ok();
```

### Custom Operation Filters

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<AddResponseHeadersFilter>();
});

public class AddResponseHeadersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses["200"].Headers ??= new Dictionary<string, OpenApiHeader>();
        operation.Responses["200"].Headers.Add("X-Rate-Limit", new OpenApiHeader
        {
            Description = "Number of requests remaining",
            Schema = new OpenApiSchema { Type = "integer" }
        });
    }
}
```

---


