# Quick Comparison

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Front-End-DotNet-UI](../README.md)

## Quick Comparison

| Technology           | Platforms                     | Rendering Model                | Best For                            | Tradeoffs                                |
| -------------------- | ----------------------------- | ------------------------------ | ----------------------------------- | ---------------------------------------- |
| **ASP.NET Core MVC** | Web                           | Server-rendered HTML           | SEO-friendly sites, complex routing | More boilerplate than Razor Pages        |
| **Razor Pages**      | Web                           | Server-rendered HTML           | Page-focused sites, CRUD            | Less flexible for complex routing        |
| **Blazor**           | Web, Desktop, Mobile (Hybrid) | Components (SSR, Server, WASM) | Interactive UIs in C#               | Payload size, interop complexity         |
| **.NET MAUI**        | Windows, macOS, iOS, Android  | Native UI via XAML             | Cross-platform apps                 | Platform-specific tuning                 |
| **WPF**              | Windows                       | XAML, data binding             | Rich Windows desktop apps           | Windows-only                             |
| **WinForms**         | Windows                       | Event-driven, designer         | Rapid desktop apps                  | UI scaling, legacy patterns              |
| **Web Forms**        | Web (.NET Framework)          | Server controls + view state   | Legacy apps                         | View state bloat, limited modern tooling |

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Quick Comparison before implementation work begins.
- Keep boundaries explicit so Quick Comparison decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Quick Comparison in production-facing code.
- When performance, correctness, or maintainability depends on consistent Quick Comparison decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Quick Comparison as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Quick Comparison is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Quick Comparison are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

