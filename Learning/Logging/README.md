# Logging Guide

## Learning goals

- Understand the main concepts covered in Logging.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- ILogger and ASP.NET pipeline basics
- Operational monitoring concepts

## Runnable examples

- ILoggerDeepDive.cs - Topic implementation and demonstration code
- LoggingBestPractices.cs - Topic implementation and demonstration code
- StructuredLogging.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../docs/Logging-Observability.md)
- [Related](../docs/HealthChecks.md)

