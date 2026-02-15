// ==============================================================================
// API VERSIONING BEST PRACTICES - Production Patterns
// ==============================================================================
//
// WHAT IS IT?
// -----------
// Practical guidance for evolving APIs without breaking clients, including
// migration strategy, deprecation, and documentation.
//
// WHY IT MATTERS
// --------------
// - Backward compatibility protects existing clients
// - Clear deprecation reduces support burden
// - Good versioning enables long-lived APIs
//
// WHEN TO USE
// -----------
// - YES: Public or partner-facing APIs
// - YES: Any API with long-lived client integrations
//
// WHEN NOT TO USE
// ---------------
// - NO: Internal APIs with tight release coordination
// - NO: Prototypes that will be replaced quickly
//
// REAL-WORLD EXAMPLE
// ------------------
// Payments API:
// - V1 supports basic charges
// - V2 adds split payments and new response fields
// - V1 is deprecated with sunset headers and migration guide
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
// Note: Swagger integration types moved in .NET 10 - example patterns shown but commented
// using Microsoft.OpenApi.Models;  // Types relocated in Swashbuckle 10.x
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace RevisionNotesDemo.WebAPI.Versioning;

/// <summary>
/// EXAMPLE 1: SWAGGER INTEGRATION - Multiple API Documents
/// 
/// THE PROBLEM:
/// Swagger shows all versions in one document - confusing.
/// Need separate Swagger document per version.
/// 
/// THE SOLUTION:
/// Configure Swagger to generate one document per API version.
/// 
/// WHY IT MATTERS:
/// - Clear documentation per version
/// - Easy API exploration
/// - Try-it-out works correctly
/// - Client SDK generation
/// </summary>
public static class SwaggerIntegrationExamples
{
    // ✅ GOOD: Configure Swagger for multiple versions
    public static void ConfigureInStartup(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        // 1. Add API versioning
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(2, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        // 2. Add API Explorer for version discovery
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // 3. Configure Swagger
        services.AddSwaggerGen();
        // services.ConfigureOptions<ConfigureSwaggerOptions>();  // Example pattern - types moved in .NET 10
    }

    /*
    // ✅ GOOD: Swagger configuration for versioning
    // NOTE: OpenApi types moved in Swashbuckle 10.x/.NET 10 - pattern shown for reference
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        
        public void Configure(SwaggerGenOptions options)
        {
            // ✅ Create Swagger document for each discovered API version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo  // Type relocated in Swashbuckle 10.x
                    {
                        Title = $"My API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated
                            ? "⚠️ This API version is deprecated. Please migrate to a newer version."
                            : "Current API version.",
                        Contact = new OpenApiContact
                        {
                            Name = "API Support",
                            Email = "api@example.com"
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
            }
        }
    }
    */

    // ✅ Configure Swagger UI for multiple versions
    public static void ConfigureSwaggerUI(WebApplication app)
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // ✅ Add endpoint for each API version
            foreach (var description in provider.ApiVersionDescriptions.Reverse())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }

            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
        });

        // Result: Swagger dropdown with options:
        // - V2 (current)
        // - V1 (deprecated)
    }
}

/// <summary>
/// EXAMPLE 2: MIGRATION STRATEGIES - Evolving Existing APIs
/// 
/// THE PROBLEM:
/// Have existing API without versioning. Need to add V2.
/// Can't break existing clients.
/// 
/// THE SOLUTION:
/// Treat existing API as V1, add V2 alongside.
/// 
/// WHY IT MATTERS:
/// - Zero downtime migration
/// - Existing clients continue working
/// - Gradual adoption
/// </summary>
public static class MigrationStrategyExamples
{
    // SCENARIO: Existing API without versioning
    [ApiController]
    [Route("api/[controller]")]  // Currently: /api/customers
    public class ExistingCustomersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[] { new { Id = 1, Name = "Customer 1" } });
        }
    }

    // STEP 1: Keep existing controller, mark as V1
    [ApiController]
    [Route("api/[controller]")]  // ✅ Original route still works
    [Route("api/v{version:apiVersion}/[controller]")]  // ✅ Add versioned route
    [ApiVersion("1.0")]  // ✅ Declare this is V1
    public class CustomersV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // ✅ Same response as before - no breaking change
            return Ok(new[] { new { Id = 1, Name = "Customer 1" } });
        }
    }

    // STEP 2: Add V2 controller with new features
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class CustomersV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // ✅ Enhanced response in V2
            return Ok(new[]
            {
                new
                {
                    Id = 1,
                    Name = "Customer 1",
                    Email = "customer1@example.com",  // New field
                    Phone = "555-0100",                // New field
                    CreatedAt = DateTime.UtcNow        // New field
                }
            });
        }
    }

    // Now these all work:
    // - GET /api/customers → V1 (existing clients)
    // - GET /api/v1/customers → V1 (explicit)
    // - GET /api/v2/customers → V2 (new clients)

    // ✅ GOOD: Gradual deprecation timeline
    // Month 1-3:   V2 released, V1 maintained, documented
    // Month 4-6:   V1 marked deprecated, sunset date announced
    // Month 7-9:   V1 deprecated warnings in response
    // Month 10-12: V1 clients contacted, migration support
    // Month 13:    V1 removed (return HTTP 410 Gone)
}

/// <summary>
/// EXAMPLE 3: TESTING VERSIONED APIs
/// 
/// THE PROBLEM:
/// Need to test all versions independently.
/// 
/// THE SOLUTION:
/// Versioned test cases with clear version specification.
/// </summary>
public static class TestingVersionedAPIsExamples
{
    // ✅ GOOD: Test helper for versioned requests
    public class VersionedApiTestBase
    {
        protected HttpClient CreateClient(string version)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.example.com")
            };

            // Add version to every request
            client.DefaultRequestHeaders.Add("X-API-Version", version);

            return client;
        }

        protected HttpClient CreateClientV1() => CreateClient("1.0");
        protected HttpClient CreateClientV2() => CreateClient("2.0");
    }

    // ✅ GOOD: Test V1 behavior
    public class UsersApiV1Tests : VersionedApiTestBase
    {
        [Fact]
        public async Task GetUsers_V1_ReturnsSimpleResponse()
        {
            var client = CreateClientV1();

            var response = await client.GetAsync("/api/v1/users");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<UserV1[]>();

            Xunit.Assert.NotNull(users);
            Xunit.Assert.All(users, user =>
            {
                Xunit.Assert.NotEqual(0, user.Id);
                Xunit.Assert.NotEmpty(user.Name);  // V1 has combined name
            });
        }
    }

    // ✅ GOOD: Test V2 behavior
    public class UsersApiV2Tests : VersionedApiTestBase
    {
        [Fact]
        public async Task GetUsers_V2_ReturnsEnhancedResponse()
        {
            var client = CreateClientV2();

            var response = await client.GetAsync("/api/v2/users");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<UserV2[]>();

            Xunit.Assert.NotNull(users);
            Xunit.Assert.All(users, user =>
            {
                Xunit.Assert.NotEqual(0, user.Id);
                Xunit.Assert.NotEmpty(user.FirstName);  // V2 has split names
                Xunit.Assert.NotEmpty(user.LastName);
                Xunit.Assert.NotEmpty(user.Email);      // V2 has email
            });
        }
    }

    // ✅ GOOD: Test version negotiation
    public class VersionNegotiationTests
    {
        [Fact]
        public async Task NoVersionSpecified_ReturnsDefaultVersion()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("/api/users");

            // Should use default version (configured in startup)
            Xunit.Assert.True(response.Headers.Contains("api-supported-versions"));
        }

        [Fact]
        public async Task InvalidVersion_Returns400()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Version", "99.0");

            var response = await client.GetAsync("/api/users");

            Xunit.Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    // Supporting DTOs
    public class UserV1
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserV2
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

/// <summary>
/// EXAMPLE 4: DEPRECATION POLICY - Retiring Old Versions
/// 
/// THE PROBLEM:
/// Need formal process for deprecating versions.
/// 
/// THE SOLUTION:
/// Documented deprecation policy with clear timelines.
/// </summary>
public static class DeprecationPolicyExamples
{
    // ✅ GOOD: Deprecation middleware
    public class DeprecationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DeprecationConfig _config;

        public DeprecationMiddleware(RequestDelegate next, DeprecationConfig config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            await _next(context);

            // ✅ Add deprecation headers to response
            if (context.GetEndpoint()?.Metadata
                .GetMetadata<ApiVersionAttribute>()?.Versions
                .Any(v => _config.DeprecatedVersions.Contains(v.ToString() ?? string.Empty)) == true)
            {
                var sunsetDate = _config.GetSunsetDate(context.GetRequestedApiVersion()?.ToString() ?? string.Empty);

                context.Response.Headers["Deprecation"] = "true";  // RFC 8594
                context.Response.Headers["Sunset"] = sunsetDate.ToString("R");  // RFC 8594
                context.Response.Headers["Link"] = $"<{_config.MigrationGuideUrl}>; rel=\"deprecation\"";
            }
        }
    }

    public class DeprecationConfig
    {
        public List<string> DeprecatedVersions { get; set; } = new();
        public Dictionary<string, DateTime> SunsetDates { get; set; } = new();
        public string MigrationGuideUrl { get; set; } = string.Empty;

        public DateTime GetSunsetDate(string version)
        {
            return SunsetDates.TryGetValue(version, out var date)
                ? date
                : DateTime.UtcNow.AddYears(1);  // Default: 1 year
        }
    }

    // Configure in  appsettings.json:
    // {
    //   "Deprecation": {
    //     "DeprecatedVersions": [ "1.0" ],
    //     "SunsetDates": {
    //       "1.0": "2024-12-31"
    //     },
    //     "MigrationGuideUrl": "https://api.example.com/docs/migration-v1-to-v2"
    //   }
    // }

    // ✅ GOOD: Deprecation warning in response
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult GetV1()
        {
            // ✅ Include deprecation info in response body too
            Response.Headers["Warning"] = "299 - \"This API version is deprecated. Please migrate to V2 by 2024-12-31.\"";

            return Ok(new
            {
                Data = new[] { new { Id = 1, Name = "Product 1" } },
                _metadata = new
                {
                    deprecated = true,
                    sunset_date = "2024-12-31",
                    migration_guide = "https://api.example.com/docs/migration-v1-to-v2",
                    successor_version = "2.0"
                }
            });
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Ok(new[] { new { Id = 1, Name = "Product 1", Stock = 100 } });
        }
    }
}

/// <summary>
/// EXAMPLE 5: VERSION-SPECIFIC VALIDATION AND BUSINESS RULES
/// 
/// THE PROBLEM:
/// Different versions have different validation rules.
/// 
/// THE SOLUTION:
/// Version-aware validation attributes or explicit checks.
/// </summary>
public static class VersionSpecificValidationExamples
{
    // ✅ V1 model - lenient validation
    public class CreateUserV1Request
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // V1: Password is optional
        public string? Password { get; set; }
    }

    // ✅ V2 model - stricter validation
    public class CreateUserV2Request
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // V2: Password is required with complexity rules
        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, and digit")]
        public string Password { get; set; } = string.Empty;

        // V2: Phone number added
        [Phone]
        public string? PhoneNumber { get; set; }
    }

    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        [MapToApiVersion("1.0")]
        public IActionResult CreateV1([FromBody] CreateUserV1Request request)
        {
            // V1: Generate password if not provided
            var password = request.Password ?? GenerateRandomPassword();

            return Ok(new { Id = 1, Username = request.Username });
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public IActionResult CreateV2([FromBody] CreateUserV2Request request)
        {
            // V2: Password required, already validated
            return Ok(new
            {
                Id = 1,
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            });
        }

        private static string GenerateRandomPassword() => "TempPassword123!";
    }
}

/// <summary>
/// EXAMPLE 6: COMMON ANTI-PATTERNS TO AVOID
/// </summary>
public static class AntiPatterns
{
    // ❌ ANTI-PATTERN 1: Breaking changes in minor versions
    public class BadVersioning
    {
        // V1.0: Returns { Id, Name }
        // V1.1: Returns { Id, FullName }  ❌ Breaking change in minor version!

        // ✅ DO: Breaking changes = new major version (V2.0)
    }

    // ❌ ANTI-PATTERN 2: Date-based versioning
    public class DateBasedVersioning
    {
        // ❌ BAD: /api/2024-01-15/users
        // Hard to understand, arbitrary dates

        // ✅ GOOD: /api/v2/users
        // Clear semantic versioning
    }

    // ❌ ANTI-PATTERN 3: Too many versions
    public class TooManyVersions
    {
        // ❌ Supporting V1, V2, V3, V4, V5 simultaneously
        // Maintenance nightmare

        // ✅ Support max 2-3 versions
        // Deprecate old versions regularly
    }

    // ❌ ANTI-PATTERN 4: No default version
    public class NoDefaultVersion
    {
        // ❌ Returns 400 if version not specified
        // Breaks existing clients when versioning added

        // ✅ AssumeDefaultVersionWhenUnspecified = true
        // Existing clients continue working
    }

    // ❌ ANTI-PATTERN 5: Different versioning per endpoint
    public class InconsistentVersioning
    {
        // ❌ /api/v1/users, /api/v2.1/products, /api/v3.0/orders
        // Confusing and hard to maintain

        // ✅ Consistent versioning across entire API
    }
}

// SUMMARY - Versioning Best Practices Checklist:
//
// ✅ START:
// - Choose one primary versioning strategy (recommend URL path)
// - Document versioning approach clearly
// - Set up Swagger for all versions
// - Define deprecation policy upfront
//
// ✅ WHEN ADDING V2:
// - Only for breaking changes
// - Keep V1 working
// - Provide migration guide
// - Set sunset date for V1
//
// ✅ DEPRECATION PROCESS:
// 1. Announce deprecation (6-12 months advance)
// 2. Add deprecation headers
// 3. Contact known V1 clients
// 4. Monitor V1 usage metrics
// 5. When usage < 1%, remove V1
// 6. Return HTTP 410 Gone for V1 requests
//
// ✅ TESTING:
// - Test all supported versions
// - Test version negotiation
// - Test deprecation headers
// - Load test multiple versions
//
// ✅ DOCUMENTATION:
// - Swagger per version
// - Migration guides
// - Breaking changes log
// - Support timeline
//
// ❌ NEVER:
// - Break existing version contract
// - Remove version without sunset notice
// - Change versioning strategy mid-stream
// - Support more than 3 versions simultaneously

// Fact attribute for testing examples
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class FactAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RequiredAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EmailAddressAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class StringLengthAttribute : Attribute
{
    public StringLengthAttribute(int max) { }
    public int MinimumLength { get; set; }
}
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RegularExpressionAttribute : Attribute
{
    public RegularExpressionAttribute(string pattern) { }
    public string ErrorMessage { get; set; } = string.Empty;
}
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PhoneAttribute : Attribute { }
