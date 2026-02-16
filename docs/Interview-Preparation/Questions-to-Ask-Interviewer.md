# Questions to Ask Interviewer

> Subject: [Interview-Preparation](../README.md)

## Questions to Ask Interviewer

### About the Role

1. What does a typical day look like?
2. What's the team structure?
3. How do you measure success in this role?
4. What are the biggest challenges the team is facing?

### About Technology

1. What's the current tech stack?
2. What's the deployment process?
3. How do you handle technical debt?
4. What's the code review process?

### About Culture

1. How does the team handle disagreements?
2. What's the work-life balance like?
3. How are learning and growth supported?
4. What's the team's approach to remote work?

### About Growth

1. What are the career paths from this role?
2. How often are performance reviews?
3. What does success look like in the first 90 days?
4. Are there mentorship opportunities?

---

## Detailed Guidance

Questions to Ask Interviewer guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Questions to Ask Interviewer before implementation work begins.
- Keep boundaries explicit so Questions to Ask Interviewer decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Questions to Ask Interviewer in production-facing code.
- When performance, correctness, or maintainability depends on consistent Questions to Ask Interviewer decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Questions to Ask Interviewer as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Questions to Ask Interviewer is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Questions to Ask Interviewer are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

