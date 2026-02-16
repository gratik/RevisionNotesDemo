# Backend Patterns (.NET)

> Subject: [DotNet-API-Vue](../README.md)

## Backend Patterns (.NET)

### SPA-friendly query endpoint

```csharp
[HttpGet]
public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> Search(
    [FromQuery] string? q,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 25,
    CancellationToken ct = default)
{
    if (page <= 0 || pageSize is < 1 or > 100)
    {
        return ValidationProblem(new Dictionary<string, string[]>
        {
            ["pagination"] = new[] { "Invalid page or pageSize." }
        });
    }

    var result = await _service.SearchAsync(q, page, pageSize, ct);
    return Ok(result);
}
```

### ProblemDetails for predictable client handling

```csharp
builder.Services.AddProblemDetails();
app.UseExceptionHandler();
```


