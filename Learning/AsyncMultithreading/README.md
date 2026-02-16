# Async and Multithreading Guide

## Learning goals

- Understand the main concepts covered in AsyncMultithreading.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- Task and async/await basics
- Threading fundamentals

## Runnable examples

- AsyncAwaitInternals.cs - Topic implementation and demonstration code
- ConcurrentCollections.cs - Topic implementation and demonstration code
- DeadlockPrevention.cs - Topic implementation and demonstration code
- TaskThreadValueTask.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../../docs/Async-Multithreading.md)
- [Related](../../docs/Performance.md)
