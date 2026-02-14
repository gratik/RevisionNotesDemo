// ==============================================================================
// STRUCTURED LOGGING WITH SERILOG - Production-Grade Logging
// ==============================================================================
// PURPOSE:
//   Master Serilog for production structured logging with rich sinks and enrichers.
//   Learn configuration, sinks (Console, File, Seq, Application Insights), and best practices.
//
// WHY SERILOG:
//   - Best-in-class structured logging
//   - Extensive sink ecosystem (50+ sinks)
//   - Powerful enrichers and filters
//   - Excellent performance
//   - JSON output for log aggregation
//   - Integrates with ILogger seamlessly
//
// WHAT YOU'LL LEARN:
//   1. Serilog setup and configuration
//   2. Sinks (Console, File, Seq, Application Insights)
//   3. Structured property binding
//   4. Enrichers (machine name, thread, user)
//   5. Filtering and sampling
//   6. Diagnostic contexts
//
// PACKAGES:
//   - Serilog.AspNetCore (includes base + ASP.NET integration)
//   - Serilog.Sinks.Console
//   - Serilog.Sinks.File
//   - Serilog.Sinks.Seq (optional)
//   - Serilog.Enrichers.* (Thread, Environment, Process)
// ==============================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
using Microsoft.Extensions.Logging;

namespace RevisionNotesDemo.Logging;

/// <summary>
/// EXAMPLE 1: SERILOG CONFIGURATION - Program.cs Setup
/// 
/// THE PROBLEM:
/// Need structured, production-ready logging from application startup.
/// Default ILogger configuration is limited.
/// 
/// THE SOLUTION:
/// Configure Serilog as the logging provider in Program.cs.
/// 
/// WHY IT MATTERS:
/// - Captures startup errors
/// - Rich structured output
/// - Multiple sinks (console, file, centralized logging)
/// - Better than default logging
/// 
/// SETUP: Install Serilog.AspNetCore package
/// </summary>
public static class SerilogConfiguration
{
    // ✅ GOOD: Basic Serilog setup in Program.cs
    public static void ConfigureBasicSerilog()
    {
        // Example for Program.cs:
        
        // var builder = WebApplication.CreateBuilder(args);
        //
        // // ✅ Configure Serilog
        // builder.Host.UseSerilog((context, configuration) =>
        // {
        //     configuration
        //         .ReadFrom.Configuration(context.Configuration) // Read from appsettings.json
        //         .En richFromLogContext()       // Add enrichers
        //         .WriteTo.Console()              // Console sink
        //         .WriteTo.File(
        //             path: "logs/app-.log",
        //             rollingInterval: RollingInterval.Day,
        //             retainedFileCountLimit: 7); // Keep 7 days
        // });
        //
        // var app = builder.Build();
        //
        // // ✅ Add Serilog request logging
        // app.UseSerilogRequestLogging();
        //
        // app.Run();
    }
    
    // ✅ GOOD: Production-ready configuration
    public static void ConfigureProductionSerilog()
    {
        // Program.cs for production:
        
        // var builder = WebApplication.CreateBuilder(args);
        //
        // builder.Host.UseSerilog((context, services, configuration) =>
        // {
        //     configuration
        //         .ReadFrom.Configuration(context.Configuration)
        //         .ReadFrom.Services(services)  // Access DI services
        //         .Enrich.FromLogContext()
        //         .Enrich.WithMachineName()
        //         .Enrich.WithThreadId()
        //         .Enrich.WithEnvironmentName()
        //         .WriteTo.Console(outputTemplate:
        //             "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        //         .WriteTo.File(
        //             formatter: new JsonFormatter(),  // JSON for log aggregation
        //             path: "logs/app-.json",
        //             rollingInterval: RollingInterval.Day,
        //             retainedFileCountLimit: 30,
        //             fileSizeLimitBytes: 100 * 1024 * 1024,  // 100 MB
        //             rollOnFileSizeLimit: true)
        //         .WriteTo.Seq(context.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341")  // Centralized logging
        //         .Filter.ByExcluding(logEvent =>
        //             logEvent.Level == LogEventLevel.Information &&
        //             logEvent.MessageTemplate.Text.Contains("health"))  // Filter health check noise
        //         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        //         .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        //         .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        // });
    }
    
    // ✅ GOOD: appsettings.json configuration
    // {
    //   "Serilog": {
    //     "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    //     "MinimumLevel": {
    //       "Default": "Information",
    //       "Override": {
    //         "Microsoft": "Warning",
    //         "System": "Warning"
    //       }
    //     },
    //     "WriteTo": [
    //       { "Name": "Console" },
    //       {
    //         "Name": "File",
    //         "Args": {
    //           "path": "logs/app-.log",
    //           "rollingInterval": "Day",
    //           "retainedFileCountLimit": 7
    //         }
    //       }
    //     ],
    //     "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
    //   }
    // }
}

/// <summary>
/// EXAMPLE 2: STRUCTURED PROPERTY BINDING - The Serilog Way
/// 
/// THE PROBLEM:
/// String interpolation loses structure - can't query by specific fields.
/// 
/// THE SOLUTION:
/// Use Serilog's message templates to bind properties.
/// 
/// WHY IT MATTERS:
/// - Properties stored separately from message
/// - Can query: WHERE UserId = 123
/// - JSON output includes structured data
/// - Searchable and filterable in log aggregators
/// 
/// SERILOG OUTPUT:
/// {
///   "Timestamp": "2024-01-15T10:30:00Z",
///   "Level": "Information",
///   "MessageTemplate": "User {UserId} placed order {OrderId} for {Amount}",
///   "Properties": {
///     "UserId": 123,
///     "OrderId": "ORD-456",
///     "Amount": 99.99
///   }
/// }
/// </summary>
public static class StructuredPropertyExamples
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }
        
        // ❌ BAD: String interpolation - not structured
        public void BadLogging(int userId, string orderId, decimal amount)
        {
            _logger.LogInformation($"User {userId} placed order {orderId} for ${amount}");
            // Output: "User 123 placed order ORD-456 for $99.99"
            // Can only search text, not structured fields
        }
        
        // ✅ GOOD: Message template with named properties
        public void GoodLogging(int userId, string orderId, decimal amount)
        {
            _logger.LogInformation("User {UserId} placed order {OrderId} for {Amount:C}",
                userId, orderId, amount);
            // Serilog output: { "UserId": 123, "OrderId": "ORD-456", "Amount": 99.99 }
            // Can query: WHERE UserId = 123 AND Amount > 50
        }
        
        // ✅ GOOD: Destructuring objects with @
        public void DestructuringExample(Order order)
        {
            // @ prefix tells Serilog to serialize all properties
            _logger.LogInformation("Processing {@Order}", order);
            // Output: { "Order": { "Id": "ORD-456", "UserId": 123, "Items": [...], "Total": 99.99 } }
            
            // Without @: just ToString()
            _logger.LogInformation("Processing {Order}", order);
            // Output: { "Order": "RevisionNotesDemo.Logging.Order" }
        }
        
        // ✅ GOOD: $ prefix for stringification (when you don't want properties)
        public void StringificationExample()
        {
            var item = new { Name = "Widget", Price = 9.99 };
            
            _logger.LogInformation("Item: {$Item}", item);
            // Output: { "Item": "{ Name = Widget, Price = 9.99 }" }
            // ToString() called, not destructured
        }
        
        // ✅ GOOD: Projection with anonymous objects
        public void ProjectionExample(User user)
        {
            // Only log relevant properties
            _logger.LogInformation("User logged in: {@User}", new { user.Id, user.Username });
            // Output: { "User": { "Id": 123, "Username": "johndoe" } }
            // Password and other sensitive fields not logged
        }
    }
}

/// <summary>
/// EXAMPLE 3: SERILOG ENRICHERS - Adding Context to All Logs
/// 
/// THE PROBLEM:
/// Same context needed in every log entry (machine, environment, correlation ID).
/// 
/// THE SOLUTION:
/// Use Serilog enrichers to add properties automatically.
/// 
/// WHY IT MATTERS:
/// - DRY - set once, applied to all logs
/// - Correlation tracking
/// - Environment/machine identification
/// - User context
/// 
/// ENRICHERS:
/// - WithMachineName - Add computer name
/// - WithThreadId - Add thread ID
/// - WithEnvironmentName - Add environment (Development/Production)
/// - WithProperty - Add custom property
/// - FromLogContext - Add scoped properties
/// </summary>
public static class EnricherExamples
{
    public static void ConfigureEnrichers()
    {
        // Configuration in Program.cs:
        // Log.Logger = new LoggerConfiguration()
        //     .Enrich.FromLogContext()          // ✅ Enable LogContext
        //     .Enrich.WithMachineName()         // ✅ Add machine name
        //     .Enrich.WithThreadId()            // ✅ Add thread ID
        //     .Enrich.WithEnvironmentName()     // ✅ Add environment
        //     .Enrich.WithProperty("Application", "MyApp")  // ✅ Custom property
        //     .WriteTo.Console()
        //     .CreateLogger();
        
        // All logs now include:
        // {
        //   "MachineName": "WEB-SERVER-01",
        //   "ThreadId": 15,
        //   "EnvironmentName": "Production",
        //   "Application": "MyApp",
        //   ...
        // }
    }
    
    // ✅ GOOD: LogContext for request-scoped properties
    public class RequestMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        
        public RequestMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            
            // ✅ Push properties to LogContext - applies to all logs in this request
            using (LogContext.PushProperty("RequestId", requestId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("UserIdentity", context.User?.Identity?.Name ?? "Anonymous"))
            {
                await _next(context);
            }
            // Properties automatically removed after request
        }
    }
    
    // ✅ GOOD: Custom enricher for user context
    public class UserInfoEnricher : Serilog.Core.ILogEventEnricher
    {
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _contextAccessor;
        
        public UserInfoEnricher(Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        public void Enrich(LogEvent logEvent, Serilog.Core.ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", httpContext.User.FindFirst("sub")?.Value ?? "unknown"));
            }
        }
    }
}

/// <summary>
/// EXAMPLE 4: SERILOG SINKS - Where Logs Go
/// 
/// THE PROBLEM:
/// Need logs in multiple places: console (dev), files (production), centralized (monitoring).
/// 
/// THE SOLUTION:
/// Configure multiple sinks - Serilog writes to all simultaneously.
/// 
/// WHY IT MATTERS:
/// - Console: real-time during development
/// - File: local backup, troubleshooting
/// - Seq/ELK/Splunk: centralized search and alerting
/// - Application Insights: Azure monitoring
/// 
/// POPULAR SINKS:
/// - Console, File, Debug
/// - Seq (free local log server)
/// - Elasticsearch (ELK stack)
/// - Application Insights (Azure)
/// - Splunk, Datadog, New Relic
/// </summary>
public static class SinkExamples
{
    // ✅ GOOD: Multiple sinks with different configurations
    public static void ConfigureMultipleSinks()
    {
        // Log.Logger = new LoggerConfiguration()
        //     // ✅ Console sink - colored, human-readable
        //     .WriteTo.Console(
        //         outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        //     
        //     // ✅ File sink - JSON for machines
        //     .WriteTo.File(
        //         formatter: new JsonFormatter(),
        //         path: "logs/app-.json",
        //         rollingInterval: RollingInterval.Day,
        //         retainedFileCountLimit: 30)
        //     
        //     // ✅ File sink - text for humans
        //     .WriteTo.File(
        //         path: "logs/app-.log",
        //         rollingInterval: RollingInterval.Day,
        //         retainedFileCountLimit: 7)
        //     
        //     // ✅ Seq sink - centralized logging (install Seq locally or use seq.io)
        //     .WriteTo.Seq("http://localhost:5341",
        //         apiKey: "your-api-key")  // Optional API key
        //     
        //     .CreateLogger();
    }
    
    // ✅ GOOD: Conditional sinks (different per environment)
    public static void ConditionalSinks(IHostEnvironment env)
    {
        // var config = new LoggerConfiguration()
        //     .Enrich.FromLogContext();
        //
        // if (env.IsDevelopment())
        // {
        //     // ✅ Development: console only, verbose
        //     config.WriteTo.Console()
        //          .MinimumLevel.Debug();
        // }
        // else if (env.IsProduction())
        // {
        //     // ✅ Production: file + Application Insights
        //     config.WriteTo.File("logs/app-.json", rollingInterval: RollingInterval.Day)
        //          .WriteTo.ApplicationInsights("your-instrumentation-key", TelemetryConverter.Traces)
        //          .MinimumLevel.Information();
        // }
        //
        // Log.Logger = config.CreateLogger();
    }
    
    // ✅ GOOD: Async sinks for better performance
    public static void AsyncSinks()
    {
        // Log.Logger = new LoggerConfiguration()
        //     .WriteTo.Async(a => a.File("logs/app-.log"))  // ✅ File writes on background thread
        //     .WriteTo.Async(a => a.Seq("http://localhost:5341"))  // ✅ Network calls async
        //     .CreateLogger();
        
        // PERFORMANCE: Up to 10x faster with async sinks
        // Logs written on background thread, doesn't block application
    }
}

/// <summary>
/// EXAMPLE 5: FILTERING AND SAMPLING - Control Log Volume
/// 
/// THE PROBLEM:
/// Too many logs in production - noise from health checks, static files.
/// High-volume endpoints logging every request.
/// 
/// THE SOLUTION:
/// Filter out noise, sample high-volume events.
/// 
/// WHY IT MATTERS:
/// - Reduce storage costs
/// - Improve signal-to-noise ratio
/// - Keep only valuable logs
/// - Sample instead of losing everything
/// </summary>
public static class FilteringExamples
{
    // ✅ GOOD: Filter by log content
    public static void ContentFiltering()
    {
        // Log.Logger = new LoggerConfiguration()
        //     // ✅ Exclude health check requests
        //     .Filter.ByExcluding(logEvent =>
        //         logEvent.MessageTemplate.Text.Contains("/health") ||
        //         logEvent.MessageTemplate.Text.Contains("/metrics"))
        //     
        //     // ✅ Exclude static file requests
        //     .Filter.ByExcluding(logEvent =>
        //         logEvent.Properties.ContainsKey("RequestPath") &&
        //         logEvent.Properties["RequestPath"].ToString().Contains(".css"))
        //     
        //     .CreateLogger();
    }
    
    // ✅ GOOD: Filter by source
    public static void SourceFiltering()
    {
        // Log.Logger = new LoggerConfiguration()
        //     // ✅ Suppress noisy Microsoft logs
        //     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        //     .MinimumLevel.Override("System", LogEventLevel.Warning)
        //     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
        //     
        //     // ✅ But keep your application logs
        //     .MinimumLevel.Override("RevisionNotesDemo", LogEventLevel.Debug)
        //     
        //     .CreateLogger();
    }
    
    // ✅ GOOD: Sampling - keep 10% of high-volume events
    public static void SamplingExample()
    {
        // Log.Logger = new LoggerConfiguration()
        //     // ✅ Sample: Only log 10% of Information events
        //     .Filter.ByIncluding(logEvent =>
        //     {
        //         if (logEvent.Level >= LogEventLevel.Warning)
        //             return true;  // Always log warnings and errors
        //         
        //         // Sample 10% of info/debug events
        //         return logEvent.GetHashCode() % 10 == 0;
        //     })
        //     .CreateLogger();
    }
    
    // ✅ GOOD: Sub-loggers with different levels
    public static void SubLoggers()
    {
        // var logger = new LoggerConfiguration()
        //     // ✅ Default: Information
        //     .MinimumLevel.Information()
        //     
        //     // ✅ Verbose logging to file for troubleshooting
        //     .WriteTo.Logger(lc => lc
        //         .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Warning)
        //         .WriteTo.File("logs/errors-.log", rollingInterval: RollingInterval.Day))
        //     
        //     // ✅ Everything to console in development
        //     .WriteTo.Console()
        //     
        //     .CreateLogger();
    }
}

/// <summary>
/// EXAMPLE 6: REQUEST LOGGING - HTTP Request/Response Tracking
/// 
/// THE PROBLEM:
/// Default ASP.NET Core logging is verbose - START/END for every request.
/// 
/// THE SOLUTION:
/// UseSerilogRequestLogging() - single log per request with duration.
/// 
/// WHY IT MATTERS:
/// - One line per request (vs 2+ with default)
/// - Automatic duration timing
/// - Customizable (add headers, query strings)
/// - Status code included
/// 
/// BEFORE:
/// [10:30:00 INF] Executing HTTP GET /api/users
/// [10:30:01 INF] Executed HTTP GET /api/users - 200 OK in 1234ms
/// 
/// AFTER:
/// [10:30:01 INF] HTTP GET /api/users responded 200 in 1234ms
/// </summary>
public static class RequestLoggingExamples
{
    // ✅ GOOD: Basic request logging
    public static void BasicRequestLogging(WebApplication app)
    {
        // ✅ Single line per request
        app.UseSerilogRequestLogging();
        
        // Output: HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms
        // HTTP GET /api/users responded 200 in 125 ms
    }
    
    // ✅ GOOD: Customized request logging
    public static void CustomizedRequestLogging(WebApplication app)
    {
        // app.UseSerilogRequestLogging(options =>
        // {
        //     // ✅ Customize message template
        //     options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000} ms";
        //     
        //     // ✅ Enrich with custom properties
        //     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        //     {
        //         diagnosticContext.Set("UserId", httpContext.User?.FindFirst("sub")?.Value ?? "anonymous");
        //         diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        //         diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        //         diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        //         
        //         // Query string
        //         if (httpContext.Request.QueryString.HasValue)
        //         {
        //             diagnosticContext.Set("QueryString", httpContext.Request.QueryString.Value);
        //         }
        //         
        //         // Response body size
        //         diagnosticContext.Set("ResponseContentLength", httpContext.Response.ContentLength);
        //     };
        //     
        //     // ✅ Custom log level based on response
        //     options.GetLevel = (httpContext, elapsed, ex) =>
        //     {
        //         if (ex != null) return LogEventLevel.Error;
        //         if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
        //         if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;
        //         if (elapsed > 5000) return LogEventLevel.Warning;  // Slow requests
        //         return LogEventLevel.Information;
        //     };
        // });
    }
}

/// <summary>
/// EXAMPLE 7: BEST PRACTICES AND GOTCHAS
/// 
/// THE PROBLEM:
/// Common mistakes with Serilog.
/// 
/// THE SOLUTION:
/// Follow best practices.
/// </summary>
public static class BestPractices
{
    // ✅ DO: Flush and close on application exit
    public static void ProperShutdown()
    {
        // try
        // {
        //     Log.Information("Application starting");
        //     var app = builder.Build();
        //     app.Run();
        // }
        // catch (Exception ex)
        // {
        //     Log.Fatal(ex, "Application failed to start");
        //     throw;
        // }
        // finally
        // {
        //     Log.CloseAndFlush();  // ✅ Very important! Ensures all logs are written
        // }
    }
    
    // ❌ DON'T: Use string interpolation in message templates
    public static void BadTemplating(Microsoft.Extensions.Logging.ILogger logger, int userId)
    {
        // ❌ BAD
        logger.LogInformation($"User {userId} logged in");  // Not structured!
        
        // ✅ GOOD
        logger.LogInformation("User {UserId} logged in", userId);
    }
    
    // ❌ DON'T: Log sensitive information
    public static void SensitiveData(Microsoft.Extensions.Logging.ILogger logger, string password, string creditCard)
    {
        // ❌ NEVER
        logger.LogInformation("User password: {Password}", password);
        
        // ✅ GOOD: Mask or exclude
        logger.LogInformation("User updated credentials");
        
        // ✅ OR: Use destructuring with projection
        var user = new { Id = 123, Username = "john", PasswordHash = "masked" };
        logger.LogInformation("User: {@User}", new { user.Id, user.Username });  // Exclude PasswordHash
    }
    
    // ✅ DO: Use @ for object destructuring
    public static void Destructuring(Microsoft.Extensions.Logging.ILogger logger, Order order)
    {
        // ❌ Without @: just ToString()
        logger.LogInformation("Order: {Order}", order);  // "RevisionNotesDemo.Logging.Order"
        
        // ✅ With @: full serialization
        logger.LogInformation("Order: {@Order}", order);  // { "Id": 1, "Total": 99.99, ... }
    }
    
    // ✅ DO: Configure minimum levels carefully
    // Production: Information or Warning
    // Development: Debug
    // Troubleshooting: Trace
    
    // ✅ DO: Rotate logs to prevent disk-full
    // - retainedFileCountLimit: Keep last N files
    // - fileSizeLimitBytes: Max file size
    // - rollOnFileSizeLimit: Create new file when size exceeded
    
    // ✅ DO: Use JSON formatter for machine reading
    // ✅ DO: Use text formatter for human reading
    
    // ✅ DO: Monitor your log volume (GB/day)
    // ✅ DO: Set up alerts on errors/critical logs
}

// Supporting types
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Note: TelemetryConverter would need Microsoft.ApplicationInsights.Serilog package
// public class TelemetryConverter { public static object Traces { get; } = new(); }
