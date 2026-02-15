// ==============================================================================
// LOGGING BEST PRACTICES - Production Patterns and Anti-Patterns
// ==============================================================================
// WHAT IS THIS?
// -------------
// Guidance on what, when, and how to log in production.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents noisy logs and log-induced outages
// ✅ Protects sensitive data while improving diagnostics
//
// WHEN TO USE
// -----------
// ✅ Any production service with observability needs
// ✅ Systems requiring audit trails and troubleshooting
//
// WHEN NOT TO USE
// ---------------
// ❌ Skipping these practices in production
// ❌ Logging secrets or personally identifiable data
//
// REAL-WORLD EXAMPLE
// ------------------
// Correlation IDs for end-to-end tracing.
// ==============================================================================

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RevisionNotesDemo.Logging;

/// <summary>
/// EXAMPLE 1: CORRELATION IDS - Tracing Requests Across Services
/// 
/// THE PROBLEM:
/// In distributed systems, single request touches multiple services.
/// Hard to trace request through logs without correlation.
/// 
/// THE SOLUTION:
/// Generate correlation ID at entry point, pass through entire request chain.
/// Log correlation ID with every entry.
/// 
/// WHY IT MATTERS:
/// - Trace requests across microservices
/// - Find all logs for specific request
/// - Debug distributed workflows
/// - Performance profiling per request
/// 
/// TOOLS: Application Insights, Seq, ELK can query by correlation ID
/// </summary>
public static class CorrelationIdExamples
{
    // ✅ GOOD: Middleware to generate and track correlation ID
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ✅ Check if correlation ID already exists (from upstream service)
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            // ✅ Add to response headers for downstream services
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            // ✅ Add to HttpContext.Items for use throughout request
            context.Items["CorrelationId"] = correlationId;

            // ✅ Add to logging scope - all logs include this
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                _logger.LogInformation("Request started: {Method} {Path}", context.Request.Method, context.Request.Path);

                await _next(context);

                _logger.LogInformation("Request completed: {StatusCode}", context.Response.StatusCode);
            }
        }
    }

    // ✅ GOOD: Using correlation ID in service
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ILogger<OrderService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task PlaceOrder(int orderId)
        {
            // ✅ Correlation ID automatically included from scope
            _logger.LogInformation("Processing order {OrderId}", orderId);

            // ✅ Pass to downstream service
            await CallPaymentService(orderId);
        }

        private async Task CallPaymentService(int orderId)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"] as string;

            // ✅ Include in HTTP request to downstream service
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

            _logger.LogInformation("Calling payment service for order {OrderId}", orderId);
            // await client.PostAsync("https://payment-service/pay", ...);
        }
    }

    // QUERY EXAMPLE in Seq/AppInsights:
    // CorrelationId = "abc-123-def"
    // Returns ALL logs across ALL services for that request
}

/// <summary>
/// EXAMPLE 2: PERFORMANCE LOGGING - Timing and Profiling
/// 
/// THE PROBLEM:
/// Need to identify slow operations and performance bottlenecks.
/// 
/// THE SOLUTION:
/// Log execution duration for operations, database queries, external calls.
/// 
/// WHY IT MATTERS:
/// - Identify performance regressions
/// - Set up alerts for slow operations
/// - Understand which operations dominate request time
/// - Data for optimization decisions
/// 
/// BEST FOR: API endpoints, database queries, external service calls
/// </summary>
public static class PerformanceLoggingExamples
{
    // ✅ GOOD: Using Stopwatch for timing
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
        }

        public async Task<List<Product>> GetProducts()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var products = await FetchFromDatabase();

                stopwatch.Stop();

                // ✅ Log duration for successful operations
                _logger.LogInformation("Fetched {Count} products in {ElapsedMs}ms",
                    products.Count, stopwatch.ElapsedMilliseconds);

                // ⚠️ Warning if slow
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    _logger.LogWarning("Slow query detected: {ElapsedMs}ms (> 1000ms)", stopwatch.ElapsedMilliseconds);
                }

                return products;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Failed to fetch products after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private Task<List<Product>> FetchFromDatabase() => Task.FromResult(new List<Product>());
    }

    // ✅ BETTER: Disposable timing helper
    public class TimingLogger : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _operation;
        private readonly Stopwatch _stopwatch;
        private readonly LogLevel _level;

        public TimingLogger(ILogger logger, string operation, LogLevel level = LogLevel.Information)
        {
            _logger = logger;
            _operation = operation;
            _level = level;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.Log(_level, "{Operation} completed in {ElapsedMs}ms", _operation, _stopwatch.ElapsedMilliseconds);
        }
    }

    public class ServiceWithTimingLogger
    {
        private readonly ILogger<ServiceWithTimingLogger> _logger;

        public ServiceWithTimingLogger(ILogger<ServiceWithTimingLogger> logger)
        {
            _logger = logger;
        }

        public async Task ProcessOrder(int orderId)
        {
            // ✅ Automatic timing with using statement
            using (new TimingLogger(_logger, "ProcessOrder"))
            {
                await ValidateOrder(orderId);
                await ChargePayment(orderId);
                await SendConfirmation(orderId);
            }
            // Logs: "ProcessOrder completed in 1234ms"
        }

        private Task ValidateOrder(int orderId) => Task.CompletedTask;
        private Task ChargePayment(int orderId) => Task.CompletedTask;
        private Task SendConfirmation(int orderId) => Task.CompletedTask;
    }

    // ✅ GOOD: Activity/DiagnosticSource for distributed tracing
    public class DistributedTracingExample
    {
        private static readonly ActivitySource ActivitySource = new("MyApp.OrderService");

        public async Task ProcessOrderWithTracing(int orderId)
        {
            using var activity = ActivitySource.StartActivity("ProcessOrder");
            activity?.SetTag("order.id", orderId);

            // Automatic timing + distributed tracing
            await Task.Delay(100);

            activity?.SetTag("order.status", "completed");
        }
        // Works with Application Insights, OpenTelemetry
    }
}

/// <summary>
/// EXAMPLE 3: SENSITIVE DATA - Never Log Secrets, PII, Credentials
/// 
/// THE PROBLEM:
/// Accidentally logging passwords, credit cards, API keys, PII.
/// Compliance issues (GDPR, PCI-DSS, HIPAA).
/// 
/// THE SOLUTION:
/// Sanitize logs, mask sensitive data, use Data Annotations.
/// 
/// WHY IT MATTERS:
/// - Legal compliance
/// - Security (logs often less protected than databases)
/// - Customer trust
/// - Avoid data breaches through logs
/// 
/// NEVER LOG:
/// - Passwords, password hashes
/// - Credit card numbers, CVV
/// - API keys, tokens, secrets
/// - SSNs, health data
/// - Full email addresses (in some jurisdictions)
/// </summary>
public static class SensitiveDataExamples
{
    // ❌ BAD: Logging sensitive data
    public class InsecureLogging
    {
        private readonly ILogger _logger;

        public InsecureLogging(ILogger logger)
        {
            _logger = logger;
        }

        public void BadLogin(string username, string password)
        {
            // ❌ NEVER!!
            _logger.LogInformation("Login attempt: Username={Username}, Password={Password}", username, password);
        }

        public void BadPayment(string cardNumber, string cvv)
        {
            // ❌ NEVER!!
            _logger.LogInformation("Processing payment: Card={CardNumber}, CVV={CVV}", cardNumber, cvv);
        }
    }

    // ✅ GOOD: Safe logging
    public class SecureLogging
    {
        private readonly ILogger _logger;

        public SecureLogging(ILogger logger)
        {
            _logger = logger;
        }

        public void GoodLogin(string username)
        {
            // ✅ Log event, not credentials
            _logger.LogInformation("Login attempt: Username={Username}", username);
            // Password not logged at all
        }

        public void GoodPayment(string cardNumber)
        {
            // ✅ Mask sensitive data
            var maskedCard = MaskCardNumber(cardNumber);
            _logger.LogInformation("Processing payment: Card={MaskedCard}", maskedCard);
            // Output: "Processing payment: Card=****-****-****-1234"
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (cardNumber.Length < 4) return "****";
            return "****-****-****-" + cardNumber.Substring(cardNumber.Length - 4);
        }

        // ✅ GOOD: Log sanitized user objects
        public void GoodUserUpdate(User user)
        {
            // ✅ Use anonymous type to exclude sensitive properties
            _logger.LogInformation("User updated: {@User}",
                new
                {
                    user.Id,
                    user.Username,
                    user.Email, // Mask if GDPR-sensitive
                    // PasswordHash explicitly excluded
                    // EmailConfirmed, etc. - only relevant properties
                });
        }
    }

    // ✅ GOOD: Custom sanitizer for complex objects
    public class LogSanitizer
    {
        private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "Password", "PasswordHash", "Secret", "Token", "ApiKey",
            "CreditCard", "CardNumber", "CVV", "SSN", "SocialSecurity"
        };

        public static object Sanitize(object obj)
        {
            // Use reflection to mask sensitive properties
            // Implementation left as exercise
            // Or use library like Destructurama.Attributed
            return obj;
        }
    }
}

/// <summary>
/// EXAMPLE 4: EXCEPTION LOGGING - Capturing Error Context
/// 
/// THE PROBLEM:
/// Exceptions logged without enough context to reproduce.
/// 
/// THE SOLUTION:
/// Log exception with relevant context: inputs, state, operation.
/// 
/// WHY IT MATTERS:
/// - Reproduce bugs
/// - Understand why exception occurred
/// - Fix root cause, not symptoms
/// 
/// INCLUDE:
/// - Input parameters
/// - Operation being performed
/// - Relevant state
/// - User/tenant ID
/// </summary>
public static class ExceptionLoggingExamples
{
    public class PaymentService
    {
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }

        // ❌ BAD: Insufficient context
        public async Task BadProcessPayment(int orderId, decimal amount)
        {
            try
            {
                await ChargeCard(amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment failed");
                // Which order? What amount? What user?
                throw;
            }
        }

        // ✅ GOOD: Rich context
        public async Task GoodProcessPayment(int orderId, int userId, decimal amount, string currency)
        {
            try
            {
                _logger.LogInformation("Processing payment: OrderId={OrderId}, UserId={UserId}, Amount={Amount}, Currency={Currency}",
                    orderId, userId, amount, currency);

                await ChargeCard(amount);

                _logger.LogInformation("Payment successful: OrderId={OrderId}", orderId);
            }
            catch (PaymentGatewayException ex)
            {
                // ✅ Log all relevant context
                _logger.LogError(ex,
                    "Payment gateway error: OrderId={OrderId}, UserId={UserId}, Amount={Amount}, Currency={Currency}, ErrorCode={ErrorCode}",
                    orderId, userId, amount, currency, ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                // ✅ Unexpected error - log everything
                _logger.LogCritical(ex,
                    "Unexpected payment error: OrderId={OrderId}, UserId={UserId}, Amount={Amount}, Currency={Currency}",
                    orderId, userId, amount, currency);
                throw;
            }
        }

        // ✅ GOOD: Log BEFORE exception for partially completed operations
        public async Task RefundOrder(int orderId, decimal amount)
        {
            _logger.LogInformation("Starting refund: OrderId={OrderId}, Amount={Amount}", orderId, amount);

            try
            {
                await UpdateOrderStatus(orderId, "Refunding");
                _logger.LogInformation("Order status updated to Refunding");

                await ProcessRefund(orderId, amount);
                _logger.LogInformation("Refund processed successfully");

                await UpdateOrderStatus(orderId, "Refunded");
                _logger.LogInformation("Refund completed: OrderId={OrderId}", orderId);
            }
            catch (Exception ex)
            {
                // ✅ Logs above show how far we got before failure
                _logger.LogError(ex, "Refund failed: OrderId={OrderId}, Amount={Amount}", orderId, amount);
                throw;
            }
        }

        private Task ChargeCard(decimal amount) => Task.CompletedTask;
        private Task UpdateOrderStatus(int orderId, string status) => Task.CompletedTask;
        private Task ProcessRefund(int orderId, decimal amount) => Task.CompletedTask;
    }
}

/// <summary>
/// EXAMPLE 5: LOG LEVELS STRATEGY - When to Use Each Level
/// 
/// THE PROBLEM:
/// Everything logged at Information, or inconsistent level usage.
/// 
/// THE SOLUTION:
/// Clear strategy for each log level across team.
/// 
/// WHY IT MATTERS:
/// - Filter noise in production
/// - Alert on errors/critical
/// - Troubleshoot with debug logs
/// - Consistent across codebase
/// </summary>
public static class LogLevelStrategy
{
    public class WellLoggedService
    {
        private readonly ILogger<WellLoggedService> _logger;

        public WellLoggedService(ILogger<WellLoggedService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessData(int dataId)
        {
            // ✅ TRACE: Very detailed, method entry/exit
            // Only enabled during deep debugging
            _logger.LogTrace("Entering ProcessData: DataId={DataId}", dataId);

            // ✅ DEBUG: Developer diagnostics
            // Useful during development, disabled in prod
            _logger.LogDebug("Fetching data from cache");
            var data = await FetchData(dataId);

            if (data == null)
            {
                // ✅ WARNING: Unexpected but handled
                _logger.LogWarning("Data {DataId} not found in cache, fetching from database", dataId);
                data = await FetchFromDatabase(dataId);
            }

            // ✅ INFORMATION: Significant business event  _logger.LogInformation("Processing data: DataId={DataId}, Size={Size}", dataId, data.Length);

            try
            {
                await ProcessInternal(data);
            }
            catch (TransientException ex)
            {
                // ✅ WARNING: Transient error, will retry
                _logger.LogWarning(ex, "Transient error processing data {DataId}, will retry", dataId);
                await Task.Delay(1000);
                await ProcessInternal(data);
            }
            catch (ValidationException ex)
            {
                // ✅ ERROR: Operation failed, handled
                _logger.LogError(ex, "Validation failed for data {DataId}", dataId);
                throw;
            }
            catch (Exception ex)
            {
                // ✅ CRITICAL: Unexpected, system-level failure
                _logger.LogCritical(ex, "Critical error processing data {DataId} - system may be unstable", dataId);
                throw;
            }

            _logger.LogTrace("Exiting ProcessData");
        }

        private Task<byte[]> FetchData(int id) => Task.FromResult(new byte[0]);
        private Task<byte[]> FetchFromDatabase(int id) => Task.FromResult(new byte[0]);
        private Task ProcessInternal(byte[] data) => Task.CompletedTask;
    }

    // PRODUCTION MINIMUM LEVELS:
    // ASP.NET Core app:
    // - Your code: Information
    // - Microsoft.*: Warning
    // - System.*: Warning
    //
    // Background job:
    // - Your code: Information
    // - Libraries: Warning
    //
    // Development:
    // - Everything: Debug or Trace
}

/// <summary>
/// EXAMPLE 6: COMMON ANTI-PATTERNS - What NOT to Do
/// 
/// THE PROBLEM:
/// Common logging mistakes that hurt performance, security, or debuggability.
/// 
/// THE SOLUTION:
/// Avoid these anti-patterns.
/// </summary>
public class AntiPatterns
{
    private static ILogger<AntiPatterns> _logger = null!;

    // ❌ ANTI-PATTERN 1: Logging in loops
    public static async Task BadLoggingInLoop(List<int> ids)
    {
        foreach (var id in ids)
        {
            // ❌ If ids.Count = 10,000, you get 10,000 log entries!
            _logger.LogInformation("Processing item {Id}", id);
            await Task.Delay(1);
        }
    }

    // ✅ BETTER: Log summary
    public static async Task GoodLoggingInLoop(List<int> ids)
    {
        _logger.LogInformation("Processing {Count} items", ids.Count);

        foreach (var id in ids)
        {
            await Task.Delay(1);
        }

        _logger.LogInformation("Completed processing {Count} items", ids.Count);
    }

    // ❌ ANTI-PATTERN 2: Logging huge objects
    public static void BadLogLargeObject(byte[] data)
    {
        // ❌ If data is 10MB, your log entry is 10MB!
        _logger.LogInformation("Received data: {Data}", data);
    }

    // ✅ BETTER: Log metadata only
    public static void GoodLogLargeObject(byte[] data)
    {
        _logger.LogInformation("Received data: Size={Size} bytes, Hash={Hash}",
            data.Length, ComputeHash(data));
    }

    // ❌ ANTI-PATTERN 3: Catching and logging, then rethrowing
    public static async Task BadExceptionPattern()
    {
        try
        {
            await DoWork();
        }
        catch (Exception ex)
        {
            // ❌ Logged here
            _logger.LogError(ex, "Error in BadExceptionPattern");
            throw; // ❌ Will be logged again by global handler
        }
        // Result: Duplicate logs for same exception
    }

    // ✅ BETTER: Let global handler log, or handle here
    public static async Task GoodExceptionPattern()
    {
        try
        {
            await DoWork();
        }
        catch (Exception ex)
        {
            // ✅ If you have context to add, log here and rethrow
            _logger.LogError(ex, "Error in GoodExceptionPattern with context X");
            throw;
        }
        // OR: Don't catch, let global handler log
    }

    // ❌ ANTI-PATTERN 4: String interpolation instead of templates
    public static void BadTemplating(int userId, string action)
    {
        // ❌ Not structured!
        _logger.LogInformation($"User {userId} performed {action}");
    }

    // ✅ GOOD: Use templates
    public static void GoodTemplating(int userId, string action)
    {
        _logger.LogInformation("User {UserId} performed {Action}", userId, action);
    }

    // ❌ ANTI-PATTERN 5: Checking if logging enabled manually
    public static void BadIsEnabledCheck()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            // ❌ Unnecessary - LogDebug does this internally
            _logger.LogDebug("Debug message");
        }
    }

    // ✅ GOOD: Just log - automatic check
    public static void GoodDirectLogging()
    {
        // ✅ No allocation if Debug is disabled
        _logger.LogDebug("Debug message");
    }

    // ✅ EXCEPTION: Only use IsEnabled for expensive computations
    public static void WhenToUseIsEnabled()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            // ✅ OK here - avoid expensive computation if debug disabled
            var expensiveData = ComputeExpensiveDebugInfo();
            _logger.LogDebug("Debug info: {Data}", expensiveData);
        }
    }

    private static string ComputeHash(byte[] data) => "hash";
    private static Task DoWork() => Task.CompletedTask;
    private static string ComputeExpensiveDebugInfo() => "expensive";
}

/// <summary>
/// SUMMARY: Logging Checklist
/// 
/// ✅ DO:
/// - Use structured logging (message templates)
/// - Include correlation IDs
/// - Log business events and errors
/// - Include sufficient context
/// - Time long-running operations
/// - Use appropriate log levels
/// - Configure log rotation
/// - Test that logs are useful
/// 
/// ❌ DON'T:
/// - Log secrets or PII
/// - Log in tight loops
/// - Log huge objects
/// - Use string interpolation in templates
/// - Ignore log volume
/// - Log everything at Information
/// - Forget to flush on shutdown
/// 
/// PRODUCTION CHECKLIST:
/// ✅ Correlation IDs in all requests
/// ✅ Request logging middleware
/// ✅ Log rotation configured
/// ✅ Centralized logging (Seq/ELK/Splunk)
/// ✅ Alerts on Error/Critical logs
/// ✅ Performance baselines (slow query alerts)
/// ✅ Sensitive data sanitization
/// ✅ Log volume monitoring (GB/day)
/// </summary>

// Supporting types
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PaymentGatewayException : Exception
{
    public string ErrorCode { get; set; } = string.Empty;
    public PaymentGatewayException(string message) : base(message) { }
}

public class TransientException : Exception { }
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
