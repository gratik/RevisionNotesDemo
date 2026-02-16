# Azure Hosting Decision Map

> Subject: [Azure-Hosting](../README.md)

## Azure Hosting Decision Map

| Workload Type | Best First Choice | Why |
| --- | --- | --- |
| Simple web API in container | App Service for Containers | Fastest setup, managed platform |
| Event-driven bursty processing | Azure Functions (optionally Dockerized) | Auto-scale and pay-per-use |
| Microservices with moderate complexity | Azure Container Apps | Revisions, scaling, lower ops overhead |
| Complex platform with custom networking | AKS | Full Kubernetes control |

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Azure Hosting Decision Map before implementation work begins.
- Keep boundaries explicit so Azure Hosting Decision Map decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Azure Hosting Decision Map in production-facing code.
- When performance, correctness, or maintainability depends on consistent Azure Hosting Decision Map decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Azure Hosting Decision Map as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Azure Hosting Decision Map is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Azure Hosting Decision Map are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

