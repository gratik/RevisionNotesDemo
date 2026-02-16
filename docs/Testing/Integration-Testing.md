# Integration Testing

> Subject: [Testing](../README.md)

## Integration Testing

### WebApplicationFactory (ASP.NET Core)

```csharp
public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
```

### In-Memory Database

```csharp
public class RepositoryIntegrationTests
{
    [Fact]
    public async Task SaveUser_ValidUser_Success()
    {
        // Arrange: In-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        using var context = new AppDbContext(options);
        var repository = new UserRepository(context);
        
        // Act
        var user = new User { Name = "Alice" };
        await repository.SaveAsync(user);
        
        // Assert
        var saved = await context.Users.FindAsync(user.Id);
        Assert.NotNull(saved);
        Assert.Equal("Alice", saved.Name);
    }
}
```

---


