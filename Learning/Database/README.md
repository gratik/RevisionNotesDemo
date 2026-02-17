# Database Reference Guide

## Learning goals

`Learning/Database` is a reference-only index.
All canonical, runnable database implementations live in `Learning/DataAccess`.

## Prerequisites

- Basic SQL and relational modeling concepts.
- Familiarity with .NET data access options (ADO.NET, Dapper, EF Core).
- Ability to run the repository from the solution root.

## Runnable examples

Use these canonical files in `Learning/DataAccess`:

- `AdoNetPatterns.cs`
- `DapperExamples.cs`
- `DatabaseShardingAndScaling.cs`
- `GraphDatabasePatterns.cs`
- `MongoDBWithDotNet.cs`
- `ReadReplicasAndCQRS.cs`
- `RedisPatterns.cs`
- `TimeSeriesDatabases.cs`
- `SqlServer/TransactionAndIsolationPatterns.cs`
- `EntityFramework/EfCoreTransactionPatterns.cs`

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Prevents duplicated topic implementations with diverging guidance.
- Keeps one canonical learning path for runnable examples.
- Preserves a dedicated database navigation entry for discoverability.

## Related docs

- [Primary](../../docs/Data-Access.md)
- [Related](../../docs/Distributed-Consistency.md)
- [Structure inventory](../../docs/Project-Structure-Inventory.md)
