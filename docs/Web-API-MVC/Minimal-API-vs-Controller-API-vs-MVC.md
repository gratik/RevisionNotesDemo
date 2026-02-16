# Minimal API vs Controller API vs MVC

> Subject: [Web-API-MVC](../README.md)

## Minimal API vs Controller API vs MVC

| Feature | Minimal API | Controller API | MVC |
|---------|-------------|----------------|-----|
| **Introduced** | .NET 6 | .NET Core 1.0 | .NET Core 1.0 |
| **Style** | Functional | OOP | OOP + Views |
| **Boilerplate** | Minimal | Moderate | More |
| **Best For** | Simple APIs, microservices | Large APIs, complex logic | Server-rendered apps |
| **Filters** | Limited | ✅ Action/Resource filters | ✅ Full filter pipeline |
| **Routing** | Convention-based | Attribute-based | Convention/Attribute |
| **DI** | Parameter injection | Constructor injection | Constructor injection |
| **Testing** | Harder | Easier | Easier |

### When to Use Each

**Minimal API**: Small APIs, microservices, quick prototypes
**Controller API**: Large REST APIs, complex validation, reusable filters
**MVC**: Server-rendered applications with views (Razor Pages often better)

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Minimal API vs Controller API vs MVC before implementation work begins.
- Keep boundaries explicit so Minimal API vs Controller API vs MVC decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Minimal API vs Controller API vs MVC in production-facing code.
- When performance, correctness, or maintainability depends on consistent Minimal API vs Controller API vs MVC decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Minimal API vs Controller API vs MVC as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Minimal API vs Controller API vs MVC is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Minimal API vs Controller API vs MVC are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

