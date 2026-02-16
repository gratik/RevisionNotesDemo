# SQL Server Deep Dive

> Subject: [Data-Access](../README.md)

## SQL Server Deep Dive

### Useful Topics to Master

- Query design: SARGable predicates, projection discipline, parameterization
- Schema design: key strategy, constraints, normalized vs denormalized tradeoffs
- Programmability: stored procedures, inline TVFs, user-defined table types (TVPs)
- Table design: type sizing, nullability, defaults, and check constraints
- Normalization strategy with pragmatic denormalization for read models
- Modeling patterns: soft delete, temporal/audit, and multi-tenant options
- Intermediate sets: CTE vs #temp table vs @table variable decision-making
- Upsert semantics: `MERGE` vs explicit `UPDATE + INSERT` patterns
- Graph transfer: efficient load/update for customers, orders, order-items
- Partitioning and data lifecycle strategy
- Index architecture and usage-driven maintenance
- Statistics/cardinality management and plan-stability tuning
- Execution-plan analysis workflow
- Concurrency controls (rowversion, isolation, locking)
- Bulk ingestion strategy (TVP, SqlBulkCopy, staged loads)
- Monitoring runbook and SQL security controls
- Performance tuning: Query Store, plans, indexes, wait analysis, and regressions
- Monitoring and operations: backups, restore drills, deadlock/timeout trend analysis
- Troubleshooting: blocking, parameter sniffing, tempdb pressure, and plan instability

### Best Practices

- Use range predicates on indexed columns instead of wrapping columns in functions.
- Keep transactions short and explicit.
- Design indexes from real workload patterns, not theory.
- Prefer inline TVFs over multi-statement TVFs for critical paths.
- Validate every change with before/after metrics (logical reads, CPU, p95 latency).

### Bad Practices

- `SELECT *` in API hot paths.
- Scalar UDFs in large scans/joins.
- No foreign keys "for speed" (causes silent data integrity drift).
- Over-indexing write-heavy tables without usage review.
- Tuning by guesswork without Query Store or wait stats.

### SQL Server Deployment Models: Differences and Restrictions

| Model | What you manage | Typical strengths | Typical restrictions/tradeoffs |
| --- | --- | --- | --- |
| Self-managed SQL Server (on-prem or self-managed VM) | You manage full stack: OS, SQL engine, HA/DR, patching, backups | Full control and broadest feature surface | Highest operational burden and incident ownership |
| Hosted SQL Server instance (IaaS hosted by provider/cloud VM) | Host manages infrastructure; SQL operations often still mostly yours | High compatibility for lift-and-shift workloads | Still significant DBA/ops ownership; cloud infra costs and sizing decisions |
| Azure SQL Managed Instance | Microsoft manages more platform operations | High SQL Server compatibility with managed service benefits | Less instance/OS control than self-managed SQL Server; validate feature parity per workload |
| Azure SQL Database | Microsoft-managed PaaS per database/elastic pool model | Strongest PaaS automation, scaling, and operational simplicity | More feature constraints vs full instance model; design for per-database boundaries |

Migration note:
- Validate SQL Agent job model, cross-database dependencies, server-level settings, extensibility assumptions, and backup/restore workflow before choosing target.

---

## Detailed Guidance

SQL Server Deep Dive guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for SQL Server Deep Dive before implementation work begins.
- Keep boundaries explicit so SQL Server Deep Dive decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring SQL Server Deep Dive in production-facing code.
- When performance, correctness, or maintainability depends on consistent SQL Server Deep Dive decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying SQL Server Deep Dive as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where SQL Server Deep Dive is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for SQL Server Deep Dive are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

