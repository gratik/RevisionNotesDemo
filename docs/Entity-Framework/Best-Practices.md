# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

## Best Practices

### ✅ Query Performance
- Use `AsNoTracking()` for read-only queries
- Use `Include()` to prevent N+1 queries
- Use projection (Select) to load only needed fields
- Avoid lazy loading in most scenarios
- Use compiled queries for hot paths

### ✅ DbContext Usage
- Register as Scoped (default in ASP.NET Core)
- Don't cache entities across requests
- Dispose DbContext properly (automatic with DI)
- Don't create DbContext manually in production
- One SaveChanges per unit of work

### ✅ Relationships
- Always configure relationships explicitly
- Use eager loading (`Include`) when you need related data
- Use explicit loading when conditionally needed
- Consider projection to avoid loading entire graphs

### ✅ Migrations
- Create migrations with descriptive names
- Review generated migrations before applying
- Test migrations on staging before production
- Keep migrations in source control
- Use `dotnet ef migrations script` for production

---

## Detailed Guidance

Best Practices guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Best Practices is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.