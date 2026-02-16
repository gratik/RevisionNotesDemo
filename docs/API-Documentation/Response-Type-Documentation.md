# Response Type Documentation

> Subject: [API-Documentation](../README.md)

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


