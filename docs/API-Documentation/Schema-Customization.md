# Schema Customization

> Subject: [API-Documentation](../README.md)

## Schema Customization

### Custom Schema Examples

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // ✅ Add examples to schemas
    options.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Example = new OpenApiString("2026-02-14T10:30:00Z")
    });

    // ✅ Customize enums
    options.SchemaFilter<EnumSchemaFilter>();

    // ✅ Hide internal properties
    options.SchemaFilter<HideInternalPropertiesFilter>();
});

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var name in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
```

### Request/Response Examples

```csharp
/// <summary>
/// User registration request
/// </summary>
/// <example>
/// {
///   "email": "john.doe@example.com",
///   "password": "SecureP@ssw0rd!",
///   "firstName": "John",
///   "lastName": "Doe"
/// }
/// </example>
public class RegisterUserRequest
{
    /// <summary>
    /// Email address (must be unique)
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password (min 8 chars, must contain uppercase, lowercase, digit, special char)
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
```

---


