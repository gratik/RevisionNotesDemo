# SQL Server Docs Index

## Metadata
- Owner: Maintainers
- Last updated: February 16, 2026
- Prerequisites: Data Access guide familiarity
- Related examples: Learning/DataAccess/SqlServer/SqlServerQueriesAndOperations.cs

## Pages
- [Bulk Ingestion Patterns](BulkIngestionPatterns.md)
- [Concurrency Consistency Patterns](ConcurrencyConsistencyPatterns.md)
- [Cte Temp Table And Table Variable Patterns](CteTempTableAndTableVariablePatterns.md)
- [Execution Plan Analysis Lab](ExecutionPlanAnalysisLab.md)
- [Index Architecture Patterns](IndexArchitecturePatterns.md)
- [Merge And Upsert Patterns](MergeAndUpsertPatterns.md)
- [Normalization And Denormalization Patterns](NormalizationAndDenormalizationPatterns.md)
- [Operational Monitoring Runbook](OperationalMonitoringRunbook.md)
- [Partitioning And Data Lifecycle Patterns](PartitioningAndDataLifecyclePatterns.md)
- [Relational Data Modeling Patterns](RelationalDataModelingPatterns.md)
- [Sql Graph Data Transfer Patterns](SqlGraphDataTransferPatterns.md)
- [Sql Security Patterns](SqlSecurityPatterns.md)
- [Sql Server Queries And Operations](SqlServerQueriesAndOperations.md)
- [Statistics And Cardinality Patterns](StatisticsAndCardinalityPatterns.md)
- [Stored Procedure Design Standards](StoredProcedureDesignStandards.md)
- [Table Design Fundamentals](TableDesignFundamentals.md)
- [Transaction And Isolation Patterns](TransactionAndIsolationPatterns.md)

---

## Interview Answer Block

- 30-second answer: This index maps each SQL Server concept page to a concrete runnable source file.
- 2-minute deep dive: Use this hub to move from concept to code quickly and validate changes against measured outcomes.
- Common follow-up: How do you keep this maintainable?
- Strong response: Keep one concept per page, one source-of-truth code file, and strict link validation in CI.
- Tradeoff callout: Too much consolidation hurts discoverability; too much fragmentation hurts navigation.

## Interview Bad vs Strong Answer

- Bad answer: "The SQL docs are somewhere in the repo."
- Strong answer: "All SQL Server concept docs are indexed here and each page links directly to the runnable implementation file."
- Why strong wins: It demonstrates clear information architecture and operational usability.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Pick one SQL Server page and explain where the source code lives and how you would validate the pattern in production.
- Required outputs:
  - One design/usage decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.