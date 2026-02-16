# Backend Patterns (.NET)

> Subject: [DotNet-API-React](../README.md)

## Backend Patterns (.NET)

### API contract and error envelope

```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IReadOnlyList<OrderDto>> Get(CancellationToken ct)
        => await _service.GetOrdersAsync(ct);
}
```

### CORS with explicit origins

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaPolicy", policy => policy
        .WithOrigins("http://localhost:5173", "https://app.example.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

app.UseRouting();
app.UseCors("SpaPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```


