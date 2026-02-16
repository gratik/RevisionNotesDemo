# Performance Checklist

> Subject: [Performance](../README.md)

## Performance Checklist

**Before Optimizing:**
- [ ] Profiled to identify hot paths?
- [ ] Benchmarked current performance?
- [ ] Verified this code actually matters?

**Optimization Targets:**
- [ ] Use Span<T> for slicing/parsing
- [ ] Use stackalloc for small buffers
- [ ] Use ArrayPool for large temp arrays
- [ ] Preallocate collections with capacity
- [ ] Use StringBuilder for string building
- [ ] Use ValueTask for cached results
- [ ] Avoid LINQ in hot paths (allocations)

**After Optimizing:**
- [ ] Benchmarked improvement?
- [ ] Verified no correctness regressions?
- [ ] Code still readable?

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Performance Checklist before implementation work begins.
- Keep boundaries explicit so Performance Checklist decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Performance Checklist in production-facing code.
- When performance, correctness, or maintainability depends on consistent Performance Checklist decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Performance Checklist as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Performance Checklist is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Performance Checklist are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

