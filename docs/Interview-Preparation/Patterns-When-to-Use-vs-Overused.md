# Patterns: When to Use vs Overused

> Subject: [Interview-Preparation](../README.md)

## Patterns: When to Use vs Overused

### Singleton Pattern

**Overused because**: Hidden global state, hard to test, lifetime coupling

**Prefer instead**: Use DI with `AddSingleton` and pass dependencies explicitly

**Still valid when**: Truly global infrastructure with stable lifecycle (logging, configuration)

---

### Abstract Factory Pattern

**Overused because**: Too many layers for simple object creation

**Prefer instead**: Use DI, configuration, or simple factory methods

**Still valid when**: Multiple product families must stay consistent

---

### Service Locator Pattern

**Overused because**: Hidden dependencies, runtime failures

**Prefer instead**: Constructor injection with explicit dependencies

**Still valid when**: Legacy frameworks where DI is not possible

---

### Repository Pattern (for every entity)

**Overused because**: Extra abstraction when EF Core DbSet already acts as repository

**Prefer instead**: Use DbContext directly with query-focused services

**Still valid when**: Complex domain rules or multiple data sources

---


