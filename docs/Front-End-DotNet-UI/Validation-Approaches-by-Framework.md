# Validation Approaches by Framework

> Subject: [Front-End-DotNet-UI](../README.md)

## Validation Approaches by Framework

| Framework   | Primary Mechanism                                      | Typical UI Feedback                       |
| ----------- | ------------------------------------------------------ | ----------------------------------------- |
| MVC         | Data annotations + `ModelState`                        | `asp-validation-for` tag helpers          |
| Razor Pages | Data annotations + `ModelState` in handlers            | `asp-validation-for` tag helpers          |
| Blazor      | `EditForm` + `DataAnnotationsValidator`                | `ValidationSummary` + `ValidationMessage` |
| .NET MAUI   | MVVM validation + `TryParse`/custom errors             | Bound error text + visual states          |
| WPF         | `ValidationRule`/`IDataErrorInfo`                      | Error template + tooltip                  |
| WinForms    | `TryParse` + `ErrorProvider`                           | ErrorProvider icon + message              |
| Web Forms   | `RequiredFieldValidator`, `RegularExpressionValidator` | ValidationSummary + validator text        |

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Validation Approaches by Framework before implementation work begins.
- Keep boundaries explicit so Validation Approaches by Framework decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Validation Approaches by Framework in production-facing code.
- When performance, correctness, or maintainability depends on consistent Validation Approaches by Framework decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Validation Approaches by Framework as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Validation Approaches by Framework is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Validation Approaches by Framework are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

