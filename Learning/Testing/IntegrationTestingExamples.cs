// ==============================================================================
// INTEGRATION TESTING - Testing Multiple Components Together
// Reference: Revision Notes - Unit Testing Best Practices
// ==============================================================================
// WHAT IS IT?
// -----------
// End-to-end component tests that exercise multiple layers together (HTTP pipeline,
// DI, middleware, and real data stores) using tools like WebApplicationFactory.
//
// WHY IT MATTERS
// --------------
// ✅ REALISTIC COVERAGE: Verifies wiring between controllers, services, and data
// ✅ CONFIDENCE: Catches configuration and middleware issues unit tests miss
// ✅ REGRESSION SAFETY: Ensures infrastructure changes don't break behavior
// ✅ DEPLOYMENT READINESS: Validates production-like scenarios
//
// WHEN TO USE
// -----------
// ✅ API endpoints and middleware pipelines
// ✅ Database access and EF Core mappings
// ✅ Authentication/authorization flows
// ✅ Critical business workflows spanning multiple components
//
// WHEN NOT TO USE
// ---------------
// ❌ Pure logic that can be validated with fast unit tests
// ❌ Every code path (integration tests are slower; be selective)
// ❌ Scenarios requiring heavy external dependencies without isolation
//
// REAL-WORLD EXAMPLE
// ------------------
// Order checkout flow:
// - HTTP request -> controller -> service -> database
// - Tests validate response status, persisted data, and auth rules
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevisionNotesDemo.Testing;

// Sample API entities
public class IntegrationProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class IntegrationTestingExamples
{
    /// <summary>
    /// EXAMPLE 1: WebApplicationFactory Pattern
    /// Tests entire HTTP pipeline
    /// </summary>
    public static void WebApplicationFactoryExample()
    {
        Console.WriteLine("\n=== EXAMPLE 1: WebApplicationFactory ===");

        Console.WriteLine("\n📦 Installation:");
        Console.WriteLine("   dotnet add package Microsoft.AspNetCore.Mvc.Testing");

        Console.WriteLine("\n📝 Custom Factory:");
        Console.WriteLine(@"
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            
            // Add in-memory database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(""TestDb"");
            });
            
            // Seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }
}");

        Console.WriteLine("\n✅ Benefit: Test with real HTTP, controllers, DI, middleware");
    }

    /// <summary>
    /// EXAMPLE 2: Complete HTTP Integration Test
    /// </summary>
    public static async Task HttpIntegrationTestExample()
    {
        Console.WriteLine("\n=== EXAMPLE 2: HTTP Integration Test ===");

        Console.WriteLine("\n📝Test Class:");
        Console.WriteLine(@"
public class ProductsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ProductsIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Get_Products_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync(""/api/products"");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products);
    }
    
    [Fact]
    public async Task Post_CreateProduct_Returns201Created()
    {
        // Arrange
        var newProduct = new Product { Name = ""Test Product"", Price = 99.99m };
        
        // Act
        var response = await _client.PostAsJsonAsync(""/api/products"", newProduct);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Product>();
        Assert.Equal(newProduct.Name, created.Name);
    }
}");

        Console.WriteLine("\n✅ Tests: Real HTTP requests, full pipeline, controllers, DI");
    }

    /// <summary>
    /// EXAMPLE 3: Authentication in Integration Tests
    /// </summary>
    public static void AuthenticationExample()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Authentication in Tests ===");

        Console.WriteLine("\n📝 Test Auth Handler:");
        Console.WriteLine(@"
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                          ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, ""TestUser""),
            new Claim(ClaimTypes.NameIdentifier, ""1""),
            new Claim(ClaimTypes.Role, ""Admin"")
        };
        
        var identity = new ClaimsIdentity(claims, ""Test"");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ""Test"");
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}");

        Console.WriteLine("\n📝 Configure in Factory:");
        Console.WriteLine(@"
services.AddAuthentication(""Test"")
    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(""Test"", options => { });");

        Console.WriteLine("\n📝 Use in Tests:");
        Console.WriteLine(@"
_client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue(""Test"");");

        Console.WriteLine("\n✅ Benefit: Test authorized endpoints without real auth");
    }

    /// <summary>
    /// EXAMPLE 4: Database Integration Testing
    /// </summary>
    public static void DatabaseIntegrationExample()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Database Integration Testing ===");

        Console.WriteLine("\n📝 In-Memory Database:");
        Console.WriteLine(@"
public class RepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    
    public RepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
    }
    
    [Fact]
    public async Task Repository_AddAndRetrieve_Works()
    {
        // Arrange
        var repository = new ProductRepository(_context);
        var product = new Product { Name = ""Test"", Price = 99.99m };
        
        // Act
        await repository.AddAsync(product);
        var retrieved = await repository.GetByIdAsync(product.Id);
        
        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(product.Name, retrieved.Name);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}");

        Console.WriteLine("\n✅ Test repositories with real database operations");
    }

    /// <summary>
    /// EXAMPLE 5: Using Real Database (SQLite)
    /// </summary>
    public static void RealDatabaseExample()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Real Database (SQLite) ===");

        Console.WriteLine("\n📦 Installation:");
        Console.WriteLine("   dotnet add package Microsoft.EntityFrameworkCore.Sqlite");

        Console.WriteLine("\n📝 SQLite In-Memory:");
        Console.WriteLine(@"
var connection = new SqliteConnection(""DataSource=:memory:"");
connection.Open();

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .Options;

using var context = new AppDbContext(options);
context.Database.EnsureCreated();

// Run tests...

connection.Close();");

        Console.WriteLine("\n✅ Benefit: Test with real SQL, not EF in-memory limitations");
    }

    /// <summary>
    /// EXAMPLE 6: Respawn Library (Database Cleanup)
    /// </summary>
    public static void RespawnExample()
    {
        Console.WriteLine("\n=== EXAMPLE 6: Respawn (Database Cleanup) ===");

        Console.WriteLine("\n📦 Installation:");
        Console.WriteLine("   dotnet add package Respawn");

        Console.WriteLine("\n📝 Usage:");
        Console.WriteLine(@"
private static Respawner _respawner;

public async Task InitializeAsync()
{
    _respawner = await Respawner.CreateAsync(_connectionString);
}

public async Task ResetDatabase()
{
    await _respawner.ResetAsync(_connectionString);
}

// Call ResetDatabase() before each test
[Fact]
public async Task Test1()
{
    await ResetDatabase();
    // Test with clean database
}");

        Console.WriteLine("\n✅ Benefit: Fast database reset between tests");
    }

    /// <summary>
    /// EXAMPLE 7: TestContainers (Real Databases in Docker)
    /// </summary>
    public static void TestContainersExample()
    {
        Console.WriteLine("\n=== EXAMPLE 7: TestContainers (Docker) ===");

        Console.WriteLine("\n📦 Installation:");
        Console.WriteLine("   dotnet add package Testcontainers.MsSql");

        Console.WriteLine("\n📝 Usage:");
        Console.WriteLine(@"
public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    public string ConnectionString { get; private set; }
    
    public DatabaseFixture()
    {
        _container = new MsSqlBuilder().Build();
    }
    
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
    }
    
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

public class IntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public IntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Test_WithRealDatabase()
    {
        // Use _fixture.ConnectionString
        // Test with real SQL Server in Docker!
    }
}");

        Console.WriteLine("\n✅ Benefit: Test with REAL database (SQL Server, PostgreSQL, etc.)");
    }

    /// <summary>
    /// Best Practices
    /// </summary>
    public static void BestPractices()
    {
        Console.WriteLine("\n=== INTEGRATION TESTING - BEST PRACTICES ===");
        Console.WriteLine("✅ Use in-memory database for simple tests");
        Console.WriteLine("✅ Use SQLite in-memory for real SQL");
        Console.WriteLine("✅ Use TestContainers for real database testing");
        Console.WriteLine("✅ Isolate tests (clean database between tests)");
        Console.WriteLine("✅ Test realistic scenarios");
        Console.WriteLine("✅ Don't over-mock in integration tests");
        Console.WriteLine("✅ Separate integration tests from unit tests");
        Console.WriteLine("✅ Use WebApplicationFactory for API testing");
        Console.WriteLine("✅ Test authentication/authorization flows");
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== INTEGRATION TESTING EXAMPLES ===\n");
        WebApplicationFactoryExample();
        await HttpIntegrationTestExample();
        AuthenticationExample();
        DatabaseIntegrationExample();
        RealDatabaseExample();
        RespawnExample();
        TestContainersExample();
        BestPractices();
        Console.WriteLine("\nIntegration Testing examples completed!\n");
    }
}
