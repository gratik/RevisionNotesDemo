# Choosing the Right Tool

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: SQL fundamentals, connection lifecycle, and transaction basics.
- Related examples: docs/Data-Access/README.md
> Subject: [Data-Access](../README.md)

## Choosing the Right Tool

### Quick Comparison

| Feature               | ADO.NET                | Dapper                    | EF Core                    |
| --------------------- | ---------------------- | ------------------------- | -------------------------- |
| **Performance**       | Fastest (baseline)     | Very Fast (90-95% of ADO) | Slower (tracking overhead) |
| **Learning Curve**    | Steepest               | Moderate                  | Easiest                    |
| **SQL Control**       | Full                   | Full                      | Limited (LINQ translated)  |
| **Object Mapping**    | Manual                 | Automatic                 | Automatic + Navigation     |
| **Change Tracking**   | None                   | None                      | Built-in                   |
| **Migrations**        | Manual                 | Manual                    | Automatic                  |
| **Best For**          | Bulk ops, stored procs | Micro-services, APIs      | Complex domains, CRUD apps |
| **Lines of Code**     | Most                   | Moderate                  | Least                      |
| **Query Flexibility** | Complete               | Complete                  | Limited by LINQ            |

### Performance Characteristics (10,000 rows)

- **ADO.NET**: ~50ms (baseline, manual mapping)
- **Dapper**: ~55ms (10% overhead for automatic mapping)
- **EF Core Tracked**: ~450ms (9x slower, change tracking + complex SQL)
- **EF Core NoTracking**: ~150ms (3x slower, still generates joins)

### Decision Tree

```
Need complex relationships + automatic migrations? → EF Core
    ↓ No
Performance critical (< 100ms SLA)? → Dapper or ADO.NET
    ↓ Bulk operations (10k+ rows)?
    Yes → ADO.NET (SqlBulkCopy)
    No → Dapper (cleaner code, good performance)
```

---

## Detailed Guidance

Choosing the Right Tool guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Choosing the Right Tool before implementation work begins.
- Keep boundaries explicit so Choosing the Right Tool decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Choosing the Right Tool in production-facing code.
- When performance, correctness, or maintainability depends on consistent Choosing the Right Tool decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Choosing the Right Tool as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Choosing the Right Tool is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Choosing the Right Tool are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Choosing the Right Tool is about data persistence and query strategy selection. It matters because it drives correctness, latency, and transactional integrity.
- Use it when choosing between EF Core, Dapper, and ADO.NET by workload.

2-minute answer:
- Start with the problem Choosing the Right Tool solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: abstraction convenience vs explicit SQL control.
- Close with one failure mode and mitigation: ignoring transaction and indexing behavior in production paths.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Choosing the Right Tool but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Choosing the Right Tool, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Choosing the Right Tool and map it to one concrete implementation in this module.
- 3 minutes: compare Choosing the Right Tool with an alternative, then walk through one failure mode and mitigation.