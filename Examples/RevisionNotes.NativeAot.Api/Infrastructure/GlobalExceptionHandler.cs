using Microsoft.AspNetCore.Diagnostics;
namespace RevisionNotes.NativeAot.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync("{\"title\":\"Unhandled server error\",\"status\":500,\"detail\":\"See logs for correlation details.\"}", cancellationToken);
        return true;
    }
}
