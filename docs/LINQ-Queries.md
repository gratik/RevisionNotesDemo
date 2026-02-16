# LINQ and Query Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Collections and lambda expressions
- Related examples: Learning/LINQAndQueries/LINQExamples.cs, Learning/LINQAndQueries/IQueryableVsIEnumerable.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#
- **When to Study**: Before Data Access and Entity Framework modules.
- **Related Files**: `../Learning/LINQAndQueries/*.cs`
- **Estimated Time**: 60-90 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](OOP-Principles.md)
- **Next Step**: [Memory-Management.md](Memory-Management.md)
<!-- STUDY-NAV-END -->


## Overview

LINQ (Language Integrated Query) provides a unified syntax for querying collections, databases, XML, and more.
This guide focuses on IQueryable vs IEnumerable, deferred execution, query operators, and performance implications.
Understanding when queries execute is critical for avoiding performance pitfalls.

---

## IQueryable vs IEnumerable

### The Critical Difference

**IEnumerable<T>**: In-memory execution (LINQ-to-Objects)
- Executes in your application (client-side)
- Uses `Func<T, bool>` (compiled delegates)
- Best for: In-memory collections (List, Array)

**IQueryable<T>**: Remote execution (LINQ-to-SQL, LINQ-to-EF)
- Executes on database server (server-side)
- Uses `Expression<Func<T, bool>>` (expression trees)
- Translates to SQL
- Best for: Database queries

### Quick Example

```csharp
// ❌ BAD: Loads entire table into memory!
IEnumerable<Customer> customers = _dbContext.Customers.ToList();  // ❌ Loads ALL
var active = customers.Where(c => c.IsActive).Take(10);
// SQL: SELECT * FROM Customers (loads everything!)

// ✅ GOOD: Filters in database
IQueryable<Customer> customers = _dbContext.Customers;
var active = customers.Where(c => c.IsActive).Take(10).ToList();
// SQL: SELECT TOP 10 * FROM Customers WHERE IsActive = 1
```

---

## Related Files

- [LINQAndQueries/LINQExamples.cs](../Learning/LINQAndQueries/LINQExamples.cs)
- [LINQAndQueries/IQueryableVsIEnumerable.cs](../Learning/LINQAndQueries/IQueryableVsIEnumerable.cs)

## Key Concepts

- Deferred execution and query composition
- IQueryable for remote providers (EF Core) vs IEnumerable in-memory
- Projections to avoid over-fetching

## Quick Reference

```csharp
var active = users
    .Where(u => u.IsActive)
    .Select(u => new { u.Id, u.Name });
```

## Examples

- [LINQExamples.cs](../Learning/LINQAndQueries/LINQExamples.cs)
- [IQueryableVsIEnumerable.cs](../Learning/LINQAndQueries/IQueryableVsIEnumerable.cs)

## See Also

- [Entity Framework](Entity-Framework.md)
- [Core C# Features](Core-CSharp.md)
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Memory-Management.md](Memory-Management.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers LINQ Queries and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know LINQ Queries and I would just follow best practices."
- Strong answer: "For LINQ Queries, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply LINQ Queries in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
