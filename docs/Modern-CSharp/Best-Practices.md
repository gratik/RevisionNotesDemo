# Best Practices

> Subject: [Modern-CSharp](../README.md)

## Best Practices

### ✅ Records
- Use for immutable DTOs and value objects
- Prefer `record` over `class` for data transfer
- Use `with` expressions for non-destructive updates
- Don't use for entities with identity

### ✅ Pattern Matching
- Use `switch` expressions for multiple branches
- Combine with LINQ for powerful queries
- Use property patterns to eliminate null checks
- Prefer pattern matching over multiple if/else

### ✅ Nullable Reference Types
- Enable `<Nullable>enable</Nullable>` in all projects
- Use `?` to explicitly mark nullable references
- Initialize properties in constructors
- Use `required` for mandatory properties (C# 11+)

### ✅ Init-Only Properties
- Use for immutable objects
- Combine with records for clean syntax
- Prefer over constructor-only initialization

---


