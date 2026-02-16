# Quick Reference

> Subject: [LINQ-Queries](../README.md)

## Quick Reference

```csharp
var active = users
    .Where(u => u.IsActive)
    .Select(u => new { u.Id, u.Name });
```


