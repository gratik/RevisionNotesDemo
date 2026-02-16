# Best Practices

> Subject: [DotNet-Concepts](../README.md)

## Best Practices

### ✅ Lifetime Selection
- **Transient**: Stateless, lightweight (most services)
- **Scoped**: DbContext, HttpContext, per-request state
- **Singleton**: Caching, configuration, expensive initialization

### ✅ Dependency Guidelines
- Prefer constructor injection over property/method injection
- Inject interfaces, not concrete classes
- Keep constructors simple (no logic)
- Don't inject IServiceProvider (service locator anti-pattern)
- Watch for captive dependencies (scoped in singleton)

### ✅ Registration
- Register services in Program.cs
- Use extension methods to organize (AddMyServices())
- Validate on startup in development
- Document required services

---


