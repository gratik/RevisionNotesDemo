# Architecture Decisions - RevisionNotes.DataAccess.AdvancedEfCore

## WHAT IS THIS?
This is an EF Core-focused API demo showing performance and correctness patterns for production data access.

## WHY IT MATTERS?
- Data access is a common source of performance regressions and subtle correctness bugs.
- This demo makes query strategy and concurrency controls explicit.

## WHEN TO USE?
- When your API relies heavily on EF Core and needs predictable query behavior.
- When you need optimistic concurrency and soft-delete semantics.

## WHEN NOT TO USE?
- When persistence is trivial and advanced EF patterns add unnecessary complexity.
- When raw SQL or micro-ORMs are a better fit for workload characteristics.

## REAL-WORLD EXAMPLE?
An e-commerce catalog API that needs fast filtered reads, non-destructive deletes, and conflict-safe updates from multiple admins.

## ADR-01: Compiled query for hot path reads
- Decision: Use `EF.CompileAsyncQuery` for repeated filtered reads.
- Why: Reduces query compilation overhead under load.
- Tradeoff: Query shape becomes less flexible than ad-hoc LINQ composition.

## ADR-02: Split query for related aggregate loading
- Decision: Use `AsSplitQuery()` when loading product with tags.
- Why: Avoids large Cartesian expansions in single SQL result sets.
- Tradeoff: Executes multiple queries, which may increase round trips.

## ADR-03: Soft delete + optimistic concurrency
- Decision: Hide deleted rows with global filter and update using version checks.
- Why: Preserves auditability and prevents lost updates.
- Tradeoff: API clients must handle concurrency conflict responses.
