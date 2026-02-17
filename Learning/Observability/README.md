# Observability Guide

## Learning goals

- Understand the main concepts covered in Observability.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- Logging and metrics fundamentals
- Distributed-system troubleshooting context

## Runnable examples

- ApplicationInsightsIntegration.cs - Topic implementation and demonstration code
- DistributedTracingJaegerZipkin.cs - Topic implementation and demonstration code
- HealthChecksAndHeartbeats.cs - Topic implementation and demonstration code
- OpenTelemetryAndApplicationInsightsIntegration.cs - Topic implementation and demonstration code
- OpenTelemetrySetup.cs - Topic implementation and demonstration code
- PrometheusAndGrafana.cs - Topic implementation and demonstration code
- StructuredLoggingAdvanced.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../../docs/Logging-Observability.md)
- [Related](../../docs/HealthChecks.md)
