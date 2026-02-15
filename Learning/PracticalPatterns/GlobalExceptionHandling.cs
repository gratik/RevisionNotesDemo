// ============================================================================
// GLOBAL EXCEPTION HANDLING
// Reference: Revision Notes - Practical Scenarios - Page 13
// ============================================================================
// WHAT IS THIS?
// -------------
// Centralized exception handling using middleware and custom exceptions.
//
// WHY IT MATTERS
// --------------
// ✅ Consistent error responses for clients
// ✅ Safer logging and reduced information leaks
//
// WHEN TO USE
// -----------
// ✅ Web APIs that need uniform error contracts
// ✅ Services that must map errors to HTTP status codes
//
// WHEN NOT TO USE
// ---------------
// ❌ Small console apps with no HTTP response layer
// ❌ Libraries that should not own global error handling
//
// REAL-WORLD EXAMPLE
// ------------------
// API middleware returns ProblemDetails for all errors.
// ============================================================================

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace RevisionNotesDemo.PracticalPatterns;

// ============================================================================
// ❌ BAD EXAMPLE - Scattered exception handling
// ============================================================================

public class UnhandledExceptionService
{
    public string ProcessData(string? input)
    {
        // No validation - will throw NullReferenceException
        return input!.ToUpper(CultureInfo.InvariantCulture);
    }

    public int Divide(int a, int b)
    {
        // No error handling - will throw DivideByZeroException
        return a / b;
    }

    public string ReadFile(string path)
    {
        // No error handling - will throw FileNotFoundException
        return File.ReadAllText(path);
    }
}

// ============================================================================
// ✅ GOOD EXAMPLE 1 - Custom Exceptions
// ============================================================================

// Custom exception hierarchy
public class BusinessException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    public BusinessException(string message, string errorCode, int statusCode = 400)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public class ValidationException : BusinessException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed", "VALIDATION_ERROR", 400)
    {
        Errors = errors;
    }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string resource, string identifier)
        : base($"{resource} with identifier '{identifier}' was not found.", "NOT_FOUND", 404)
    {
    }
}

// ============================================================================
// ✅ GOOD EXAMPLE 2 - Service with proper error handling
// ============================================================================

public class RobustService
{
    private static readonly string[] InputCannotBeNullOrEmpty = ["Input cannot be null or empty"];
    private static readonly string[] DivisorCannotBeZero = ["Divisor cannot be zero"];

    public string ProcessData(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "input", InputCannotBeNullOrEmpty }
            });
        }

        try
        {
            return input.ToUpper(CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            throw new BusinessException("Failed to process data", "PROCESSING_ERROR", 500);
        }
    }

    public int DivideSafely(int a, int b)
    {
        if (b == 0)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { "divisor", DivisorCannotBeZero }
            });
        }

        return a / b;
    }

    public string ReadFileSafely(string path)
    {
        if (!File.Exists(path))
        {
            throw new NotFoundException("File", path);
        }

        try
        {
            return File.ReadAllText(path);
        }
        catch (UnauthorizedAccessException)
        {
            throw new BusinessException("Access denied to file", "ACCESS_DENIED", 403);
        }
        catch (Exception)
        {
            throw new BusinessException("Failed to read file", "FILE_READ_ERROR", 500);
        }
    }
}

// ============================================================================
// ✅ GOOD EXAMPLE 3 - Global Exception Handler Middleware
// ============================================================================

public class ErrorResponse
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public string? StackTrace { get; set; }
}

public class GlobalExceptionHandlerMiddleware
{
    private static readonly Action<ILogger, string, Exception?> UnhandledExceptionOccurred =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(741, nameof(UnhandledExceptionOccurred)),
            "An unhandled exception occurred: {Message}");

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly bool _isDevelopment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _isDevelopment = env.IsDevelopment();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            UnhandledExceptionOccurred(_logger, ex.Message, ex);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ErrorResponse();
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = validationEx.StatusCode;
                response.ErrorCode = validationEx.ErrorCode;
                response.Message = validationEx.Message;
                response.ValidationErrors = validationEx.Errors;
                break;

            case BusinessException businessEx:
                statusCode = businessEx.StatusCode;
                response.ErrorCode = businessEx.ErrorCode;
                response.Message = businessEx.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Forbidden;
                response.ErrorCode = "ACCESS_DENIED";
                response.Message = "You don't have permission to access this resource.";
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorCode = "INTERNAL_ERROR";
                response.Message = _isDevelopment
                    ? exception.Message
                    : "An error occurred while processing your request.";
                break;
        }

        // Include stack trace only in development
        if (_isDevelopment)
        {
            response.StackTrace = exception.StackTrace;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, JsonSerializerOptions);

        return context.Response.WriteAsync(jsonResponse);
    }
}

// Extension method for easy registration
public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

// ============================================================================
// DEMONSTRATION
// ============================================================================

public class GlobalExceptionHandlingDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== GLOBAL EXCEPTION HANDLING DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios - Page 13\n");

        var robustService = new RobustService();

        // Example 1: Validation error
        Console.WriteLine("--- Validation Exception Example ---");
        try
        {
            robustService.ProcessData(null);
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"[VALIDATION] Caught: {ex.ErrorCode} - {ex.Message}");
            Console.WriteLine($"[VALIDATION] Status Code: {ex.StatusCode}");
            foreach (var error in ex.Errors)
            {
                Console.WriteLine($"[VALIDATION]   - {error.Key}: {string.Join(", ", error.Value)}");
            }
        }
        Console.WriteLine();

        // Example 2: Business logic error
        Console.WriteLine("--- Business Exception Example ---");
        try
        {
            robustService.DivideSafely(10, 0);
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"[BUSINESS] Caught: {ex.ErrorCode} - {ex.Message}");
            Console.WriteLine($"[BUSINESS] Status Code: {ex.StatusCode}");
        }
        Console.WriteLine();

        // Example 3: Not found error
        Console.WriteLine("--- Not Found Exception Example ---");
        try
        {
            robustService.ReadFileSafely("nonexistent.txt");
        }
        catch (NotFoundException ex)
        {
            Console.WriteLine($"[NOT_FOUND] Caught: {ex.ErrorCode} - {ex.Message}");
            Console.WriteLine($"[NOT_FOUND] Status Code: {ex.StatusCode}");
        }
        Console.WriteLine();

        // Example 4: Successful operation
        Console.WriteLine("--- Successful Operation ---");
        try
        {
            var result = robustService.ProcessData("hello world");
            Console.WriteLine($"[SUCCESS] Result: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("💡 From Revision Notes - Exception Handling Best Practices:");
        Console.WriteLine("   ✅ Use custom exception types for different error categories");
        Console.WriteLine("   ✅ Global exception middleware for consistent error responses");
        Console.WriteLine("   ✅ Log all exceptions with context");
        Console.WriteLine("   ✅ Return appropriate HTTP status codes");
        Console.WriteLine("   ✅ Hide sensitive details in production (security)");
        Console.WriteLine("   ✅ Include validation errors in structured format");
        Console.WriteLine("   ✅ Use problem details format (RFC 7807) for APIs");
        Console.WriteLine("\n   Middleware Registration: app.UseGlobalExceptionHandler();");
    }
}
