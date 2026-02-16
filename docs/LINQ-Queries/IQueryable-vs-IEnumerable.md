# IQueryable vs IEnumerable

> Subject: [LINQ-Queries](../README.md)

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


