# Best Practices

> Subject: [OOP-Principles](../README.md)

## Best Practices

### ✅ Applying SOLID
- **SRP**: Each class should do one thing well
- **OCP**: Design for extension (strategy pattern, inheritance)
- **LSP**: Ensure derived classes don't break base class behavior
- **ISP**: Keep interfaces small and focused
- **DIP**: Always inject dependencies via constructor

### ✅ When to Apply
- Use SOLID for **core domain logic** (services, repositories)
- Don't overengineer simple **DTOs or data classes**
- Apply principles when code becomes **hard to test or change**
- Refactor toward SOLID as complexity grows

### ✅ Balance Pragmatism
- SOLID is a guide, not a law
- Start simple, refactor when needed (YAGNI)
- Avoid over-abstraction
- Consider team size and project lifetime

---


