# Authentication Methods Comparison

> Subject: [Security](../README.md)

## Authentication Methods Comparison

| Method | Use Case | Stateful? | Cross-Domain | Mobile-Friendly |
|--------|----------|-----------|--------------|------------------|
| **Cookie** | Traditional web apps | Yes (session) | ❌ No | ❌ Limited |
| **JWT** | APIs, SPAs, mobile | No (stateless) | ✅ Yes | ✅ Yes |
| **OAuth 2.0** | Third-party login | Depends | ✅ Yes | ✅ Yes |
| **API Key** | Service-to-service | No | ✅ Yes | ✅ Yes |
| **Certificate** | Enterprise/devices | No | ✅ Yes | ⚠️ Complex |

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Authentication Methods Comparison before implementation work begins.
- Keep boundaries explicit so Authentication Methods Comparison decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Authentication Methods Comparison in production-facing code.
- When performance, correctness, or maintainability depends on consistent Authentication Methods Comparison decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Authentication Methods Comparison as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Authentication Methods Comparison is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Authentication Methods Comparison are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

