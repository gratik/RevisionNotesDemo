// ==============================================================================
// MINIMAL API BEST PRACTICES - Modern ASP.NET Core Endpoints
// ==============================================================================
//
// WHAT IS IT?
// -----------
// Minimal APIs provide a lightweight, function-first way to define HTTP endpoints
// with less boilerplate and improved performance.
//
// WHY IT MATTERS
// --------------
// - Faster startup and request handling
// - Less ceremony for small services
// - Easy to read and maintain for simple APIs
//
// WHEN TO USE
// -----------
// - YES: Microservices and small APIs
// - YES: Simple routing and minimal infrastructure needs
// - YES: Performance-sensitive endpoints
//
// WHEN NOT TO USE
// ---------------
// - NO: Large APIs with complex filters and conventions
// - NO: When controller-based structure is required
//
// REAL-WORLD EXAMPLE
// ------------------
// Product pricing service:
// - A few endpoints for price lookup and updates
// - Simple routing and DI
// - Minimal overhead, easy to deploy
// ==============================================================================

namespace RevisionNotesDemo.WebAPI.MinimalAPI;

/// <summary>
/// EXAMPLE 1: BASIC ENDPOINT DEFINITION
/// 
/// THE PROBLEM:
/// Defining endpoints without clear intent, mixing concerns,
/// and not following REST conventions.
/// 
/// THE SOLUTION:
/// - Use descriptive route patterns
/// - Follow REST conventions (GET for reads, POST for creates, etc.)
/// - Return appropriate HTTP status codes
/// - Use RouteGrouping for related endpoints
/// 
/// WHY IT MATTERS:
/// - Clear API contracts reduce integration issues
/// - Proper HTTP verbs enable caching and browser behavior
/// - Status codes communicate intent to clients
/// 
/// BEST PRACTICE: Group related endpoints with MapGroup()
/// </summary>
public static class BasicEndpointExamples
{
    public static void MapBasicEndpoints(this WebApplication app)
    {
        // ❌ BAD: Unclear route, wrong HTTP verb, no status codes
        // app.MapPost("/data", () => GetAllProducts());

        // ✅ GOOD: Clear route, correct verb, proper status codes
        app.MapGet("/api/products", () =>
        {
            var products = GetAllProducts();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .WithTags("Products")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        // ✅ GOOD: Route parameters with validation
        app.MapGet("/api/products/{id:int}", (int id) =>
        {
            var product = GetProductById(id);
            return product is not null
                ? Results.Ok(product)
                : Results.NotFound($"Product {id} not found");
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // ✅ GOOD: Using route groups for organization
        var productsGroup = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        productsGroup.MapPost("/", (Product product) =>
        {
            // Validation would happen here
            var created = CreateProduct(product);
            return Results.Created($"/api/products/{created.Id}", created);
        })
        .Produces<Product>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }

    private static List<Product> GetAllProducts() => new();
    private static Product? GetProductById(int id) => null;
    private static Product CreateProduct(Product product) => product with { Id = 1 };
}

/// <summary>
/// EXAMPLE 2: DEPENDENCY INJECTION IN ENDPOINTS
/// 
/// THE PROBLEM:
/// Manually creating dependencies, hardcoding connections,
/// or using service locator pattern in endpoints.
/// 
/// THE SOLUTION:
/// - Inject services as endpoint parameters
/// - Use interface types for testability
/// - Register services in Program.cs
/// - Leverage constructor injection for complex dependencies
/// 
/// WHY IT MATTERS:
/// - Testable code without tight coupling
/// - Easier to swap implementations
/// - Follows SOLID principles
/// - Built-in lifetime management
/// 
/// PERFORMANCE IMPACT: DI overhead is negligible (~1-2μs per request)
/// </summary>
public static class DependencyInjectionExamples
{
    public static void MapDIEndpoints(this WebApplication app)
    {
        // ❌ BAD: Manual instantiation, tight coupling
        // app.MapGet("/api/orders", () =>
        // {
        //     var dbContext = new OrderDbContext(); // Manual creation!
        //     var orders = dbContext.Orders.ToList();
        //     return Results.Ok(orders);
        // });

        // ✅ GOOD: Inject dependencies as parameters
        app.MapGet("/api/orders", (IOrderService orderService) =>
        {
            var orders = orderService.GetAllOrders();
            return Results.Ok(orders);
        });

        // ✅ GOOD: Multiple dependencies
        app.MapPost("/api/orders", async (
            Order order,
            IOrderService orderService,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            logger.LogInformation("Creating order for customer {CustomerId}", order.CustomerId);

            var created = await orderService.CreateOrderAsync(order, ct);
            return Results.Created($"/api/orders/{created.Id}", created);
        })
        .WithName("CreateOrder");

        // ✅ GOOD: Injecting HttpContext when needed
        app.MapGet("/api/me", (HttpContext context, IUserService userService) =>
        {
            var userId = context.User.FindFirst("sub")?.Value;
            if (userId is null) return Results.Unauthorized();

            var user = userService.GetUserById(userId);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        })
        .RequireAuthorization();
    }
}

/// <summary>
/// EXAMPLE 3: REQUEST VALIDATION AND ERROR HANDLING
/// 
/// THE PROBLEM:
/// Not validating input, returning 500 for validation errors,
/// exposing stack traces, and inconsistent error responses.
/// 
/// THE SOLUTION:
/// - Validate input using FluentValidation or DataAnnotations
/// - Return 400 Bad Request for validation errors
/// - Use Problem Details for structured errors
/// - Handle exceptions with middleware
/// 
/// WHY IT MATTERS:
/// - Prevent bad data from entering the system
/// - Clear error messages for API consumers
/// - Security (don't expose internals)
/// - Consistent error structure across API
/// 
/// TIP: Use FluentValidation for complex validation logic
/// </summary>
public static class ValidationExamples
{
    public static void MapValidationEndpoints(this WebApplication app)
    {
        // ❌ BAD: No validation, exceptions leak
        // app.MapPost("/api/users", (User user) =>
        // {
        //     var created = CreateUser(user); // What if user is invalid?
        //     return Results.Ok(created);
        // });

        // ✅ GOOD: Manual validation with clear response
        app.MapPost("/api/users", (User user) =>
        {
            var errors = ValidateUser(user);
            if (errors.Any())
            {
                return Results.ValidationProblem(errors);
            }

            var created = CreateUser(user);
            return Results.Created($"/api/users/{created.Id}", created);
        })
        .Produces<User>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        // ✅ GOOD: Using IValidator<T> from FluentValidation
        app.MapPost("/api/users/validated", async (
            User user,
            IValidator<User> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                return Results.ValidationProblem(errors);
            }

            var created = CreateUser(user);
            return Results.Created($"/api/users/{created.Id}", created);
        });

        // ✅ GOOD: Handling specific exceptions
        app.MapGet("/api/users/{id:int}", (int id, IUserService userService) =>
        {
            try
            {
                var user = userService.GetUserById(id.ToString());
                return user is not null
                    ? Results.Ok(user)
                    : Results.NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Problem(
                    title: "Access Denied",
                    statusCode: StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                // Log exception here
                return Results.Problem(
                    title: "An error occurred",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });
    }

    private static Dictionary<string, string[]> ValidateUser(User user)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrEmpty(user.Name))
            errors["name"] = new[] { "Name is required" };
        if (string.IsNullOrEmpty(user.Email))
            errors["email"] = new[] { "Email is required" };
        return errors;
    }

    private static User CreateUser(User user) => user with { Id = "1" };
}

/// <summary>
/// EXAMPLE 4: ASYNC OPERATIONS AND CANCELLATION
/// 
/// THE PROBLEM:
/// Blocking async operations with .Result or .Wait(),
/// not supporting cancellation, or not using async all the way.
/// 
/// THE SOLUTION:
/// - Always use async/await for I/O operations
/// - Accept CancellationToken parameter
/// - Pass token through the call chain
/// - Return Task<IResult> for async endpoints
/// 
/// WHY IT MATTERS:
/// - Better thread pool utilization (10x+ more concurrent requests)
/// - Graceful shutdown support
/// - Client can cancel long-running requests
/// - Prevents thread starvation
/// 
/// PERFORMANCE: Async I/O frees threads - one thread can handle 1000+ concurrent requests
/// </summary>
public static class AsyncExamples
{
    public static void MapAsyncEndpoints(this WebApplication app)
    {
        // ❌ BAD: Blocking async code
        // app.MapGet("/api/data", (IDataService dataService) =>
        // {
        //     var data = dataService.GetDataAsync().Result; // BLOCKS thread!
        //     return Results.Ok(data);
        // });

        // ✅ GOOD: Proper async/await
        app.MapGet("/api/data", async (IDataService dataService) =>
        {
            var data = await dataService.GetDataAsync();
            return Results.Ok(data);
        });

        // ✅ GOOD: With cancellation token support
        app.MapGet("/api/data/async", async (
            IDataService dataService,
            CancellationToken cancellationToken) =>
        {
            var data = await dataService.GetDataAsync(cancellationToken);
            return Results.Ok(data);
        });

        // ✅ GOOD: Long-running operation with cancellation
        app.MapPost("/api/reports", async (
            ReportRequest request,
            IReportService reportService,
            CancellationToken ct) =>
        {
            // Client can cancel by closing connection
            var report = await reportService.GenerateReportAsync(request, ct);
            return Results.Ok(report);
        })
        .WithRequestTimeout(TimeSpan.FromMinutes(5)); // .NET 8+
    }
}

/// <summary>
/// EXAMPLE 5: AUTHENTICATION AND AUTHORIZATION
/// 
/// THE PROBLEM:
/// No authentication, hardcoded authorization logic,
/// exposing sensitive data, or inconsistent security.
/// 
/// THE SOLUTION:
/// - Use .RequireAuthorization() for protected endpoints
/// - Define authorization policies
/// - Use roles and claims for fine-grained access
/// - Validate JWT tokens properly
/// 
/// WHY IT MATTERS:
/// - Security is not optional
/// - Proper authZ prevents data breaches
/// - Policies are testable and reusable
/// - Compliance requirements (GDPR, etc.)
/// 
/// GOTCHA: RequireAuthorization() without authentication middleware will always return 401
/// </summary>
public static class AuthorizationExamples
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // ❌ BAD: No authorization
        // app.MapGet("/api/admin/users", () => GetAllUsers());

        // ✅ GOOD: Simple authorization (any authenticated user)
        app.MapGet("/api/profile", (HttpContext context) =>
        {
            var userName = context.User.Identity?.Name;
            return Results.Ok(new { userName });
        })
        .RequireAuthorization();

        // ✅ GOOD: Role-based authorization
        app.MapGet("/api/admin/users", () =>
        {
            var users = GetAllUsers();
            return Results.Ok(users);
        })
        .RequireAuthorization("AdminOnly");

        // ✅ GOOD: Policy-based authorization
        app.MapDelete("/api/orders/{id:int}", (int id, IOrderService orderService) =>
        {
            orderService.DeleteOrder(id);
            return Results.NoContent();
        })
        .RequireAuthorization(policy =>
            policy.RequireRole("Admin", "Manager")
                  .RequireClaim("permission", "orders:delete"));

        // ✅ GOOD: Custom authorization with claims
        app.MapPut("/api/users/{id}", (
            string id,
            User user,
            HttpContext context) =>
        {
            var currentUserId = context.User.FindFirst("sub")?.Value;

            // User can only update their own profile
            if (currentUserId != id && !context.User.IsInRole("Admin"))
            {
                return Results.Forbid();
            }

            var updated = UpdateUser(id, user);
            return Results.Ok(updated);
        })
        .RequireAuthorization();
    }

    private static List<User> GetAllUsers() => new();
    private static User UpdateUser(string id, User user) => user;
}

// Supporting classes
public record Product(int Id, string Name, decimal Price, string Description);
public record Order(int Id, string CustomerId, decimal Total, List<OrderItem> Items);
public record OrderItem(int ProductId, int Quantity, decimal Price);
public record User(string Id, string Name, string Email, string Role);
public record ReportRequest(DateTime StartDate, DateTime EndDate, string Type);

public interface IOrderService
{
    List<Order> GetAllOrders();
    Task<Order> CreateOrderAsync(Order order, CancellationToken ct);
    void DeleteOrder(int id);
}

public interface IUserService
{
    User? GetUserById(string id);
}

public interface IDataService
{
    Task<object> GetDataAsync(CancellationToken ct = default);
}

public interface IReportService
{
    Task<object> GenerateReportAsync(ReportRequest request, CancellationToken ct);
}

public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationFailure> Errors { get; set; } = new();
}

public class ValidationFailure
{
    public string PropertyName { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
}
