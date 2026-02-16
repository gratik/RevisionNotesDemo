# Best Practices

> Subject: [Core-CSharp](../README.md)

## Best Practices

### ✅ Generics
- Use constraints to make APIs type-safe
- Prefer `List<T>` over `ArrayList` (always)
- Use `IEnumerable<T>` for read-only sequences
- Avoid over-constraining (only add constraints you need)

### ✅ Delegates and Events
- Use `EventHandler<TEventArgs>` for events
- Use `Func<T>` and `Action<T>` instead of custom delegates
- Always check for null before invoking events (`?.Invoke`)
- Unsubscribe from events to prevent memory leaks

### ✅ Extension Methods
- Put in static classes with clear names (`StringExtensions`)
- Don't overuse (prefer instance methods when you own the class)
- Use for "utility" methods on types you don't control
- Make them discoverable (good naming)

### ✅ Interfaces
- Prefer composition over inheritance
- Keep interfaces small (ISP)
- Use interfaces for dependency injection
- Name interfaces descriptively (`IRepository`, not `IRepo`)

---


