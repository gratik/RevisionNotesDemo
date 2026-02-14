# LINQ and Query Patterns

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

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

- [LINQAndQueries/LINQExamples.cs](../LINQAndQueries/LINQExamples.cs)
- [LINQAndQueries/IQueryableVsIEnumerable.cs](../LINQAndQueries/IQueryableVsIEnumerable.cs)

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

- [LINQExamples.cs](../LINQAndQueries/LINQExamples.cs)
- [IQueryableVsIEnumerable.cs](../LINQAndQueries/IQueryableVsIEnumerable.cs)

## See Also

- [Entity Framework](Entity-Framework.md)
- [Core C# Features](Core-CSharp.md)
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
