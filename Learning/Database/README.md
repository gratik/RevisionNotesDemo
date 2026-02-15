# Database Guide

## Learning goals

- Understand the main concepts covered in Database.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- Database fundamentals
- Read/write scaling concepts

## Runnable examples

- DatabaseShardingAndScaling.cs - Topic implementation and demonstration code
- GraphDatabasePatterns.cs - Topic implementation and demonstration code
- MongoDBWithDotNet.cs - Topic implementation and demonstration code
- ReadReplicasAndCQRS.cs - Topic implementation and demonstration code
- RedisPatterns.cs - Topic implementation and demonstration code
- TimeSeriesDatabases.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../docs/Data-Access.md)
- [Related](../docs/Distributed-Consistency.md)

