# API Documentation and OpenAPI

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Web API basics, OpenAPI concepts
- Related examples: Learning/WebAPI/OpenAPISwaggerAdvanced.cs, Learning/WebAPI/ContentNegotiationAdvanced.cs


**Last Updated**: 2026-02-15

Comprehensive guide to documenting APIs with Swagger/OpenAPI, including versioning strategies,
XML documentation, schema customization, and testing patterns. Essential for professional API development.

## Module Metadata

- **Prerequisites**: Web API and MVC
- **When to Study**: After building endpoints and before public API rollout.
- **Related Files**: `../Learning/WebAPI/OpenAPISwaggerAdvanced.cs`, `../Learning/WebAPI/Versioning/*.cs`
- **Estimated Time**: 60-90 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Configuration.md)
- **Next Step**: [Data-Access.md](Data-Access.md)
<!-- STUDY-NAV-END -->


---

## Why API Documentation Matters

**For Developers**:

- Clear contract between frontend and backend
- Interactive testing with Swagger UI
- Automatic client SDK generation
- Faster onboarding for new team members

**For API Consumers**:

- Self-service discovery
- Try-it-out functionality
- Type definitions and examples
- Version comparison

**Business Impact**:

- Reduced support tickets
- Faster integration
- Better developer experience
- Increased API adoption

---

## Swagger vs OpenAPI

| Aspect            | Swagger             | OpenAPI                       |
| ----------------- | ------------------- | ----------------------------- |
| **What is it?**   | Toolset for OpenAPI | Specification standard        |
| **Specification** | Uses OpenAPI 3.x    | Format definition (JSON/YAML) |
| **Tools**         | SwaggerUI, Codegen  | Industry standard             |
| **Usage**         | Implementation      | Contract                      |

**Relationship**: Swagger is the toolset, OpenAPI is the specification it implements.

---

## Basic Swagger Setup

### Installation

```csharp
// In .csproj
<PackageReference Include="Swashbuckle.AspNetCore" Version="*" />
```

### Minimal Configuration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ Add Swagger generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "A sample API for learning",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@example.com",
            Url = new Uri("https://example.com/support")
        }
    });
});

var app = builder.Build();

// ✅ Enable Swagger UI (only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty;  // Swagger at root: http://localhost:5000/
    });
}

app.MapControllers();
app.Run();
```

**Access Points**:

- Swagger UI: `http://localhost:5000/`
- OpenAPI JSON: `http://localhost:5000/swagger/v1/swagger.json`

---

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

## Response Type Documentation

### ProducesResponseType Attribute

```csharp
// ✅ GOOD: Explicit response types
[HttpGet("{id}")]
[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<UserDto>> GetUser(int id)
{
    var user = await _repository.GetByIdAsync(id);

    if (user == null)
        return NotFound(new ProblemDetails
        {
            Title = "User not found",
            Status = 404,
            Detail = $"User with ID {id} does not exist"
        });

    return Ok(user);
}

// ❌ BAD: No documentation
[HttpGet("{id}")]
public async Task<ActionResult> GetUser(int id)
{
    // Swagger doesn't know what types to expect
}
```

### Multiple Response Scenarios

```csharp
[HttpPost]
[ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
{
    // 400: Invalid request
    if (!ModelState.IsValid)
        return ValidationProblem(ModelState);

    // 409: User already exists
    if (await _repository.ExistsAsync(request.Email))
        return Conflict(new ProblemDetails
        {
            Title = "User already exists",
            Detail = $"User with email {request.Email} already exists"
        });

    var user = await _repository.CreateAsync(request);

    // 201: Created successfully
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

---

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

## Schema Customization

### Custom Schema Examples

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Add examples to schemas
    options.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Example = new OpenApiString("2026-02-14T10:30:00Z")
    });

    // ✅ Customize enums
    options.SchemaFilter<EnumSchemaFilter>();

    // ✅ Hide internal properties
    options.SchemaFilter<HideInternalPropertiesFilter>();
});

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
```

### Request/Response Examples

```csharp
/// <summary>
/// User registration request
/// </summary>
/// <example>
/// {
///   "email": "john.doe@example.com",
///   "password": "SecureP@ssw0rd!",
///   "firstName": "John",
///   "lastName": "Doe"
/// }
/// </example>
public class RegisterUserRequest
{
    /// <summary>
    /// Email address (must be unique)
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password (min 8 chars, must contain uppercase, lowercase, digit, special char)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
```

---

## Security Documentation

### JWT Bearer Authentication

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Define security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // ✅ Apply security globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

**Result**: "Authorize" button in Swagger UI where users can enter JWT token.

### API Key Authentication

```csharp
options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
{
    Description = "API Key needed to access endpoints. API-Key: {your key}",
    In = ParameterLocation.Header,
    Name = "API-Key",
    Type = SecuritySchemeType.ApiKey
});
```

---

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

## Testing with Swagger

### Manual Testing

1. **Navigate to Swagger UI**: `http://localhost:5000/`
2. **Authenticate** (if required): Click "Authorize", enter token
3. **Expand endpoint**: Click on GET/POST/etc.
4. **Try it out**: Click "Try it out" button
5. **Enter parameters**: Fill in required/optional parameters
6. **Execute**: Click "Execute" button
7. **Review response**: Check status code, headers, body

### Automated Testing

```csharp
// Test that Swagger JSON is generated correctly
public class SwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SwaggerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_JSON_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"openapi\":", content);
    }

    [Fact]
    public async Task Swagger_UI_IsAccessible()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Best Practices

### Documentation

- ✅ Use XML comments for all public endpoints
- ✅ Provide request/response examples
- ✅ Document all possible status codes
- ✅ Include authentication requirements
- ✅ Add meaningful operation IDs

### Versioning

- ✅ Create separate Swagger doc for each version
- ✅ Mark deprecated versions clearly
- ✅ Provide migration guide in description
- ✅ Show version in URL and headers

### Security

- ✅ Never expose Swagger in production (or require auth)
- ✅ Document authentication requirements
- ✅ Hide internal/admin endpoints
- ✅ Sanitize error messages

### Performance

- ✅ Enable Swagger only in Development
- ✅ Cache generated JSON in production (if needed)
- ✅ Use minimal XML comment parsing
- ✅ Consider Swagger alternatives for high-traffic APIs

---

## Common Pitfalls

### ❌ **Exposing Internal Details**

```csharp
// ❌ BAD: Leaking implementation details
public class UserDto
{
    public int Id { get; set; }
    public string DatabaseConnectionString { get; set; }  // ❌ Security risk
    public string InternalNotes { get; set; }  // ❌ Not for clients
}

// ✅ GOOD: Only expose what clients need
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}
```

### ❌ **Missing ProducesResponseType**

```csharp
// ❌ BAD: Swagger doesn't know return types
[HttpGet]
public async Task<IActionResult> Get()
{
    return Ok(new UserDto());  // Swagger shows "object"
}

// ✅ GOOD: Explicit type documentation
[HttpGet]
[ProducesResponseType(typeof(UserDto), 200)]
public async Task<ActionResult<UserDto>> Get()
{
    return Ok(new UserDto());
}
```

### ❌ **No Version Strategy**

```csharp
// ❌ BAD: Adding fields breaks existing clients
[HttpGet]
public UserDto Get()
{
    return new UserDto
    {
        Id = 1,
        Email = "test@example.com",
        NewField = "breaks clients"  // ❌ Breaking change
    };
}

// ✅ GOOD: Version your API
[ApiVersion("2.0")]
[HttpGet]
public UserV2Dto Get()
{
    return new UserV2Dto  // New DTO for V2
    {
        Id = 1,
        Email = "test@example.com",
        NewField = "safe in V2"
    };
}
```

### ❌ **Swagger in Production Without Auth**

```csharp
// ❌ BAD: Exposing API structure to everyone
app.UseSwagger();
app.UseSwaggerUI();

// ✅ GOOD: Only in Development, or with auth
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// OR: Protected Swagger in production
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/swagger"),
    appBuilder => appBuilder.UseMiddleware<ApiKeyAuthMiddleware>());
```

---

## Related Files

- [WebAPI/Versioning/VersioningBestPractices.cs](../Learning/WebAPI/Versioning/VersioningBestPractices.cs) - API versioning with Swagger integration
- [WebAPI/Versioning/APIVersioningStrategies.cs](../Learning/WebAPI/Versioning/APIVersioningStrategies.cs) - URL, header, query versioning
- [WebAPI/Middleware/MiddlewareBestPractices.cs](../Learning/WebAPI/Middleware/MiddlewareBestPractices.cs) - API middleware patterns

---

## See Also

- [Web API and MVC](Web-API-MVC.md) - API development patterns
- [Security](Security.md) - Authentication and authorization
- [Testing](Testing.md) - API testing strategies
- [Configuration](Configuration.md) - Environment-specific settings
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Data-Access.md](Data-Access.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers API Documentation and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know API Documentation and I would just follow best practices."
- Strong answer: "For API Documentation, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply API Documentation in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
