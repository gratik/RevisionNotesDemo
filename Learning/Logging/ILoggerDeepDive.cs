// ==============================================================================
// ILogger<T> DEEP DIVE - ASP.NET Core Logging Abstraction
// ==============================================================================
// PURPOSE:
//   Master Microsoft.Extensions.Logging.ILogger<T> for production-grade logging.
//   Learn log levels, scopes, structured parameters, and performance.
//
// WHY I LOGGER:
//   - Built into ASP.NET Core (no external dependencies)
//   - Provider-agnostic (swap providers without code changes)
//   - High performance with source generators (C# 10+)
//   - Dependency injection ready
//   - Structured logging support
//
// WHAT YOU'LL LEARN:
//   1. ILogger<T> basics and DI
//   2. Log levels and when to use each
//   3. Message templates (structured logging)
//   4. Log scopes (context)
//   5. Performance optimization with LoggerMessage
//   6. Testing logging
//
// PROVIDERS:
//   - Console (development)
//   - Debug (Visual Studio)
//   - EventSource
//   - EventLog (Windows)
//   - Serilog (structured, popular)
//   - NLog, Log4Net (legacy)
//   - Azure Application Insights
// ==============================================================================

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xunit;

namespace RevisionNotesDemo.Logging;

/// <summary>
/// EXAMPLE 1: ILogger<T> BASICS - Dependency Injection and Usage
/// 
/// THE PROBLEM:
/// Need logging that works across different providers and environments.
/// Console.WriteLine is not production-ready.
/// 
/// THE SOLUTION:
/// Use ILogger<T> injected via DI - abstracttion over logging infrastructure.
/// 
/// WHY IT MATTERS:
/// - Switch providers without code changes (Console → Serilog → AppInsights)
/// - Testable (mock ILogger)
/// - Type-safe with ILogger<T>
/// - Automatic category naming
/// 
/// PERFORMANCE: ILogger<T> is singleton-safe, minimal allocation
/// </summary>
public static class ILoggerBasics
{
    // ❌ BAD: Using Console.WriteLine
    public class BadUserService
    {
        public void CreateUser(string username)
        {
            Console.WriteLine($"Creating user: {username}"); // Not structured, no levels, can't control
            Console.WriteLine($"Created user at {DateTime.Now}"); // No timestamps management
        }
    }
    
    // ✅ GOOD: Using ILogger<T>
    public class GoodUserService
    {
        private readonly ILogger<GoodUserService> _logger;
        
        // ✅ Inject ILogger<T> - category is fully qualified type name
        public GoodUserService(ILogger<GoodUserService> logger)
        {
            _logger = logger;
        }
        
        public void CreateUser(string username)
        {
            // ✅ Structured, level-aware, provider-agnostic
            _logger.LogInformation("Creating user: {Username}", username);
            
            // ✅ Log level can be controlled via configuration
            _logger.LogDebug("Validating username format");
            
            _logger.LogInformation("User {Username} created successfully", username);
        }
    }
    
    // ✅ GOOD: Configure in Program.cs (ASP.NET Core)
    public static class ProgramSetup
    {
        // Example for Program.cs:
        // var builder = WebApplication.CreateBuilder(args);
        //
        // // Configure logging
        // builder.Logging.ClearProviders(); // Remove default providers
        // builder.Logging.AddConsole();     // Add console
        // builder.Logging.AddDebug();       // Add debug (VS Output)
        // builder.Logging.SetMinimumLevel(LogLevel.Information);
        //
        // // Per-category filtering
        // builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
        // builder.Logging.AddFilter("System", LogLevel.Warning);
        // builder.Logging.AddFilter("RevisionNotesDemo", LogLevel.Debug);
        
        // appsettings.json configuration:
        // {
        //   "Logging": {
        //     "LogLevel": {
        //       "Default": "Information",
        //       "Microsoft": "Warning",
        //       "Microsoft.Hosting.Lifetime": "Information"
        //     }
        //   }
        // }
    }
}

/// <summary>
/// EXAMPLE 2: LOG LEVELS - When to Use Each Level
/// 
/// THE PROBLEM:
/// Logging everything at "Information" floods logs.
/// Not using appropriate levels makes troubleshooting hard.
/// 
/// THE SOLUTION:
/// Use the right log level for the right situation.
/// 
/// WHY IT MATTERS:
/// - Filter noise in production
/// - Enable detailed logging for specific issues
/// - Performance (lower levels skipped if not enabled)
/// - Signal importance
/// 
/// LEVELS (least to most severe):
/// Trace → Debug → Information → Warning → Error → Critical
/// </summary>
public static class LogLevelExamples
{
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }
        
        public async Task<Order> ProcessOrder(int orderId)
        {
            // ✅ TRACE: Very detailed - loop iterations, method entry/exit
            // Disabled by default, only enabled for deep debugging
            _logger.LogTrace("Entering ProcessOrder method with OrderId: {OrderId}", orderId);
            
            // ✅ DEBUG: Developer diagnostic information
            // Useful during development, disabled in production
            _logger.LogDebug("Fetching order {OrderId} from database", orderId);
            
            var order = await FetchOrder(orderId);
            
            if (order == null)
            {
                // ✅ WARNING: Unexpected but handled situation
                _logger.LogWarning("Order {OrderId} not found, will return null", orderId);
                return null!;
            }
            
            // ✅ INFORMATION: Normal flow, significant events
            // Enabled in production, track business events
            _logger.LogInformation("Processing order {OrderId} with {ItemCount} items, total: {Total:C}",
                order.Id, order.Items.Count, order.Total);
            
            try
            {
                await ChargePayment(order);
                
                // ✅ INFORMATION: Successful business operation
                _logger.LogInformation("Payment processed successfully for order {OrderId}, Amount: {Amount:C}",
                    order.Id, order.Total);
            }
            catch (PaymentException ex)
            {
                // ✅ ERROR: Exception occurred, operation failed
                // Needs attention but system continues
                _logger.LogError(ex, "Payment failed for order {OrderId}", order.Id);
                throw;
            }
            catch (Exception ex)
            {
                // ✅ CRITICAL: System-level failure, immediate attention required
                // Database down, critical service unavailable
                _logger.LogCritical(ex, "Critical error processing order {OrderId} - payment system unresponsive", order.Id);
                throw;
            }
            
            _logger.LogTrace("Exiting ProcessOrder method");
            return order;
        }
        
        private Task<Order?> FetchOrder(int orderId) => Task.FromResult<Order?>(new Order { Id = orderId, Items = new(), Total = 100m });
        private Task ChargePayment(Order order) => Task.CompletedTask;
    }
    
    // GUIDELINE - When to use each level:
    //
    // TRACE (LogTrace):
    // ✅ Method entry/exit
    // ✅ Loop iterations
    // ✅ Very detailed flow
    // ❌ Never enabled in production
    //
    // DEBUG (LogDebug):
    // ✅ Variable values during debugging
    // ✅ Decision points (if/switch)
    // ✅ Query details, cache hits/misses
    // ❌ Usually disabled in production
    //
    // INFORMATION (LogInformation):
    // ✅ Business events (order placed, user registered)
    // ✅ Service startup/shutdown
    // ✅ Configuration loaded
    // ✅ API requests (in middleware)
    // ✅ Background job execution
    //
    // WARNING (LogWarning):
    // ✅ Deprecated API usage
    // ✅ Retry attempts
    // ✅ Degraded performance
    // ✅ Recoverable errors (fallback used)
    // ✅ Validation failures (user input)
    //
    // ERROR (LogError):
    // ✅ Exceptions that are handled
    // ✅ Operations that failed
    // ✅ Data inconsistencies
    // ✅ External service failures
    //
    // CRITICAL (LogCritical):
    // ✅ Database unavailable
    // ✅ Disk full
    // ✅ Out of memory
    // ✅ Critical dependencies down
    // ✅ Data corruption
}

/// <summary>
/// EXAMPLE 3: MESSAGE TEMPLATES - Structured Logging
/// 
/// THE PROBLEM:
/// String concatenation or interpolation loses structure.
/// Can't query by specific values, only text search.
/// 
/// THE SOLUTION:
/// Use message templates with named placeholders.
/// 
/// WHY IT MATTERS:
/// - Query logs by structured property (e.g., all orders > $1000)
/// - Automatic serialization
/// - Better performance (no string allocation if level disabled)
/// - Provider-specific benefits (JSON in Serilog)
/// 
/// BEST FOR: Serilog, Application Insights, structured log stores
/// </summary>
public static class MessageTemplateExamples
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        
        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }
        
        public void UpdatePrice(int productId, decimal oldPrice, decimal newPrice)
        {
            // ❌ BAD: String interpolation - not structured
            _logger.LogInformation($"Product {productId} price changed from {oldPrice} to {newPrice}");
            // In Serilog: { "Message": "Product 42 price changed from 10.5 to 12.99" }
            // Can't query by productId as a number
            
            // ✅ GOOD: Message template - structured
            _logger.LogInformation("Product {ProductId} price changed from {OldPrice} to {NewPrice}",
                productId, oldPrice, newPrice);
            // In Serilog: { "ProductId": 42, "OldPrice": 10.5, "NewPrice": 12.99, "Message": "Product 42..." }
            // Can query WHERE ProductId = 42 or WHERE NewPrice > 10
        }
        
        // ✅ GOOD: Format specifiers
        public void FormatExamples()
        {
            var price = 1234.56m;
            var date = DateTime.Now;
            var duration = TimeSpan.FromSeconds(45.678);
            
            // Currency formatting
            _logger.LogInformation("Total: {Total:C}", price); // $1,234.56
            
            // Date formatting
            _logger.LogInformation("Processed at {ProcessedAt:yyyy-MM-dd HH:mm:ss}", date);
            
            // Custom formatting
            _logger.LogInformation("Duration: {Duration:0.00}s", duration.TotalSeconds);
            
            // Number formatting
            _logger.LogInformation("Count: {Count:N0}", 1234567); // 1,234,567
        }
        
        // ✅ GOOD: Destructuring complex objects (Serilog feature)
        public void LogComplexObject(Order order)
        {
            // @ prefix tells Serilog to destructure the object
            _logger.LogInformation("Order received: {@Order}", order);
            // Result: All properties logged as structured data
            // { "Order": { "Id": 1, "Items": [...], "Total": 100.50 }, ... }
            
            // Without @: just ToString()
            _logger.LogInformation("Order received: {Order}", order);
            // Result: { "Order": "RevisionNotesDemo.Logging.Order", ... }
        }
        
        // ❌ BAD: Too many parameters
        public void TooManyParameters()
        {
            _logger.LogInformation("User {UserId} {Action} {Resource} on {Date} from {IP} with {UserAgent}",
                123, "updated", "profile", DateTime.Now, "192.168.1.1", "Mozilla/5.0...");
            // Hard to read, consider using a context object
        }
        
        // ✅ GOOD: Group related properties
        public void GroupedContext()
        {
            var context = new RequestContext
            {
                UserId = 123,
                Action = "updated",
                Resource = "profile",
                Timestamp = DateTime.Now,
                IpAddress = "192.168.1.1"
            };
            
            _logger.LogInformation("Request: {@Context}", context);
        }
    }
}

/// <summary>
/// EXAMPLE 4: LOG SCOPES - Adding Context to Multiple Log Entries
/// 
/// THE PROBLEM:
/// Repeating the same context in every log statement (RequestId, UserId, etc.)
/// 
/// THE SOLUTION:
/// Use BeginScope to add context that applies to multiple log entries.
/// 
/// WHY IT MATTERS:
/// - DRY principle - set once, applies to all logs in scope
/// - Correlation (trace requests across services)
/// - Automatic cleanup (using statement)
/// - Nested scopes supported
/// 
/// BEST FOR: Request tracking, user context, transaction IDs
/// </summary>
public static class LogScopeExamples
{
    public class CheckoutService
    {
        private readonly ILogger<CheckoutService> _logger;
        
        public CheckoutService(ILogger<CheckoutService> logger)
        {
            _logger = logger;
        }
        
        // ❌ BAD: Repeating context everywhere
        public async Task BadProcessCheckout(int orderId, int userId)
        {
            _logger.LogInformation("OrderId: {OrderId}, UserId: {UserId} - Starting checkout", orderId, userId);
            _logger.LogInformation("OrderId: {OrderId}, UserId: {UserId} - Validating items", orderId, userId);
            _logger.LogInformation("OrderId: {OrderId}, UserId: {UserId} - Processing payment", orderId, userId);
            // Tedious and error-prone
        }
        
        // ✅ GOOD: Using scopes
        public async Task GoodProcessCheckout(int orderId, int userId)
        {
            // ✅ Scope applies to all logs within using block
            using (_logger.BeginScope("OrderId: {OrderId}, UserId: {UserId}", orderId, userId))
            {
                _logger.LogInformation("Starting checkout");
                _logger.LogInformation("Validating items");
                
                await ValidateItems(orderId);
                
                _logger.LogInformation("Processing payment");
                
                await ProcessPayment(orderId);
                
                _logger.LogInformation("Checkout complete");
            }
            // All logs above include OrderId and UserId automatically
        }
        
        // ✅ GOOD: Dictionary scope (strongly-typed keys)
        public async Task ScopedWithDictionary(int orderId)
        {
            var scope = new Dictionary<string, object>
            {
                ["OrderId"] = orderId,
                ["SessionId"] = Guid.NewGuid(),
                ["Timestamp"] = DateTime.UtcNow
            };
            
            using (_logger.BeginScope(scope))
            {
                _logger.LogInformation("Processing order");
                // All entries include OrderId, SessionId, Timestamp
            }
        }
        
        // ✅ GOOD: Nested scopes
        public async Task NestedScopes(int userId, int orderId)
        {
            using (_logger.BeginScope("UserId: {UserId}", userId))
            {
                _logger.LogInformation("User scope"); // Includes UserId
                
                using (_logger.BeginScope("OrderId: {OrderId}", orderId))
                {
                    _logger.LogInformation("Order scope"); // Includes BOTH UserId and OrderId
                    
                    await ProcessPayment(orderId);
                }
                
                _logger.LogInformation("Back to user scope"); // Only UserId
            }
        }
        
        private Task ValidateItems(int orderId) => Task.CompletedTask;
        private Task ProcessPayment(int orderId) => Task.CompletedTask;
    }
    
    // ✅ BEST PRACTICE: Middleware for request-scoped logging
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            
            // ✅ Scope for entire request
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["Method"] = context.Request.Method,
                ["Path"] = context.Request.Path,
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString()
            }))
            {
                _logger.LogInformation("Request started");
                
                var stopwatch = Stopwatch.StartNew();
                
                await _next(context);
                
                stopwatch.Stop();
                
                _logger.LogInformation("Request completed in {ElapsedMs}ms with status {StatusCode}",
                    stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
    }
}

/// <summary>
/// EXAMPLE 5: PERFORMANCE - LoggerMessage Source Generators (C# 10+)
/// 
/// THE PROBLEM:
/// ILogger allocates strings and objects on every call, even if level is disabled.
/// High-throughput scenarios need better performance.
/// 
/// THE SOLUTION:
/// Use LoggerMessage.Define or source generators (C# 10+) for zero-allocation logging.
/// 
/// WHY IT MATTERS:
/// - Up to 10x faster than standard LogInformation
/// - Zero allocations if log level disabled
/// - Compile-time generated delegates
/// - No runtime overhead
/// 
/// PERFORMANCE: 
/// - Standard: ~300ns + allocations
/// - LoggerMessage: ~30ns, zero allocations
/// 
/// BEST FOR: Hot paths, high-frequency logging
/// </summary>
public static partial class HighPerformanceLogging
{
    // ❌ SLOW: Standard logging (allocates on every call)
    public static void SlowLogging(ILogger logger, int orderId, int itemCount)
    {
        // Allocates even if DEBUG is disabled
        logger.LogDebug("Processing order {OrderId} with {ItemCount} items", orderId, itemCount);
    }
    
    // ✅ FAST: Source generator approach (C# 10+)
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Debug,
        Message = "Processing order {orderId} with {itemCount} items")]
    public static partial void FastLogging(ILogger logger, int orderId, int itemCount);
    
    // ✅ Multiple examples
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "User {userId} logged in from {ipAddress}")]
    public static partial void UserLoggedIn(ILogger logger, int userId, string ipAddress);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Payment retry {attempt} of {maxAttempts} for order {orderId}")]
    public static partial void PaymentRetry(ILogger logger, int attempt, int maxAttempts, int orderId);
    
    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Failed to process order {orderId}")]
    public static partial void OrderProcessingFailed(ILogger logger, int orderId, Exception exception);
    
    // Usage:
    public class PerformanceExampleUsage
    {
        private readonly ILogger<PerformanceExampleUsage> _logger;
        
        public PerformanceExampleUsage(ILogger<PerformanceExampleUsage> logger)
        {
            _logger = logger;
        }
        
        public void ProcessOrder(int orderId)
        {
            // ✅ Fast, zero-allocation logging
            FastLogging(_logger, orderId, 10);
            
            try
            {
                // ... process order
            }
            catch (Exception ex)
            {
                OrderProcessingFailed(_logger, orderId, ex);
            }
        }
    }
}

/// <summary>
/// EXAMPLE 6: TESTING LOGGING
/// 
/// THE PROBLEM:
/// How to verify logging in unit tests?
/// 
/// THE SOLUTION:
/// Use test loggers or mocking.
/// 
/// WHY IT MATTERS:
/// - Verify critical events are logged
/// - Test log content
/// - Ensure proper log levels
/// </summary>
public static class TestingLogging
{
    // ✅ OPTION 1: Use NullLogger for tests that don't care about logging
    public class SimpleTest
    {
        [Fact]
        public void Test_WithoutCheckingLogs()
        {
            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<MyService>.Instance;
            var service = new MyService(logger);
            
            // Test service logic without worrying about logs
            service.DoSomething();
        }
    }
    
    // ✅ OPTION 2: Use ITestOutputHelper (xUnit)
    public class TestWithOutput
    {
        private readonly ILogger<MyService> _logger;
        
        public TestWithOutput(Xunit.Abstractions.ITestOutputHelper output)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new XunitLoggerProvider(output));
            });
            _logger = loggerFactory.CreateLogger<MyService>();
        }
        
        [Fact]
        public void Test_LogsVisibleInTestOutput()
        {
            var service = new MyService(_logger);
            service.DoSomething();
            // Logs appear in test output
        }
    }
    
    // ✅ OPTION 3: Custom test logger to verify logs
    public class LogVerificationTest
    {
        [Fact]
        public void Test_VerifyLogContent()
        {
            var testLogger = new TestLogger<MyService>();
            var service = new MyService(testLogger);
            
            service.DoSomething();
            
            // ✅ Verify log was written
            Xunit.Assert.Contains(testLogger.Logs, log =>
                log.Level == LogLevel.Information &&
                log.Message.Contains("Something happened"));
        }
    }
    
    public class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Logs { get; } = new();
        
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        
        public bool IsEnabled(LogLevel logLevel) => true;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Logs.Add(new LogEntry
            {
                Level = logLevel,
                EventId = eventId,
                Message = formatter(state, exception),
                Exception = exception
            });
        }
        
        public class LogEntry
        {
            public LogLevel Level { get; init; }
            public EventId EventId { get; init; }
            public string Message { get; init; } = string.Empty;
            public Exception? Exception { get; init; }
        }
    }
}

// Supporting types (minimal for this file)
public class PaymentException : Exception
{
    public PaymentException(string message) : base(message) { }
}

public class RequestContext
{
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}

public class MyService
{
    private readonly ILogger _logger;
    
    public MyService(ILogger logger)
    {
        _logger = logger;
    }
    
    public void DoSomething()
    {
        _logger.LogInformation("Something happened");
    }
}

// xUnit test support
public class XunitLoggerProvider : ILoggerProvider
{
    private readonly Xunit.Abstractions.ITestOutputHelper _output;
    
    public XunitLoggerProvider(Xunit.Abstractions.ITestOutputHelper output)
    {
        _output = output;
    }
    
    public ILogger CreateLogger(string categoryName) => new XunitLogger<object>(_output, categoryName);
    
    public void Dispose() { }
}

public class XunitLogger<T> : ILogger<T>
{
    private readonly Xunit.Abstractions.ITestOutputHelper _output;
    private readonly string _categoryName;
    
    public XunitLogger(Xunit.Abstractions.ITestOutputHelper output, string categoryName)
    {
        _output = output;
        _categoryName = categoryName;
    }
    
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    
    public bool IsEnabled(LogLevel logLevel) => true;
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _output.WriteLine($"[{logLevel}] {_categoryName}: {formatter(state, exception)}");
    }
}

// Fact attribute for testing examples
public class FactAttribute : Attribute { }
