# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET Core request pipeline and routing fundamentals.
- Related examples: docs/Web-API-MVC/README.md
> Subject: [Web-API-MVC](../README.md)

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

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about ASP.NET endpoint architecture patterns. It matters because architecture choices affect testability, throughput, and maintainability.
- Use it when selecting minimal API, controller API, or MVC by problem shape.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer speed vs explicit control and extensibility.
- Close with one failure mode and mitigation: mixing styles without clear boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.