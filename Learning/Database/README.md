# Database Reference Guide

## Purpose

`Learning/Database` is a reference-only index.
All canonical, runnable database implementations live in `Learning/DataAccess`.

## Where to study and run code

Use these canonical files in `Learning/DataAccess`:

- `AdoNetPatterns.cs`
- `DapperExamples.cs`
- `DatabaseShardingAndScaling.cs`
- `GraphDatabasePatterns.cs`
- `MongoDBWithDotNet.cs`
- `ReadReplicasAndCQRS.cs`
- `RedisPatterns.cs`
- `TimeSeriesDatabases.cs`
- `TransactionPatterns.cs`

Run examples from the project root:

```bash
dotnet run
```

## Why this split exists

- Prevents duplicated topic implementations with diverging guidance.
- Keeps one canonical learning path for runnable examples.
- Preserves a dedicated database navigation entry for discoverability.

## Related docs

- [Primary](../docs/Data-Access.md)
- [Related](../docs/Distributed-Consistency.md)
- [Structure inventory](../docs/Project-Structure-Inventory.md)