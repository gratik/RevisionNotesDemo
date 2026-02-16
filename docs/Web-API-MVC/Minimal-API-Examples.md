# Minimal API Examples

> Subject: [Web-API-MVC](../README.md)

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

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Minimal API Examples before implementation work begins.
- Keep boundaries explicit so Minimal API Examples decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Minimal API Examples in production-facing code.
- When performance, correctness, or maintainability depends on consistent Minimal API Examples decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Minimal API Examples as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Minimal API Examples is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Minimal API Examples are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

