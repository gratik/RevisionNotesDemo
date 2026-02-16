# Resilience Guide

## Learning goals

- Understand the main concepts covered in Resilience.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- HTTP failure modes and retries
- Async programming fundamentals

## Runnable examples

- CircuitBreakerPattern.cs - Topic implementation and demonstration code
- PollyRetryPatterns.cs - Topic implementation and demonstration code
- TimeoutAndBulkhead.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../../docs/Resilience.md)
- [Related](../../docs/Performance.md)

