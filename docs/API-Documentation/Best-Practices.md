# Best Practices

> Subject: [API-Documentation](../README.md)

## Best Practices

### Documentation

- ✅ Use XML comments for all public endpoints
- ✅ Provide request/response examples
- ✅ Document all possible status codes
- ✅ Include authentication requirements
- ✅ Add meaningful operation IDs

### Versioning

- ✅ Create separate Swagger doc for each version
- ✅ Mark deprecated versions clearly
- ✅ Provide migration guide in description
- ✅ Show version in URL and headers

### Security

- ✅ Never expose Swagger in production (or require auth)
- ✅ Document authentication requirements
- ✅ Hide internal/admin endpoints
- ✅ Sanitize error messages

### Performance

- ✅ Enable Swagger only in Development
- ✅ Cache generated JSON in production (if needed)
- ✅ Use minimal XML comment parsing
- ✅ Consider Swagger alternatives for high-traffic APIs

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

