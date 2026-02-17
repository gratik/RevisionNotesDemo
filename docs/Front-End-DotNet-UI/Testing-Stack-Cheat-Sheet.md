# Testing Stack Cheat-Sheet

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Front-End-DotNet-UI](../README.md)

## Testing Stack Cheat-Sheet

| Framework   | Unit Tests                       | UI/Integration Tests                    |
| ----------- | -------------------------------- | --------------------------------------- |
| MVC         | xUnit + controller/service tests | Playwright (web) or Selenium            |
| Razor Pages | xUnit + PageModel tests          | Playwright (web) or Selenium            |
| Blazor      | bUnit for components + xUnit     | Playwright (Web) / MAUI UITest (Hybrid) |
| .NET MAUI   | xUnit + VM tests                 | Appium or MAUI UITest (if available)    |
| WPF         | xUnit + VM tests                 | FlaUI or WinAppDriver                   |
| WinForms    | xUnit + presenter tests          | FlaUI or WinAppDriver                   |
| Web Forms   | xUnit + utility tests            | Selenium (legacy web UI)                |

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Testing Stack Cheat-Sheet before implementation work begins.
- Keep boundaries explicit so Testing Stack Cheat-Sheet decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Testing Stack Cheat-Sheet in production-facing code.
- When performance, correctness, or maintainability depends on consistent Testing Stack Cheat-Sheet decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Testing Stack Cheat-Sheet as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Testing Stack Cheat-Sheet is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Testing Stack Cheat-Sheet are documented and reviewable.
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

