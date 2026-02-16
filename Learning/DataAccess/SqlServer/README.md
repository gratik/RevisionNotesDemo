# SQL Server Deep-Dive Guide

## Learning goals

- Choose between CTEs, temp tables, and table variables based on workload shape.
- Apply safe `MERGE`/upsert approaches without introducing race conditions.
- Move relational graphs (customers/orders/order-items) between SQL Server and C# efficiently.

## Prerequisites

- SQL query fundamentals
- ADO.NET or Dapper basics
- C# collections and LINQ

## Runnable examples

- SqlServerQueriesAndOperations.cs - SQL Server query quality, tuning, monitoring, and platform differences
- TableDesignFundamentals.cs - Table design choices: types, keys, nullability, and constraints
- NormalizationAndDenormalizationPatterns.cs - 1NF/2NF/3NF and pragmatic denormalization strategy
- RelationalDataModelingPatterns.cs - Soft delete, temporal/audit, and multi-tenant modeling patterns
- PartitioningAndDataLifecyclePatterns.cs - Partitioning and retention lifecycle strategy
- IndexArchitecturePatterns.cs - Clustered/nonclustered/filtered index design strategy
- StatisticsAndCardinalityPatterns.cs - Statistics health and cardinality estimation behavior
- ExecutionPlanAnalysisLab.cs - Execution plan reading workflow and common smells
- StoredProcedureDesignStandards.cs - Procedure design and error-handling standards
- ConcurrencyConsistencyPatterns.cs - Rowversion, isolation choices, and locking guidance
- BulkIngestionPatterns.cs - TVP vs SqlBulkCopy vs staged ingestion patterns
- OperationalMonitoringRunbook.cs - Monitoring, incident flow, and restore readiness checks
- SqlSecurityPatterns.cs - Least privilege, RLS/DDM/Always Encrypted patterns
- CteTempTableAndTableVariablePatterns.cs - Decision matrix, good/bad query patterns, and examples
- MergeAndUpsertPatterns.cs - `MERGE` guidance, alternatives, and concurrency-safe upsert templates
- SqlGraphDataTransferPatterns.cs - Graph read/write patterns, delta updates, and anti-patterns
- TransactionAndIsolationPatterns.cs - Transaction boundaries, isolation levels, and deadlock handling

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: row-by-row logic, non-reusable intermediate sets, and broad lock contention.
- Good: set-based operations, measured staging strategy, and explicit concurrency handling.
- Why it matters: improves latency, throughput, and incident predictability under load.

## Related docs

- [Primary](../../../docs/Data-Access.md)
- [Related](../../../docs/Entity-Framework.md)
