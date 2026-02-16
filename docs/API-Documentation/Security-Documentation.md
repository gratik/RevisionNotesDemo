# Security Documentation

> Subject: [API-Documentation](../README.md)

## Security Documentation

### JWT Bearer Authentication

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Define security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // ✅ Apply security globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

**Result**: "Authorize" button in Swagger UI where users can enter JWT token.

### API Key Authentication

```csharp
options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
{
    Description = "API Key needed to access endpoints. API-Key: {your key}",
    In = ParameterLocation.Header,
    Name = "API-Key",
    Type = SecuritySchemeType.ApiKey
});
```

---


