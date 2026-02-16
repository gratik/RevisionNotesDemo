# Docker Hosting Patterns

> Subject: [Azure-Hosting](../README.md)

## Docker Hosting Patterns

### Recommended Flow

1. Build immutable Docker image in CI
2. Scan image for vulnerabilities
3. Push to Azure Container Registry
4. Deploy by versioned tag/digest
5. Validate health and shift traffic

### Key Practices

- Use multi-stage Dockerfiles for small runtime images
- Run containers with non-root users where possible
- Store config/secrets in Key Vault and inject at runtime
- Pin base images and apply patching cadence

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Docker Hosting Patterns before implementation work begins.
- Keep boundaries explicit so Docker Hosting Patterns decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Docker Hosting Patterns in production-facing code.
- When performance, correctness, or maintainability depends on consistent Docker Hosting Patterns decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Docker Hosting Patterns as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Docker Hosting Patterns is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Docker Hosting Patterns are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

