# Migrations

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

## Migrations

### Creating Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Migration Best Practices

```csharp
// ✅ GOOD: Descriptive migration names
dotnet ef migrations add AddUserEmailIndex
dotnet ef migrations add AddOrderStatusColumn

// ✅ Always review generated migration
public partial class AddUserEmailIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");
    }
}
```

---

## Detailed Guidance

Migrations guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Migrations before implementation work begins.
- Keep boundaries explicit so Migrations decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Migrations in production-facing code.
- When performance, correctness, or maintainability depends on consistent Migrations decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Migrations as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Migrations is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Migrations are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Migrations is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Migrations solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Migrations but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Migrations, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Migrations and map it to one concrete implementation in this module.
- 3 minutes: compare Migrations with an alternative, then walk through one failure mode and mitigation.