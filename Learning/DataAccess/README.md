# Data Access Guide

Canonical source note: `Learning/DataAccess` is the single source of truth for runnable data-access examples. `Learning/Database` is reference-only.

## Learning goals

- Understand the main concepts covered in DataAccess.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- SQL and data modeling basics
- Dependency injection and repository concepts

## Runnable examples

- AdoNetPatterns.cs - Topic implementation and demonstration code
- DapperExamples.cs - Topic implementation and demonstration code
- DatabaseShardingAndScaling.cs - Topic implementation and demonstration code
- GraphDatabasePatterns.cs - Topic implementation and demonstration code
- MongoDBWithDotNet.cs - Topic implementation and demonstration code
- ReadReplicasAndCQRS.cs - Topic implementation and demonstration code
- RedisPatterns.cs - Topic implementation and demonstration code
- TimeSeriesDatabases.cs - Topic implementation and demonstration code
- TransactionPatterns.cs - Topic implementation and demonstration code

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
- [Related](../docs/Entity-Framework.md)
