# API Versioning with Swagger

> Subject: [API-Documentation](../README.md)

## API Versioning with Swagger

### Multiple Swagger Documents

```csharp
// Add versioning support
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Generate Swagger doc for each version
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "⚠️ DEPRECATED - Please migrate to V2"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "My API",
        Version = "v2",
        Description = "Current version with enhanced features"
    });
});

// Configure Swagger UI with dropdown
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 (Current)");
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 (Deprecated)");
});
```

### Versioned Controllers

```csharp
// V1 Controller
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0", Deprecated = true)]
public class CustomersV1Controller : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(CustomerV1Dto), 200)]
    public IActionResult Get() => Ok(new CustomerV1Dto());
}

// V2 Controller
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class CustomersV2Controller : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(CustomerV2Dto), 200)]
    public IActionResult Get() => Ok(new CustomerV2Dto());
}
```

---


