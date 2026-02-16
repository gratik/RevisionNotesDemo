# Testing with Swagger

> Subject: [API-Documentation](../README.md)

## Testing with Swagger

### Manual Testing

1. **Navigate to Swagger UI**: `http://localhost:5000/`
2. **Authenticate** (if required): Click "Authorize", enter token
3. **Expand endpoint**: Click on GET/POST/etc.
4. **Try it out**: Click "Try it out" button
5. **Enter parameters**: Fill in required/optional parameters
6. **Execute**: Click "Execute" button
7. **Review response**: Check status code, headers, body

### Automated Testing

```csharp
// Test that Swagger JSON is generated correctly
public class SwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SwaggerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_JSON_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"openapi\":", content);
    }

    [Fact]
    public async Task Swagger_UI_IsAccessible()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/swagger/index.html");
        response.EnsureSuccessStatusCode();
    }
}
```

---


