# Minimal API vs Controller API vs MVC

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET Core request pipeline and routing fundamentals.
- Related examples: docs/Web-API-MVC/README.md
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

## Interview Answer Block
30-second answer:
- Minimal API vs Controller API vs MVC is about ASP.NET endpoint architecture patterns. It matters because architecture choices affect testability, throughput, and maintainability.
- Use it when selecting minimal API, controller API, or MVC by problem shape.

2-minute answer:
- Start with the problem Minimal API vs Controller API vs MVC solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer speed vs explicit control and extensibility.
- Close with one failure mode and mitigation: mixing styles without clear boundaries.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Minimal API vs Controller API vs MVC but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Minimal API vs Controller API vs MVC, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Minimal API vs Controller API vs MVC and map it to one concrete implementation in this module.
- 3 minutes: compare Minimal API vs Controller API vs MVC with an alternative, then walk through one failure mode and mitigation.