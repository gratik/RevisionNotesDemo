# Response Type Documentation

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Response Type Documentation before implementation work begins.
- Keep boundaries explicit so Response Type Documentation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Response Type Documentation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Response Type Documentation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Response Type Documentation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Response Type Documentation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Response Type Documentation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

