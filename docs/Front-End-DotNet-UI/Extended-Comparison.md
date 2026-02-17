# Extended Comparison

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Frontend fundamentals and basic .NET web/UI application structure.
- Related examples: docs/Front-End-DotNet-UI/README.md
> Subject: [Front-End-DotNet-UI](../README.md)

## Extended Comparison

| Technology           | Startup/Perf              | Deployment Model              | Testability                  | Offline Support  | Accessibility Defaults       |
| -------------------- | ------------------------- | ----------------------------- | ---------------------------- | ---------------- | ---------------------------- |
| **ASP.NET Core MVC** | Fast server response      | Server app + static assets    | High (controllers, services) | N/A (server web) | Good with proper markup      |
| **Razor Pages**      | Fast server response      | Server app + static assets    | High (page models)           | N/A (server web) | Good with proper markup      |
| **Blazor Server**    | Low client payload        | Server app + SignalR circuit  | High (components + services) | Limited          | Good with component patterns |
| **Blazor WASM**      | Higher initial download   | Static site + API backend     | Medium (UI + integration)    | Possible         | Good with component patterns |
| **.NET MAUI**        | Native startup varies     | App store / MSIX / installers | Medium (UI + VM tests)       | Strong           | Depends on platform tooling  |
| **WPF**              | Strong on Windows         | MSI/MSIX/ClickOnce            | High (VM + UI automation)    | Strong           | Good with WPF automation     |
| **WinForms**         | Very fast on Windows      | MSI/MSIX/ClickOnce            | Medium (UI automation heavy) | Strong           | Depends on control usage     |
| **Web Forms**        | Server-bound + view state | IIS-hosted .NET Framework     | Low (legacy patterns)        | N/A (server web) | Mixed, often manual fixes    |

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Extended Comparison before implementation work begins.
- Keep boundaries explicit so Extended Comparison decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Extended Comparison in production-facing code.
- When performance, correctness, or maintainability depends on consistent Extended Comparison decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Extended Comparison as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Extended Comparison is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Extended Comparison are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Extended Comparison is about .NET UI stack patterns and frontend integration choices. It matters because UI architecture affects usability, testability, and delivery speed.
- Use it when choosing the right .NET UI approach for product constraints.

2-minute answer:
- Start with the problem Extended Comparison solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rapid UI iteration vs maintainable component structure.
- Close with one failure mode and mitigation: tight coupling between UI and data access concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Extended Comparison but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Extended Comparison, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Extended Comparison and map it to one concrete implementation in this module.
- 3 minutes: compare Extended Comparison with an alternative, then walk through one failure mode and mitigation.