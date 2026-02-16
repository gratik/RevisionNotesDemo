using Microsoft.AspNetCore.Diagnostics;

namespace RevisionNotes.Observability.Showcase.Infrastructure;

public sealed class GlobalExceptionHandler(
    RequestTelemetry telemetry,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        telemetry.RecordFailure();
        logger.LogError(exception, "Unhandled exception on {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unexpected server error",
            detail: "The request failed. Inspect logs and traces for diagnostics.")
            .ExecuteAsync(httpContext);

        return true;
    }
}