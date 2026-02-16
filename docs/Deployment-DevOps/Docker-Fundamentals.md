# Docker Fundamentals

> Subject: [Deployment-DevOps](../README.md)

## Docker Fundamentals

### What is Docker?

**Container** = Lightweight, standalone package with:

- Application code
- Runtime (e.g., .NET SDK)
- System tools and libraries
- Dependencies

**Benefits**:

- ✅ Same environment everywhere (dev, test, prod)
- ✅ Isolated processes
- ✅ Fast startup (vs VMs)
- ✅ Easy scaling
- ✅ Efficient resource usage

### Docker vs Virtual Machine

| Aspect          | Docker Container | Virtual Machine |
| --------------- | ---------------- | --------------- |
| **Size**        | MBs              | GBs             |
| **Startup**     | Seconds          | Minutes         |
| **Isolation**   | Process-level    | Full OS         |
| **Performance** | Near-native      | Overhead        |
| **Density**     | Many per host    | Few per host    |

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Docker Fundamentals before implementation work begins.
- Keep boundaries explicit so Docker Fundamentals decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Docker Fundamentals in production-facing code.
- When performance, correctness, or maintainability depends on consistent Docker Fundamentals decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Docker Fundamentals as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Docker Fundamentals is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Docker Fundamentals are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

