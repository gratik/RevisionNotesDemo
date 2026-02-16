# Choosing the Right Tool

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


