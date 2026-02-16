using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RevisionNotes.Observability.Showcase.Infrastructure;

namespace RevisionNotes.Observability.Showcase.Controllers;

[ApiController]
[Route("observability")]
public sealed class ObservabilityController(
    RequestTelemetry telemetry,
    ILogger<ObservabilityController> logger) : ControllerBase
{
    [HttpGet("success")]
    public IActionResult Success([FromQuery] string? correlationId = null)
    {
        using var activity = telemetry.ActivitySource.StartActivity("observability.success", ActivityKind.Internal);
        activity?.SetTag("correlation.id", correlationId ?? HttpContext.TraceIdentifier);

        telemetry.RecordSuccess();
        logger.LogInformation("Success endpoint called. CorrelationId={CorrelationId}", correlationId ?? HttpContext.TraceIdentifier);

        return Ok(new { Message = "Success event recorded", CorrelationId = correlationId ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("slow")]
    public async Task<IActionResult> Slow([FromQuery] int delayMs = 500)
    {
        var boundedDelay = Math.Clamp(delayMs, 50, 5000);

        using var activity = telemetry.ActivitySource.StartActivity("observability.slow", ActivityKind.Internal);
        activity?.SetTag("delay.ms", boundedDelay);

        await Task.Delay(boundedDelay, HttpContext.RequestAborted);
        telemetry.RecordSuccess();
        logger.LogInformation("Slow endpoint completed with delay {DelayMs}ms", boundedDelay);

        return Ok(new { Message = "Slow endpoint completed", DelayMs = boundedDelay });
    }

    [HttpGet("failure")]
    public IActionResult Failure()
    {
        using var activity = telemetry.ActivitySource.StartActivity("observability.failure", ActivityKind.Internal);
        activity?.SetTag("failure.kind", "demo");

        logger.LogWarning("Failure endpoint intentionally throwing exception for demo");
        throw new InvalidOperationException("Intentional failure for observability demonstration.");
    }

    [HttpGet("log-levels")]
    public IActionResult LogLevels()
    {
        telemetry.RecordSuccess();
        logger.LogTrace("Trace log example");
        logger.LogDebug("Debug log example");
        logger.LogInformation("Information log example");
        logger.LogWarning("Warning log example");

        return Ok(new { Message = "Logged events at multiple levels" });
    }
}