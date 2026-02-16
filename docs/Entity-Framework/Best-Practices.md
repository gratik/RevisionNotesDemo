# Best Practices

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


