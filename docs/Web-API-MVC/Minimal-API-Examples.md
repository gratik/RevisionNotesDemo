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


