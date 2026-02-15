# Web API, Minimal API, and MVC Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: HTTP and ASP.NET Core basics
- Related examples: Learning/WebAPI/MinimalAPI/MinimalAPIBestPractices.cs, Learning/WebAPI/ControllerAPI/WebAPIBestPractices.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Module Metadata

- **Prerequisites**: DotNet Concepts, Async Multithreading
- **When to Study**: Start of Backend/API implementation work.
- **Related Files**: `../WebAPI/**/*.cs`
- **Estimated Time**: 150-180 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Configuration.md)
- **Next Step**: [API-Documentation.md](API-Documentation.md)
<!-- STUDY-NAV-END -->


## Overview

ASP.NET Core offers multiple approaches for building web applications: Minimal APIs (lightweight, functional),
Controller APIs (structured, feature-rich), and MVC (server-rendered views). This guide covers when to use each,
middleware patterns, versioning strategies, and common pitfalls.

---

## Minimal API vs Controller API vs MVC

| Feature | Minimal API | Controller API | MVC |
|---------|-------------|----------------|-----|
| **Introduced** | .NET 6 | .NET Core 1.0 | .NET Core 1.0 |
| **Style** | Functional | OOP | OOP + Views |
| **Boilerplate** | Minimal | Moderate | More |
| **Best For** | Simple APIs, microservices | Large APIs, complex logic | Server-rendered apps |
| **Filters** | Limited | ✅ Action/Resource filters | ✅ Full filter pipeline |
| **Routing** | Convention-based | Attribute-based | Convention/Attribute |
| **DI** | Parameter injection | Constructor injection | Constructor injection |
| **Testing** | Harder | Easier | Easier |

### When to Use Each

**Minimal API**: Small APIs, microservices, quick prototypes
**Controller API**: Large REST APIs, complex validation, reusable filters
**MVC**: Server-rendered applications with views (Razor Pages often better)

---

## Minimal API Examples

### Basic Endpoints

```csharp
// ✅ Simple GET
app.MapGet("/api/users", async (IUserRepository repo) =>
{
    var users = await repo.GetAllAsync();
    return Results.Ok(users);
});

// ✅ GET with route parameter
app.MapGet("/api/users/{id:int}", async (int id, IUserRepository repo) =>
{
    var user = await repo.GetByIdAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

// ✅ POST with validation
app.MapPost("/api/users", async (CreateUserRequest request, IUserRepository repo) =>
{
    if (string.IsNullOrEmpty(request.Name))
        return Results.BadRequest("Name is required");
    
    var user = await repo.CreateAsync(request);
    return Results.Created($"/api/users/{user.Id}", user);
});

// ✅ DELETE
app.MapDelete("/api/users/{id:int}", async (int id, IUserRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});
```

### Grouping Endpoints

```csharp
// ✅ Group related endpoints
var users = app.MapGroup("/api/users")
    .RequireAuthorization()  // Apply to all
    .WithTags("Users");      // For Swagger

users.MapGet("/", GetAllUsers);
users.MapGet("/{id}", GetUserById);
users.MapPost("/", CreateUser);
users.MapPut("/{id}", UpdateUser);
users.MapDelete("/{id}", DeleteUser);
```

---

## Controller API Examples

### RESTful Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    
    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }
    
    // GET api/users
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }
    
    // GET api/users/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }
    
    // POST api/users
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Create(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await _repository.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
    
    // DELETE api/users/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
}
```

### Action Filters

```csharp
// ✅ Reusable validation filter
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}

// Usage
[HttpPost]
[ValidateModel]
public async Task<ActionResult<User>> Create(CreateUserRequest request)
{
    // ModelState already validated
}
```

---

## Middleware Pipeline

### Order Matters!

```csharp
var app = builder.Build();

// ✅ Correct order
app.UseHttpsRedirection();       // 1. Redirect HTTP to HTTPS
app.UseStaticFiles();            // 2. Serve static files early
app.UseRouting();                // 3. Route matching
app.UseCors();                   // 4. CORS after routing
app.UseAuthentication();         // 5. WHO are you?
app.UseAuthorization();          // 6. WHAT can you do?
app.UseRateLimiter();            // 7. Rate limiting
app.MapControllers();            // 8. Execute endpoint
```

### Custom Middleware

```csharp
// ✅ Inline middleware
app.Use(async (context, next) =>
{
    var start = DateTimeOffset.UtcNow;
    await next();  // Call next middleware
    var elapsed = DateTimeOffset.UtcNow - start;
    
    Console.WriteLine($"Request took {elapsed.TotalMilliseconds}ms");
});

// ✅ Class-based middleware
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();
        
        _logger.LogInformation(
            "Request {Method} {Path} completed in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            sw.ElapsedMilliseconds);
    }
}

// Register
app.UseMiddleware<RequestTimingMiddleware>();
```

### Short-Circuiting

```csharp
// ✅ Stop pipeline early
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("OK");
        return;  // ✅ Don't call next()
    }
    
    await next();
});
```

---

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

## Best Practices

### ✅ API Design
- Use RESTful conventions (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 400, 404, 500)
- Use DTOs for requests/responses (don't expose domain models)
- Implement pagination for large collections
- Add API versioning from day 1

### ✅ Controllers
- Keep controllers thin (delegate to services)
- Use action filters for cross-cutting concerns
- Validate input with model binding and validation attributes
- Use ProducesResponseType for Swagger documentation
- Return ActionResult<T> for better type safety

### ✅ Middleware
- Order middleware correctly (see pipeline order above)
- Use short-circuiting when appropriate
- Keep middleware focused (single responsibility)
- Use scoped services via HttpContext.RequestServices

### ✅ Performance
- Use async/await for I/O operations
- Enable response compression
- Cache responses where appropriate
- Use output caching (ASP.NET Core 7+)
- Minimize allocations in hot paths

---

## Common Pitfalls

### ❌ Not Using Async/Await

```csharp
// ❌ BAD: Blocking call
public ActionResult<User> Get(int id)
{
    var user = _repository.GetByIdAsync(id).Result;  // ❌ Blocks thread
    return Ok(user);
}

// ✅ GOOD: Async all the way
public async Task<ActionResult<User>> Get(int id)
{
    var user = await _repository.GetByIdAsync(id);
    return Ok(user);
}
```

### ❌ Exposing Domain Models

```csharp
// ❌ BAD: Exposes internal structure
[HttpGet]
public ActionResult<Customer> Get()
{
    return Ok(_dbContext.Customers.First());  // ❌ Domain model
}

// ✅ GOOD: Use DTOs
[HttpGet]
public ActionResult<CustomerDto> Get()
{
    var customer = _dbContext.Customers.First();
    var dto = _mapper.Map<CustomerDto>(customer);
    return Ok(dto);
}
```

### ❌ Wrong Status Codes

```csharp
// ❌ BAD: Always returns 200
[HttpPost]
public ActionResult Create(User user)
{
    _repository.Add(user);
    return Ok(user);  // ❌ Should be 201 Created
}

// ✅ GOOD: Correct status code
[HttpPost]
public ActionResult<User> Create(User user)
{
    _repository.Add(user);
    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
}
```

---

## Related Files

- [WebAPI/MinimalAPI/MinimalAPIBestPractices.cs](../WebAPI/MinimalAPI/MinimalAPIBestPractices.cs)
- [WebAPI/ControllerAPI/WebAPIBestPractices.cs](../WebAPI/ControllerAPI/WebAPIBestPractices.cs)
- [WebAPI/MVC/MVCBestPractices.cs](../WebAPI/MVC/MVCBestPractices.cs)
- [WebAPI/Middleware/MiddlewareBestPractices.cs](../WebAPI/Middleware/MiddlewareBestPractices.cs)
- [WebAPI/Versioning/APIVersioningStrategies.cs](../WebAPI/Versioning/APIVersioningStrategies.cs)
- [WebAPI/README.md](../WebAPI/README.md)

---

## See Also

- [Security](Security.md) - Authentication and authorization
- [Practical Patterns](Practical-Patterns.md) - Validation, caching, error handling
- [Logging and Observability](Logging-Observability.md) - Request/response logging
- [Testing](Testing.md) - Integration testing APIs
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [API-Documentation.md](API-Documentation.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: A production API is more than CRUD; it needs explicit contracts, validation, auth boundaries, error semantics, and version strategy.
- 2-minute deep dive: I define request/response DTOs per endpoint, validate early, return consistent problem details, and use idempotency for retry-sensitive operations.
- Common follow-up: Minimal API vs controllers?
- Strong response: Minimal APIs for focused services and fast iteration; controllers for larger APIs needing richer conventions and cross-cutting filters.
- Tradeoff callout: Over-versioning too early increases maintenance overhead.

## Interview Bad vs Strong Answer

- Bad answer: "I know Web API MVC and I would just follow best practices."
- Strong answer: "For Web API MVC, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Web API MVC in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
