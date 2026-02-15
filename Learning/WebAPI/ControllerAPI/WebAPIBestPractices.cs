// ==============================================================================
// WEB API CONTROLLER BEST PRACTICES - Traditional ASP.NET Core Controllers
// ==============================================================================
//
// WHAT IS IT?
// -----------
// A structured, controller-based approach to building Web APIs with routing,
// filters, model binding, and validation baked in.
//
// WHY IT MATTERS
// --------------
// - Scales well for large APIs with many endpoints
// - Supports rich filters and cross-cutting concerns
// - Clear conventions for routing and response types
// - Mature testing and tooling support
//
// WHEN TO USE
// -----------
// - YES: Large or complex APIs
// - YES: Need action filters, model validation, or MVC features
// - YES: Team prefers OOP patterns and class-based structure
//
// WHEN NOT TO USE
// ---------------
// - NO: Very small services with only a few endpoints
// - NO: When minimal API provides simpler routing and less boilerplate
//
// REAL-WORLD EXAMPLE
// ------------------
// Enterprise product catalog API:
// - Controllers for Products, Orders, Customers
// - Filters for auth, logging, and validation
// - Versioned endpoints for backward compatibility
// ==============================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RevisionNotesDemo.WebAPI.ControllerAPI;

/// <summary>
/// EXAMPLE 1: BASIC CONTROLLER STRUCTURE
/// 
/// THE PROBLEM:
/// Controllers without proper attributes, inconsistent routing,
/// missing HTTP verb attributes, and poor action naming.
/// 
/// THE SOLUTION:
/// - Use [ApiController] attribute for automatic model validation
/// - Define route template with [Route] attribute
/// - Use HTTP verb attributes ([HttpGet], [HttpPost], etc.)
/// - Follow RESTful conventions
/// 
/// WHY IT MATTERS:
/// - [ApiController] provides automatic 400 responses
/// - Consistent routing improves API usability
/// - Proper verbs enable HTTP semantics
/// - Self-documenting API through conventions
/// 
/// BEST PRACTICE: Always use [ApiController] for Web APIs
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    // ❌ BAD: No HTTP verb, unclear route
    // public IActionResult Get() => Ok(GetAll());

    // ✅ GOOD: Clear HTTP verb and route
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public ActionResult<List<Product>> GetAll()
    {
        _logger.LogInformation("Getting all products");
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    // ✅ GOOD: Route parameter with constraint
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound($"Product {id} not found");
        }

        return Ok(product);
    }

    // ✅ GOOD: POST with model binding and validation
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> Create([FromBody] CreateProductDto dto)
    {
        // [ApiController] validates ModelState automatically
        // If invalid, returns 400 with validation errors

        var product = _productService.CreateProduct(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    // ✅ GOOD: PUT with validation
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Update(int id, [FromBody] UpdateProductDto dto)
    {
        var exists = _productService.ProductExists(id);
        if (!exists)
        {
            return NotFound();
        }

        _productService.UpdateProduct(id, dto);
        return NoContent(); // 204 is standard for successful PUT
    }

    // ✅ GOOD: DELETE
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        var exists = _productService.ProductExists(id);
        if (!exists)
        {
            return NotFound();
        }

        _productService.DeleteProduct(id);
        return NoContent();
    }
}

/// <summary>
/// EXAMPLE 2: MODEL BINDING AND VALIDATION
/// 
/// THE PROBLEM:
/// Manual model validation, inconsistent error responses,
/// not using Data Annotations, or accepting any input.
/// 
/// THE SOLUTION:
/// - Use Data Annotations attributes
/// - [ApiController] provides automatic validation
/// - Custom validation attributes for complex rules
/// - FluentValidation for advanced scenarios
/// 
/// WHY IT MATTERS:
/// - Data Annotations are declarative and clear
/// - Automatic 400 responses save code
/// - Consistent validation across application
/// - Self-documenting models
/// 
/// PERFORMANCE IMPACT: Validation overhead is ~10-50μs per request
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ValidationExamplesController : ControllerBase
{
    // [ApiController] automatically validates this
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] CreateUserDto dto)
    {
        // No need to check ModelState - [ApiController] does it!
        // If invalid, returns 400 automatically with error details

        var user = CreateUserFromDto(dto);
        return CreatedAtAction(null, null, user);
    }

    // ❌ BAD: Manual ModelState check (unnecessary with [ApiController])
    [HttpPost("manual")]
    public ActionResult<User> CreateUserManual([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = CreateUserFromDto(dto);
        return Ok(user);
    }

    // ✅ GOOD: Multiple binding sources
    [HttpGet("{id}/orders")]
    public ActionResult GetUserOrders(
        [FromRoute] int id,              // From URL path
        [FromQuery] int page = 1,        // From query string
        [FromQuery] int pageSize = 10,   // From query string
        [FromHeader(Name = "X-API-Key")] string? apiKey = null) // From header
    {
        // Validate page parameters
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination parameters");
        }

        var orders = GetOrders(id, page, pageSize);
        return Ok(orders);
    }

    private User CreateUserFromDto(CreateUserDto dto) =>
        new User { Id = 1, Name = dto.Name, Email = dto.Email };

    private List<object> GetOrders(int id, int page, int pageSize) => new();
}

/// <summary>
/// EXAMPLE 3: ACTION FILTERS AND MIDDLEWARE
/// 
/// THE PROBLEM:
/// Duplicated cross-cutting logic (logging, caching, validation),
/// inconsistent error handling, or performance issues.
/// 
/// THE SOLUTION:
/// - Use action filters for reusable logic
/// - Apply filters globally or per-action
/// - Create custom filters for specific needs
/// - Use middleware for pipeline concerns
/// 
/// WHY IT MATTERS:
/// - DRY principle - write once, apply everywhere
/// - Separation of concerns
/// - Testable cross-cutting logic
/// - Better maintainability
/// 
/// EXAMPLE FILTERS: Logging, caching, rate limiting, authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // ✅ GOOD: Custom authorization filter
    [HttpGet]
    [RequirePermission("orders:read")]
    public ActionResult<List<Order>> GetAll() => Ok(new List<Order>());

    // ✅ GOOD: Multiple filters applied
    [HttpPost]
    [RequirePermission("orders:create")]
    [ValidateModel]
    [LogPerformance]
    public ActionResult<Order> Create([FromBody] CreateOrderDto dto)
    {
        var order = CreateOrder(dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // ✅ GOOD: Async action with cancellation
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await GetOrderAsync(id, cancellationToken);
        return order != null ? Ok(order) : NotFound();
    }

    private Order CreateOrder(CreateOrderDto dto) => new Order(1);
    private async Task<Order?> GetOrderAsync(int id, CancellationToken ct)
    {
        await Task.Delay(10, ct);
        return new Order(id);
    }
}

/// <summary>
/// EXAMPLE 4: ERROR HANDLING AND PROBLEM DETAILS
/// 
/// THE PROBLEM:
/// Inconsistent error responses, exposing stack traces,
/// returning 500 for validation errors, or no error details.
/// 
/// THE SOLUTION:
/// - Use Problem Details (RFC 7807) for errors
/// - Return appropriate status codes
/// - Use exception handling middleware
/// - Log errors but don't expose details
/// 
/// WHY IT MATTERS:
/// - Consistent error format for consumers
/// - Security (don't leak implementation details)
/// - Better debugging with structured errors
/// - Industry standard format
/// 
/// BEST PRACTICE: Use ProblemDetails for all error responses
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ✅ GOOD: Using Problem() for errors
    [HttpGet("{id:int}")]
    public ActionResult<User> GetUser(int id)
    {
        try
        {
            var user = GetUserById(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"User with ID {id} was not found",
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(user);
        }
        catch (UnauthorizedAccessException)
        {
            return Problem(
                title: "Access denied",
                statusCode: StatusCodes.Status403Forbidden,
                detail: "You don't have permission to access this user");
        }
        catch (Exception)
        {
            // Log exception (don't expose to client)
            return Problem(
                title: "An error occurred",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ✅ GOOD: ValidationProblem for model errors
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] CreateUserDto dto)
    {
        if (UserExists(dto.Email))
        {
            ModelState.AddModelError("email", "Email already exists");
            return ValidationProblem(ModelState);
        }

        var user = CreateUserInternal(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    private User? GetUserById(int id) => null;
    private bool UserExists(string email) => false;
    private User CreateUserInternal(CreateUserDto dto) =>
        new User { Id = 1, Name = dto.Name, Email = dto.Email };
}

/// <summary>
/// EXAMPLE 5: ASYNC/AWAIT AND CANCELLATION
/// 
/// THE PROBLEM:
/// Blocking async code, not supporting cancellation,
/// or not using async all the way through.
/// 
/// THE SOLUTION:
/// - Use async/await for I/O operations
/// - Accept CancellationToken parameters
/// - Pass tokens through the call chain
/// - Return Task<ActionResult<T>>
/// 
/// WHY IT MATTERS:
/// - Better scalability (1000+ concurrent requests per thread)
/// - Graceful shutdown support
/// - Client cancellation support
/// - No thread blocking
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataService _dataService;

    public DataController(IDataService dataService) => _dataService = dataService;

    // ❌ BAD: Blocking async with .Result
    // [HttpGet]
    // public ActionResult<Data> GetData()
    // {
    //     var data = _dataService.GetDataAsync().Result; // BLOCKS!
    //     return Ok(data);
    // }

    // ✅ GOOD: Proper async/await
    [HttpGet]
    public async Task<ActionResult<Data>> GetData(CancellationToken cancellationToken)
    {
        var data = await _dataService.GetDataAsync(cancellationToken);
        return Ok(data);
    }

    // ✅ GOOD: Long-running operation with timeout
    [HttpPost("report")]
    [RequestTimeout(300000)] // 5 minutes
    public async Task<ActionResult<Report>> GenerateReport(
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        var report = await _dataService.GenerateReportAsync(request, cancellationToken);
        return Ok(report);
    }
}

// Supporting classes and interfaces
public record Product(int Id, string Name, decimal Price);
public record CreateProductDto(string Name, decimal Price);
public record UpdateProductDto(string Name, decimal Price);

public record User
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
}

public record CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; init; } = "";
}

public record Order(int Id);
public record CreateOrderDto(int CustomerId, List<int> ProductIds);
public record Data(string Value);
public record Report(string Content);
public record ReportRequest(DateTime Start, DateTime End);

public interface IProductService
{
    List<Product> GetAllProducts();
    Product? GetProductById(int id);
    Product CreateProduct(CreateProductDto dto);
    void UpdateProduct(int id, UpdateProductDto dto);
    void DeleteProduct(int id);
    bool ProductExists(int id);
}

public interface IDataService
{
    Task<Data> GetDataAsync(CancellationToken ct);
    Task<Report> GenerateReportAsync(ReportRequest request, CancellationToken ct);
}

// Custom filter attributes (example definitions)
public class RequirePermissionAttribute : Attribute
{
    public RequirePermissionAttribute(string permission) { }
}

public class ValidateModelAttribute : ActionFilterAttribute { }
public class LogPerformanceAttribute : ActionFilterAttribute { }
public class RequestTimeoutAttribute : Attribute
{
    public RequestTimeoutAttribute(int milliseconds) { }
}
