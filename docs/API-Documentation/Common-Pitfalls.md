# Common Pitfalls

> Subject: [API-Documentation](../README.md)

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


