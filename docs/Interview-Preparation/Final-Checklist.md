# Final Checklist

> Subject: [Interview-Preparation](../README.md)

## Final Checklist

### Week Before Interview

- [ ] Review common questions in this guide
- [ ] Practice 5-10 coding challenges
- [ ] Review your projects and be ready to discuss
- [ ] Research the company thoroughly
- [ ] Prepare questions to ask

### Day Before Interview

- [ ] Review quick reference tables
- [ ] Practice introducing yourself
- [ ] Review STAR method examples
- [ ] Prepare your workspace (if remote)
- [ ] Test camera/mic (if remote)

### Interview Day

- [ ] Arrive/login 10 minutes early
- [ ] Have water nearby
- [ ] Have pen and paper for notes
- [ ] Turn off notifications
- [ ] Relax and be yourself!

---

## Detailed Guidance

Final Checklist guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Final Checklist before implementation work begins.
- Keep boundaries explicit so Final Checklist decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Final Checklist in production-facing code.
- When performance, correctness, or maintainability depends on consistent Final Checklist decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Final Checklist as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Final Checklist is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Final Checklist are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

