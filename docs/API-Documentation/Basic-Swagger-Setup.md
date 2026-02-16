# Basic Swagger Setup

> Subject: [API-Documentation](../README.md)

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


