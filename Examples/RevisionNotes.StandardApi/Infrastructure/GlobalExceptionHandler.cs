using Microsoft.AspNetCore.Diagnostics;

namespace RevisionNotes.StandardApi.Infrastructure;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unexpected server error",
            detail: "The request failed. Check application logs for details.")
            .ExecuteAsync(httpContext);

        return true;
    }
}