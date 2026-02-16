# Interview Weekly Sprint Planner

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Docs Hub
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/Resilience/PollyRetryPatterns.cs

Use this planner to run structured 7-day interview preparation sprints.

## Sprint template (7 days)

| Day | Focus | Primary asset | Output |
| --- | --- | --- | --- |
| 1 | Fundamentals refresh | Interview-Answer-Blocks.md | 5 concise answers with tradeoffs |
| 2 | Quality calibration | Interview-Bad-vs-Strong-Answers.md | 5 improved answer rewrites |
| 3 | Timed execution | Interview-Timed-Practice-Sets.md | One scored 30-minute round |
| 4 | Role targeting | Interview-Role-Tracks.md + Interview-Role-Mock-Scripts.md | One role-specific mock script run |
| 5 | Design depth | Interview-Whiteboard-Prompts.md | One whiteboard response with rubric score |
| 6 | Scoring and feedback | Interview-Scoring-Sheets.md + Interview-Readiness-Dashboard.md | Updated scores + top 2 weak signals |
| 7 | Final polish | Interview-Cheat-Card.md + Interview-Last-Day-Revision-Sheet.md | Final checklist + confidence notes |

## Sprint goals

- Complete at least 3 timed rounds.
- Improve primary role score by at least 1 point.
- Close at least 2 weak-signal items from dashboard.
- Practice one behavioral-to-technical STAR story.

## End-of-sprint review

1. Which role score improved most?
2. Which weak signal remains highest risk?
3. Which answer type still lacks metrics/tradeoffs?
4. What changes in next sprint plan?

## Detailed Guidance

Interview Weekly Sprint Planner guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Interview Weekly Sprint Planner before implementation work begins.
- Keep boundaries explicit so Interview Weekly Sprint Planner decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Interview Weekly Sprint Planner in production-facing code.
- When performance, correctness, or maintainability depends on consistent Interview Weekly Sprint Planner decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Interview Weekly Sprint Planner as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Interview Weekly Sprint Planner is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Interview Weekly Sprint Planner are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
- 30-second answer: The weekly sprint planner makes interview preparation repeatable with daily focus, measurable outputs, and role alignment.
- 2-minute deep dive: I run a 7-day cycle across answer quality, timed execution, role mocks, scoring, and final polish, then use dashboard trends to plan the next sprint.
- Common follow-up: What if you only have three days?
- Strong response: Collapse the plan to Days 1, 3, and 6 with one high-intensity mock and a focused weak-signal fix.
- Tradeoff callout: Covering too many topics in one sprint lowers quality retention.

## Interview Bad vs Strong Answer

- Bad answer: “I practiced a bit each day.”
- Strong answer: “I followed a 7-day sprint with role-targeted mocks, tracked scores, and closed two weak-signal gaps with measurable improvements.”
- Why strong wins: It shows disciplined execution and objective progress.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Draft your next 7-day sprint using this template for your target role.
- Required outputs:
  - one daily plan
  - one measurable goal
  - one risk and mitigation for the sprint
- Self-check score (0-3 each): realism, role fit, measurable planning.


