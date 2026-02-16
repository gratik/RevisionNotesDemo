# Azure Storage Strategy

> Subject: [Azure-Hosting](../README.md)

## Azure Storage Strategy

| Data Pattern | Azure Service |
| --- | --- |
| Object/file storage | Blob Storage |
| Relational transactions | Azure SQL |
| Globally distributed JSON | Cosmos DB |
| Cache/session state | Azure Cache for Redis |
| Async decoupling | Queue Storage / Service Bus |

### Storage Design Principles

- Select storage by access pattern, not team preference
- Configure retention/lifecycle to control cost
- Test restore workflows regularly
- Use geo-redundancy based on business RTO/RPO

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Azure Storage Strategy before implementation work begins.
- Keep boundaries explicit so Azure Storage Strategy decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Azure Storage Strategy in production-facing code.
- When performance, correctness, or maintainability depends on consistent Azure Storage Strategy decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Azure Storage Strategy as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Azure Storage Strategy is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Azure Storage Strategy are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

