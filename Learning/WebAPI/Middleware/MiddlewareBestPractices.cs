// ==============================================================================
// MIDDLEWARE BEST PRACTICES - ASP.NET Core Request Pipeline
// ==============================================================================
//
// WHAT IS MIDDLEWARE?
// -------------------
// Middleware are pipeline components that handle requests and responses in order.
// They can short-circuit, modify responses, or delegate to the next component.
//
// WHY IT MATTERS
// --------------
// - Centralized cross-cutting concerns (logging, auth, error handling)
// - Predictable request flow when ordered correctly
// - Reusable and composable components
//
// WHEN TO USE
// -----------
// - YES: Logging, authentication, authorization, CORS, rate limiting
// - YES: Global error handling and response shaping
// - YES: Performance and security policies
//
// WHEN NOT TO USE
// ---------------
// - NO: Business logic belongs in services/controllers
// - NO: Avoid heavy work in middleware that slows every request
//
// REAL-WORLD EXAMPLE
// ------------------
// API pipeline:
// - Error handler -> HTTPS -> Routing -> CORS -> Auth -> Logging -> Endpoints
// - Correct order prevents security gaps and improves reliability
// ==============================================================================

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.RateLimiting;

namespace RevisionNotesDemo.WebAPI.Middleware;

/// <summary>
/// EXAMPLE 1: MIDDLEWARE PIPELINE ORDER - The Most Critical Concept
/// 
/// THE PROBLEM:
/// Middleware added in wrong order causes security issues, performance problems,
/// or features not working (e.g., auth before routing, CORS after endpoints).
/// 
/// THE SOLUTION:
/// Follow the recommended order:
/// 1. Exception handling (first - catches all exceptions)
/// 2. HTTPS redirection
/// 3. Static files (can short-circuit early)
/// 4. Routing (must come before auth/CORS/endpoints)
/// 5. CORS (after routing, before auth)
/// 6. Authentication (before authorization)
/// 7. Authorization (before endpoints)
/// 8. Custom middleware
/// 9. Endpoints (last - executes the endpoint)
/// 
/// WHY IT MATTERS:
/// - Wrong order = security vulnerabilities (auth after endpoints = no auth!)
/// - Performance issues (static files after routing = unnecessary processing)
/// - Features not working (CORS after endpoints = CORS errors)
/// 
/// GOTCHA: Authorization must come after Authentication, but both before Endpoints!
/// </summary>
public static class MiddlewarePipelineOrder
{
    public static void ConfigureCorrectPipelineOrder(WebApplication app)
    {
        // ❌ BAD: Random order, security issues
        // app.MapControllers();              // Endpoints FIRST = no auth/CORS!
        // app.UseAuthentication();           // Too late!
        // app.UseExceptionHandler("/error"); // Too late to catch exceptions!

        // ✅ GOOD: Correct order

        // 1. Exception handling FIRST - catches all exceptions
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            app.UseHsts(); // HTTP Strict Transport Security
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        // 2. HTTPS redirection (before static files)
        app.UseHttpsRedirection();

        // 3. Static files (can short-circuit for .js/.css/images)
        app.UseStaticFiles();

        // 4. Routing (MUST come before CORS, Auth, Authorization)
        app.UseRouting();

        // 5. CORS (after routing, before auth)
        app.UseCors("AllowSpecificOrigin");

        // 6. Authentication (validates who you are)
        app.UseAuthentication();

        // 7. Authorization (validates what you can do)
        app.UseAuthorization();

        // 8. Custom middleware (request/response logging, etc.)
        app.UseRequestLogging();
        app.UseRateLimiter();

        // 9. Session and others
        app.UseSession();

        // 10. Endpoints LAST
        app.MapControllers();
        app.MapRazorPages();
    }
}

/// <summary>
/// EXAMPLE 2: CUSTOM INLINE MIDDLEWARE - Use, Run, and Map
/// 
/// THE PROBLEM:
/// Need simple middleware logic but creating a full class is overkill.
/// Not understanding the difference between Use, Run, and Map.
/// 
/// THE SOLUTION:
/// - Use(): Add middleware that calls next (continues pipeline)
/// - Run(): Terminal middleware (ends pipeline, doesn't call next)
/// - Map(): Branch the pipeline based on request path
/// - MapWhen(): Branch based on predicate
/// 
/// WHY IT MATTERS:
/// - Inline middleware is perfect for simple logic
/// - Run() is useful for catch-all handlers
/// - Map() enables path-specific pipelines
/// - Clear understanding prevents pipeline bugs
/// 
/// PERFORMANCE: Inline middleware has zero overhead vs class-based
/// </summary>
public static class InlineMiddlewareExamples
{
    public static void ConfigureInlineMiddleware(WebApplication app)
    {
        // ✅ GOOD: Use() - adds to pipeline, calls next
        app.Use(async (context, next) =>
        {
            // Before: Execute before next middleware
            context.Response.Headers["X-Custom-Header"] = "Added by middleware";

            await next(); // Call next middleware

            // After: Execute after next middleware (on response)
            Console.WriteLine($"Response status: {context.Response.StatusCode}");
        });

        // ✅ GOOD: Run() - terminal middleware (doesn't call next)
        app.Run(async context =>
        {
            // This ends the pipeline - use for catch-all
            await context.Response.WriteAsync("End of pipeline");
            // No next() call - pipeline stops here
        });

        // ❌ BAD: Run() before endpoints = endpoints never execute!
        // app.Run(async context => await context.Response.WriteAsync("Oops"));
        // app.MapControllers(); // Never reached!

        // ✅ GOOD: Map() - branch pipeline by path
        app.Map("/health", healthApp =>
        {
            healthApp.Run(async context =>
            {
                await context.Response.WriteAsync("Healthy");
            });
        });

        // ✅ GOOD: MapWhen() - branch by predicate
        app.MapWhen(
            context => context.Request.Query.ContainsKey("debug"),
            debugApp =>
            {
                debugApp.Run(async context =>
                {
                    await context.Response.WriteAsync("Debug mode enabled");
                });
            });

        // ✅ GOOD: UseWhen() - conditional middleware (rejoins pipeline)
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api"),
            apiApp =>
            {
                apiApp.Use(async (context, next) =>
                {
                    context.Response.Headers["X-API-Version"] = "1.0";
                    await next();
                });
            });
    }
}

/// <summary>
/// EXAMPLE 3: CUSTOM MIDDLEWARE CLASS - Production-Ready Pattern
/// 
/// THE PROBLEM:
/// Complex middleware logic in inline lambdas is hard to read and test.
/// Need dependency injection, logging, or complex processing.
/// 
/// THE SOLUTION:
/// - Create middleware class with InvokeAsync method
/// - Accept RequestDelegate in constructor
/// - Inject services via InvokeAsync parameters
/// - Register with extension method
/// 
/// WHY IT MATTERS:
/// - Testable (can mock RequestDelegate and HttpContext)
/// - Supports dependency injection
/// - Clear separation of concerns
/// - Reusable across projects
/// 
/// BEST PRACTICE: Use class-based middleware for anything beyond trivial logic
/// </summary>
public class RequestTimingMiddleware
{
    private static readonly Action<ILogger, string, string, Exception?> RequestStarted =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(301, nameof(RequestStarted)),
            "Request started: {Method} {Path}");

    private static readonly Action<ILogger, string, string, int, long, Exception?> RequestCompleted =
        LoggerMessage.Define<string, string, int, long>(
            LogLevel.Information,
            new EventId(302, nameof(RequestCompleted)),
            "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms");

    private static readonly Action<ILogger, string, string, long, Exception?> RequestFailed =
        LoggerMessage.Define<string, string, long>(
            LogLevel.Error,
            new EventId(303, nameof(RequestFailed)),
            "Request failed: {Method} {Path} - Duration: {ElapsedMs}ms");

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    // Constructor DI - only for singletons!
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // InvokeAsync/Invoke - called for each request
    // Can inject scoped/transient services here
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Before: Execute before next middleware
            RequestStarted(_logger, context.Request.Method, context.Request.Path.ToString(), null);

            await _next(context); // Call next middleware

            // After: Execute on response
            stopwatch.Stop();
            RequestCompleted(
                _logger,
                context.Request.Method,
                context.Request.Path.ToString(),
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                null);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            RequestFailed(
                _logger,
                context.Request.Method,
                context.Request.Path.ToString(),
                stopwatch.ElapsedMilliseconds,
                ex);
            throw; // Re-throw to let exception handler deal with it
        }
    }
}

// ✅ GOOD: Extension method for clean registration
public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimingMiddleware>();
    }
}

/// <summary>
/// EXAMPLE 4: EXCEPTION HANDLING MIDDLEWARE - Global Error Handler
/// 
/// THE PROBLEM:
/// Unhandled exceptions return 500 with stack traces (security risk).
/// Inconsistent error responses across application.
/// No centralized logging of errors.
/// 
/// THE SOLUTION:
/// - Create exception handling middleware (FIRST in pipeline)
/// - Return Problem Details for all errors
/// - Log exceptions with correlation IDs
/// - Different handling for dev vs production
/// 
/// WHY IT MATTERS:
/// - Security (don't expose stack traces in production)
/// - Consistent error format for API consumers
/// - Centralized error logging
/// - Better monitoring and alerting
/// 
/// GOTCHA: Must be FIRST middleware in pipeline to catch all exceptions!
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private static readonly Action<ILogger, string, Exception?> UnhandledExceptionWithCorrelation =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(304, nameof(UnhandledExceptionWithCorrelation)),
            "Unhandled exception occurred. CorrelationId: {CorrelationId}");

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Generate correlation ID for tracking
        var correlationId = context.TraceIdentifier;

        UnhandledExceptionWithCorrelation(_logger, correlationId, exception);

        // Determine status code based on exception type
        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        // ✅ GOOD: Use Problem Details format (RFC 7807)
        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title = title,
            status = statusCode,
            detail = _environment.IsDevelopment()
                ? exception.Message // Dev: show message
                : "An error occurred processing your request", // Prod: generic
            instance = context.Request.Path.ToString(),
            traceId = correlationId,
            // Only include in development
            stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

/// <summary>
/// EXAMPLE 5: REQUEST/RESPONSE LOGGING MIDDLEWARE
/// 
/// THE PROBLEM:
/// Need to log request details (body, headers) for debugging,
/// but reading request body prevents controllers from reading it.
/// Response body is not seekable by default.
/// 
/// THE SOLUTION:
/// - Enable request body buffering
/// - Read and reset stream position
/// - Replace response body stream with MemoryStream
/// - Log both request and response
/// 
/// WHY IT MATTERS:
/// - Essential for debugging production issues
/// - Audit trail for compliance
/// - Performance monitoring
/// - API analytics
/// 
/// GOTCHA: Reading request body without EnableBuffering() breaks model binding!
/// PERFORMANCE: Only enable detailed logging in dev or for specific endpoints
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private static readonly Action<ILogger, string, string, string, string, string, Exception?> HttpRequestLogged =
        LoggerMessage.Define<string, string, string, string, string>(
            LogLevel.Information,
            new EventId(305, nameof(HttpRequestLogged)),
            "HTTP Request: {Method} {Path} {QueryString}\nHeaders: {Headers}\nBody: {Body}");

    private static readonly Action<ILogger, int, string, Exception?> HttpResponseLogged =
        LoggerMessage.Define<int, string>(
            LogLevel.Information,
            new EventId(306, nameof(HttpResponseLogged)),
            "HTTP Response: Status {StatusCode}\nBody: {Body}");

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ GOOD: Enable request body buffering (allows multiple reads)
        context.Request.EnableBuffering();

        // Log request
        await LogRequest(context.Request);

        // ✅ GOOD: Replace response body stream to capture output
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Log response
            await LogResponse(context.Response);

            // ✅ CRITICAL: Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpRequest request)
    {
        request.Body.Position = 0; // Reset position
        using var reader = new StreamReader(
            request.Body,
            encoding: System.Text.Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true); // CRITICAL: leave stream open

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset for next middleware

        HttpRequestLogged(
            _logger,
            request.Method,
            request.Path.ToString(),
            request.QueryString.ToString(),
            string.Join(", ", request.Headers.Select(h => $"{h.Key}:{h.Value}")),
            body,
            null);
    }

    private async Task LogResponse(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        response.Body.Position = 0;

        HttpResponseLogged(_logger, response.StatusCode, body, null);
    }
}

public static class RequestResponseLoggingExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

/// <summary>
/// EXAMPLE 6: CORS MIDDLEWARE - Cross-Origin Resource Sharing
/// 
/// THE PROBLEM:
/// Browser blocks API calls from different origins (security feature).
/// Need to allow specific origins for SPAs/mobile apps.
/// CORS configured after endpoints = doesn't work!
/// 
/// THE SOLUTION:
/// - Configure CORS policy in Program.cs
/// - Apply UseCors() after UseRouting(), before UseAuthorization()
/// - Use named policies for different requirements
/// - Be specific (avoid AllowAnyOrigin in production)
/// 
/// WHY IT MATTERS:
/// - Required for SPAs (React, Angular, Vue) on different domains
/// - Security (control which origins can access API)
/// - Credentials support (cookies, auth headers)
/// 
/// GOTCHA: CORS must come after UseRouting(), before UseAuthorization()!
/// SECURITY: Never use AllowAnyOrigin() with AllowCredentials()
/// </summary>
public static class CorsConfiguration
{
    public static void ConfigureCorsServices(WebApplicationBuilder builder)
    {
        // ❌ BAD: Allow any origin (security risk!)
        // builder.Services.AddCors(options =>
        // {
        //     options.AddDefaultPolicy(policy =>
        //         policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        // });

        // ✅ GOOD: Specific origins with named policies
        builder.Services.AddCors(options =>
        {
            // Policy 1: Development - permissive
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // Allow cookies/auth
            });

            // Policy 2: Production - restrictive
            options.AddPolicy("ProductionPolicy", policy =>
            {
                policy.WithOrigins("https://myapp.com", "https://www.myapp.com")
                      .WithMethods("GET", "POST", "PUT", "DELETE") // Specific methods
                      .WithHeaders("Content-Type", "Authorization") // Specific headers
                      .AllowCredentials()
                      .SetIsOriginAllowedToAllowWildcardSubdomains() // *.myapp.com
                      .SetPreflightMaxAge(TimeSpan.FromHours(1)); // Cache preflight
            });

            // Policy 3: Public API - no credentials
            options.AddPolicy("PublicApiPolicy", policy =>
            {
                policy.WithOrigins("https://publicapp.com")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .DisallowCredentials(); // No cookies
            });
        });
    }

    public static void ConfigureCorsMiddleware(WebApplication app)
    {
        // ✅ CORRECT ORDER: After UseRouting, before UseAuthorization
        app.UseRouting();

        // Apply policy based on environment
        var corsPolicy = app.Environment.IsDevelopment()
            ? "DevelopmentPolicy"
            : "ProductionPolicy";

        app.UseCors(corsPolicy);

        app.UseAuthentication();
        app.UseAuthorization();

        // ❌ WRONG: CORS after endpoints = doesn't work!
        // app.MapControllers();
        // app.UseCors(corsPolicy); // Too late!
    }
}

/// <summary>
/// EXAMPLE 7: RATE LIMITING MIDDLEWARE (.NET 7+)
/// 
/// THE PROBLEM:
/// APIs vulnerable to abuse (DDoS, brute force, resource exhaustion).
/// Need to limit requests per IP, user, or endpoint.
/// 
/// THE SOLUTION:
/// - Use built-in RateLimiter middleware (.NET 7+)
/// - Configure policies (Fixed Window, Sliding Window, Token Bucket, Concurrency)
/// - Apply globally or per-endpoint
/// - Return 429 Too Many Requests
/// 
/// WHY IT MATTERS:
/// - Prevent abuse and DDoS attacks
/// - Fair resource allocation
/// - Protect against brute force
/// - Cost control (API usage limits)
/// 
/// BEST PRACTICE: Combine multiple strategies (IP + user + endpoint)
/// </summary>
public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            // ✅ GOOD: Fixed window limiter (simple, predictable)
            options.AddFixedWindowLimiter("fixed", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100; // 100 requests
                limiterOptions.Window = TimeSpan.FromMinutes(1); // Per minute
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10; // Queue up to 10
            });

            // ✅ GOOD: Sliding window (smoother distribution)
            options.AddSlidingWindowLimiter("sliding", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.SegmentsPerWindow = 4; // 4 segments of 15 seconds
                limiterOptions.QueueLimit = 10;
            });

            // ✅ GOOD: Token bucket (burst support)
            options.AddTokenBucketLimiter("token", limiterOptions =>
            {
                limiterOptions.TokenLimit = 100;
                limiterOptions.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                limiterOptions.TokensPerPeriod = 100;
                limiterOptions.AutoReplenishment = true;
                limiterOptions.QueueLimit = 10;
            });

            // ✅ GOOD: Concurrency limiter (max concurrent requests)
            options.AddConcurrencyLimiter("concurrency", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10; // Max 10 concurrent
                limiterOptions.QueueLimit = 20;
            });

            // ✅ GOOD: Per-IP rate limiting
            options.AddPolicy("per-ip", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    ipAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });

            // Global rejection response
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var retryAfter = context.Lease.TryGetMetadata(
                    MetadataName.RetryAfter,
                    out var retryAfterValue)
                    ? retryAfterValue.TotalSeconds
                    : 60;

                context.HttpContext.Response.Headers["Retry-After"] =
                    retryAfter.ToString(CultureInfo.InvariantCulture);

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = $"{retryAfter} seconds"
                }, token);
            };
        });
    }

    public static void UseRateLimitingMiddleware(WebApplication app)
    {
        // ✅ GOOD: After authentication/authorization
        app.UseRateLimiter();

        // Apply to endpoints
        app.MapGet("/api/data", () => "Success")
            .RequireRateLimiting("fixed");

        app.MapGet("/api/expensive", () => "Expensive operation")
            .RequireRateLimiting("concurrency");
    }
}

/// <summary>
/// EXAMPLE 8: SHORT-CIRCUITING AND CONDITIONAL MIDDLEWARE
/// 
/// THE PROBLEM:
/// Some requests don't need full pipeline (health checks, static files).
/// Want different middleware for different paths.
/// Need to optimize performance by skipping unnecessary processing.
/// 
/// THE SOLUTION:
/// - Use Map() to branch pipeline
/// - Use MapWhen() for conditional branching
/// - Use short-circuit middleware (static files, health checks)
/// - Use UseWhen() to conditionally apply and rejoin pipeline
/// 
/// WHY IT MATTERS:
/// - Performance (skip auth for static files)
/// - Security (different rules for /admin vs /public)
/// - Flexibility (per-path middleware configuration)
/// 
/// TIP: Static files and health checks naturally short-circuit
/// </summary>
public static class ShortCircuitingExamples
{
    public static void ConfigureShortCircuiting(WebApplication app)
    {
        // ✅ GOOD: Static files short-circuit early
        app.UseStaticFiles(); // If file found, pipeline ends here

        // ✅ GOOD: Health check endpoint (short-circuits)
        app.MapHealthChecks("/health"); // Returns immediately

        // ✅ GOOD: Map branches pipeline (separate pipeline)
        app.Map("/admin", adminApp =>
        {
            // Admin-only middleware
            adminApp.UseAuthentication();
            adminApp.UseAuthorization();
            adminApp.Use(async (context, next) =>
            {
                if (!context.User.IsInRole("Admin"))
                {
                    context.Response.StatusCode = 403;
                    return; // Short-circuit
                }
                await next();
            });
            // Note: Use MapGroup for endpoint registration in branches
            // adminApp doesn't have MapControllers() - use main app instead
            adminApp.Run(async context =>
            {
                await context.Response.WriteAsync("Admin area");
            });
        });

        // ✅ GOOD: UseWhen applies conditionally but rejoins pipeline
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api"),
            apiApp =>
            {
                apiApp.UseRateLimiter();
                apiApp.Use(async (context, next) =>
                {
                    context.Response.Headers["X-API-Version"] = "2.0";
                    await next();
                });
            }
        ); // Rejoins main pipeline

        // ✅ GOOD: Early return for specific conditions
        app.Use(async (context, next) =>
        {
            // Maintenance mode
            if (IsMaintenanceMode())
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Service Unavailable",
                    message = "System is under maintenance"
                });
                return; // Short-circuit
            }

            await next();
        });
    }

    private static bool IsMaintenanceMode() => false;
}

// Extension methods for custom middleware registration
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimingMiddleware>();
    }
}
