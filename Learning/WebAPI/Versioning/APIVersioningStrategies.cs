// ==============================================================================
// API VERSIONING STRATEGIES - ASP.NET Core Web API
// ==============================================================================
//
// WHAT ARE THEY?
// --------------
// Strategies to expose API versions via URL path, query string, headers, or
// media types so you can evolve APIs without breaking clients.
//
// WHY IT MATTERS
// --------------
// - Allows breaking changes while keeping old clients working
// - Enables gradual migration and deprecation
// - Improves API longevity and trust
//
// WHEN TO USE
// -----------
// - YES: Public APIs and partner integrations
// - YES: APIs with multiple client versions in the wild
//
// WHEN NOT TO USE
// ---------------
// - NO: Short-lived internal services with coordinated releases
// - NO: Simple prototypes where versioning adds overhead
//
// REAL-WORLD EXAMPLE
// ------------------
// User API:
// - V1 returns Name as a single field
// - V2 returns FirstName and LastName
// - Both versions run in parallel during migration
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace RevisionNotesDemo.WebAPI.Versioning;

/// <summary>
/// EXAMPLE 1: URL PATH VERSIONING - Most Common Approach
/// 
/// THE PROBLEM:
/// API needs to evolve. V2 has breaking changes.
/// Old clients still using V1.
/// 
/// THE SOLUTION:
/// Include version in URL path: /api/v1/users, /api/v2/users
/// 
/// WHY IT MATTERS:
/// - Most visible and explicit
/// - Easy to test (just change URL)
/// - Good for public APIs
/// - Recommended by Microsoft, Google, AWS
/// 
/// PROS:
/// ✅ Very clear which version you're calling
/// ✅ Easy to test different versions
/// ✅ Works in browser, Swagger
/// ✅ Cacheable (different URLs)
/// 
/// CONS:
/// ❌ URLs change between versions
/// ❌ Can't version individual endpoints differently
/// </summary>
public static class UrlPathVersioningExamples
{
    // Configure in Program.cs / Startup.cs
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // ✅ Read version from URL path
            options.ApiVersionReader = new UrlSegmentApiVersionReader();

            // ✅ Report supported versions in response headers
            options.ReportApiVersions = true;

            // ✅ Default version if client doesn't specify
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        // ✅ Add API Explorer for Swagger integration
        services.AddVersionedApiExplorer(options =>
        {
            // Format version as 'v'major[.minor]
            options.GroupNameFormat = "'v'VVV";

            // Substitute version in route
            options.SubstituteApiVersionInUrl = true;
        });
    }

    // ✅ GOOD: V1 controller
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]  // ✅ {version:apiVersion} placeholder
    [ApiVersion("1.0")]  // ✅ This controller handles v1.0
    public class UsersV1Controller : ControllerBase
    {
        // GET api/v1/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new[]
            {
                new { Id = 1, Name = "John Doe" },  // V1: Simple response
                new { Id = 2, Name = "Jane Smith" }
            };

            return Ok(users);
        }

        // GET api/v1/users/1
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new { Id = id, Name = "John Doe" });
        }
    }

    // ✅ GOOD: V2 controller with breaking changes
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]  // ✅ This controller handles v2.0
    public class UsersV2Controller : ControllerBase
    {
        // GET api/v2/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new[]
            {
                new  // V2: Richer response with more fields
                {
                    Id = 1,
                    FirstName = "John",  // ✅ Split Name into FirstName/LastName
                    LastName = "Doe",
                    Email = "john@example.com",  // ✅ New field
                    CreatedAt = DateTime.UtcNow  // ✅ New field
                },
                new
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            return Ok(users);
        }

        // GET api/v2/users/1
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}

/// <summary>
/// EXAMPLE 2: QUERY STRING VERSIONING - Version as Parameter
/// 
/// THE PROBLEM:
/// Want same URL across versions.
/// URL path versioning too visible.
/// 
/// THE SOLUTION:
/// Include version as query parameter: /api/users?api-version=2.0
/// 
/// WHY IT MATTERS:
/// - Same base URL across versions
/// - Optional versioning parameter
/// - Good for internal APIs
/// 
/// PROS:
/// ✅ Same URL structure
/// ✅ Optional (can have default)
/// ✅ Works with existing URLs
/// 
/// CONS:
/// ❌ Less visible than URL path
/// ❌ Query string might be ignored by proxies/caches
/// ❌ Harder to construct URLs
/// </summary>
public static class QueryStringVersioningExamples
{
    // Configure in Program.cs / Startup.cs
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // ✅ Read version from query string
            options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }

    // ✅ GOOD: Single controller  with multiple versions
    [ApiController]
    [Route("api/[controller]")]  // ✅ No version in route
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProductsController : ControllerBase
    {
        // GET api/products?api-version=1.0
        [HttpGet]
        [MapToApiVersion("1.0")]  // ✅ This method handles v1.0
        public IActionResult GetProductsV1()
        {
            return Ok(new[] { new { Id = 1, Name = "Product 1", Price = 9.99 } });
        }

        // GET api/products?api-version=2.0
        [HttpGet]
        [MapToApiVersion("2.0")]  // ✅ This method handles v2.0
        public IActionResult GetProductsV2()
        {
            return Ok(new[]
            {
                new
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 9.99m,
                    Currency = "USD",  // ✅ V2 adds currency
                    InStock = true      // ✅ V2 adds stock status
                }
            });
        }

        // GET api/products/1 (version-neutral)
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            // ✅ This endpoint works for all versions
            return Ok(new { Id = id, Name = "Product 1" });
        }
    }
}

/// <summary>
/// EXAMPLE 3: HEADER VERSIONING - Version in HTTP Header
/// 
/// THE PROBLEM:
/// Don't want version in URL at all.
/// More RESTful purists approach.
/// 
/// THE SOLUTION:
/// Send version in custom header: X-API-Version: 2.0
/// 
/// WHY IT MATTERS:
/// - Clean URLs (no version info)
/// - RESTful (resource URLs don't change)
/// - Good for internal microservices
/// 
/// PROS:
/// ✅ Cleanest URLs
/// ✅ RESTful
/// ✅ Can version per-request
/// 
/// CONS:
/// ❌ Less discoverable
/// ❌ Harder to test in browser
/// ❌ Not visible in most API explorers
/// </summary>
public static class HeaderVersioningExamples
{
    // Configure in Program.cs / Startup.cs
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // ✅ Read version from header
            options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");

            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }

    // ✅ GOOD: Clean URLs, version in header
    [ApiController]
    [Route("api/[controller]")]  // ✅ No version in URL
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class OrdersController : ControllerBase
    {
        // GET api/orders
        // Header: X-API-Version: 1.0
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult GetOrdersV1()
        {
            return Ok(new[] { new { Id = 1, Total = 100.00 } });
        }

        // GET api/orders
        // Header: X-API-Version: 2.0
        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetOrdersV2()
        {
            return Ok(new[]
            {
                new
                {
                    Id = 1,
                    Total = 100.00m,
                    Currency = "USD",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }
    }

    // Example HTTP request:
    // GET /api/orders HTTP/1.1
    // Host: api.example.com
    // X-API-Version: 2.0
    // Accept: application/json
}

/// <summary>
/// EXAMPLE 4: MEDIA TYPE (CONTENT NEGOTIATION) VERSIONING - Accept Header
/// 
/// THE PROBLEM:
/// Want true REST content negotiation.
/// Version is part of media type.
/// 
/// THE SOLUTION:
/// Include version in Accept header: Accept: application/vnd.myapi.v2+json
/// 
/// WHY IT MATTERS:
/// - Most RESTful approach
/// - Follows HTTP standards
/// - Used by GitHub, Stripe APIs
/// 
/// PROS:
/// ✅ True REST content negotiation
/// ✅ Can version representation, not resource
/// ✅ Standards-compliant
/// 
/// CONS:
/// ❌ Most complex to implement
/// ❌ Least intuitive for developers
/// ❌ Harder to test
/// </summary>
public static class MediaTypeVersioningExamples
{
    // Configure in Program.cs / Startup.cs
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // ✅ Read version from Accept header media type
            options.ApiVersionReader = new MediaTypeApiVersionReader();

            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }

    // Configuration for media types
    // Example media types:
    // - application/vnd.myapi.v1+json
    // - application/vnd.myapi.v2+json
    // - application/vnd.myapi.v1+xml

    // Example HTTP request:
    // GET /api/customers HTTP/1.1
    // Host: api.example.com
    // Accept: application/vnd.myapi.v2+json
}

/// <summary>
/// EXAMPLE 5: MULTIPLE VERSIONING STRATEGIES - Flexibility
/// 
/// THE PROBLEM:
/// Different clients prefer different versioning methods.
/// 
/// THE SOLUTION:
/// Support multiple versioning methods simultaneously.
/// 
/// WHY IT MATTERS:
/// - Maximum flexibility
/// - Support different client types
/// - Migration path
/// </summary>
public static class MultipleVersioningStrategiesExamples
{
    // Configure in Program.cs / Startup.cs
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // ✅ Support URL, query string, AND header versioning
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-API-Version")
            );

            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }

    // Now these all work:
    // - GET /api/v2/users (URL versioning)
    // - GET /api/users?api-version=2.0 (Query string)
    // - GET /api/users with header X-API-Version: 2.0 (Header)
}

/// <summary>
/// EXAMPLE 6: VERSION-SPECIFIC MODELS AND SHARED LOGIC
/// 
/// THE PROBLEM:
/// Each version has different DTOs, but much logic is shared.
/// 
/// THE SOLUTION:
/// Version-specific view models, shared domain logic.
/// </summary>
public static class VersionSpecificModelsExample
{
    // ✅ Shared domain model
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // ✅ V1 DTO (simpler)
    public class UserV1Dto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;  // Combined name
    }

    // ✅ V2 DTO (richer)
    public class UserV2Dto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // ✅ Shared service (domain logic)
    public class UserService
    {
        public List<User> GetUsers()
        {
            // Shared business logic
            return new List<User>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", CreatedAt = DateTime.UtcNow }
            };
        }
    }

    // ✅ V1 Controller
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UsersControllerV1 : ControllerBase
    {
        private readonly UserService _userService;

        public UsersControllerV1(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetUsers();

            // ✅ Map to V1 DTO
            var v1Dtos = users.Select(u => new UserV1Dto
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}"  // Combine names for V1
            });

            return Ok(v1Dtos);
        }
    }

    // ✅ V2 Controller
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class UsersControllerV2 : ControllerBase
    {
        private readonly UserService _userService;

        public UsersControllerV2(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetUsers();

            // ✅ Map to V2 DTO
            var v2Dtos = users.Select(u => new UserV2Dto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            });

            return Ok(v2Dtos);
        }
    }
}

/// <summary>
/// EXAMPLE 7: DEPRECATED VERSIONS - Sunsetting APIs
/// 
/// THE PROBLEM:
/// Need to retire old API versions gracefully.
/// 
/// THE SOLUTION:
/// Mark versions as deprecated, set sunset dates.
/// </summary>
public static class DeprecationExamples
{
    public static void ConfigureInStartup(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.ReportApiVersions = true;  // ✅ Reports deprecated versions in headers
            options.DefaultApiVersion = new ApiVersion(2, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });
    }

    // ✅ GOOD: Mark version as deprecated
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]  // ✅ V1 is deprecated
    [ApiVersion("2.0")]  // V2 is current
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult GetProductsV1()
        {
            // ✅ Add deprecation header
            Response.Headers["X-API-Deprecated"] = "true";
            Response.Headers["X-API-Sunset-Date"] = "2024-12-31";
            Response.Headers["Link"] = "<https://api.example.com/api/v2/products>; rel=\"successor-version\"";

            return Ok(new { Message = "This version is deprecated. Please migrate to V2." });
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetProductsV2()
        {
            return Ok(new[] { new { Id = 1, Name = "Product 1" } });
        }
    }

    // Response headers for deprecated version:
    // HTTP/1.1 200 OK
    // X-API-Deprecated: true
    // X-API-Sunset-Date: 2024-12-31
    // Link: <https://api.example.com/api/v2/products>; rel="successor-version"
    // api-supported-versions: 1.0, 2.0
    // api-deprecated-versions: 1.0
}

// SUMMARY - Versioning Strategy Decision Tree:
//
// Public API, many external clients?
//   → URL path versioning (/api/v1/users)
//
// Internal API, same organization?
//   → Query string or header versioning
//
// True REST, content negotiation?
//   → Media type versioning
//
// Migration scenario?
//   → Multiple strategies (URL + query string + header)
//
// BEST PRACTICES:
// ✅ Use semantic versioning (1.0, 2.0, not 2024-01-01)
// ✅ Only major versions in URL (not 1.1, 1.2 - use 1, 2)
// ✅ Report supported versions in response headers
// ✅ Document deprecation timeline (6-12 months)
// ✅ Share code between versions where possible
// ✅ Version DTOs, not domain models
// ❌ Don't version every endpoint differently
// ❌ Don't support too many versions (3 max recommended)
// ❌ Don't break compatibility within a version
