# Behavioral Questions

> Subject: [Interview-Preparation](../README.md)

## Behavioral Questions

### STAR Method Framework

**S**ituation: Context and background
**T**ask: Challenge or responsibility
**A**ction: Steps you took
**R**esult: Outcome and learnings

---

### Common Behavioral Questions

**Q: Tell me about a time you faced a difficult bug**

**Example Answer**:

- **Situation**: Production issue - API response time increased from 200ms to 5s
- **Task**: Identify root cause and fix without downtime
- **Action**:
  1. Checked monitoring dashboards (APM)
  2. Found N+1 query problem in EF Core
  3. Added `.Include()` for eager loading
  4. Deployed fix with feature flag
- **Result**: Response time back to 200ms, learned importance of query analysis

---

**Q: Describe a time you had to learn something quickly**

**Example Answer**:

- **Situation**: Project required SignalR for real-time features, had 1 week
- **Task**: Learn SignalR and implement chat feature
- **Action**:
  1. Read official docs
  2. Built prototype
  3. Code review with senior dev
  4. Implemented production feature
- **Result**: Delivered on time, became team's SignalR expert

---

**Q: Tell me about a time you disagreed with a team member**

**Example Answer**:

- **Situation**: Teammate wanted to use Repository pattern for all entities
- **Task**: Discuss trade-offs and reach consensus
- **Action**:
  1. Presented research on when Repository adds value
  2. Showed EF Core DbSet already provides repository functionality
  3. Proposed using it only for complex domain logic
- **Result**: Team agreed, reduced unnecessary abstraction

---

## Detailed Guidance

Behavioral Questions guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Behavioral Questions before implementation work begins.
- Keep boundaries explicit so Behavioral Questions decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Behavioral Questions in production-facing code.
- When performance, correctness, or maintainability depends on consistent Behavioral Questions decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Behavioral Questions as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Behavioral Questions is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Behavioral Questions are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

