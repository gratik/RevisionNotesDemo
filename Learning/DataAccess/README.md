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
- SqlServer/SqlServerQueriesAndOperations.cs - SQL Server queries, tuning, monitoring, and platform differences
- SqlServer/TableDesignFundamentals.cs - Table design fundamentals and data type/key/constraint guidance
- SqlServer/NormalizationAndDenormalizationPatterns.cs - Normalization strategy and pragmatic denormalization patterns
- SqlServer/RelationalDataModelingPatterns.cs - Soft delete, temporal/audit, and multi-tenant modeling patterns
- SqlServer/PartitioningAndDataLifecyclePatterns.cs - Partitioning and retention lifecycle patterns
- SqlServer/IndexArchitecturePatterns.cs - Clustered/nonclustered/filtered index architecture patterns
- SqlServer/StatisticsAndCardinalityPatterns.cs - Statistics health and cardinality estimation guidance
- SqlServer/ExecutionPlanAnalysisLab.cs - Execution plan analysis workflow and tuning smells
- SqlServer/StoredProcedureDesignStandards.cs - Stored procedure design standards and error handling patterns
- SqlServer/ConcurrencyConsistencyPatterns.cs - Concurrency control and consistency patterns
- SqlServer/BulkIngestionPatterns.cs - Bulk ingestion methods and batching strategy
- SqlServer/OperationalMonitoringRunbook.cs - SQL operational monitoring and incident workflow
- SqlServer/SqlSecurityPatterns.cs - SQL security controls and least-privilege patterns
- TimeSeriesDatabases.cs - Topic implementation and demonstration code
- SqlServer/CteTempTableAndTableVariablePatterns.cs - CTE vs temp table vs table variable selection and examples
- SqlServer/MergeAndUpsertPatterns.cs - MERGE/upsert guidance and safer alternatives
- SqlServer/SqlGraphDataTransferPatterns.cs - Graph load/update patterns for customers, orders, and order items
- SqlServer/TransactionAndIsolationPatterns.cs - SQL Server transactions, isolation levels, and deadlock handling
- EntityFramework/EfCoreTransactionPatterns.cs - EF Core transaction boundaries and shared-transaction examples

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../../docs/Data-Access.md)
- [Related](../../docs/Entity-Framework.md)
- [SQL Server Deep Dive](SqlServer/README.md)
