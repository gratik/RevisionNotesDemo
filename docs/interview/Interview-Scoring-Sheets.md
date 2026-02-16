# Interview Scoring Sheets

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Role Mock Scripts
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/Observability/OpenTelemetrySetup.cs

Use these templates during mock interviews to capture objective scoring and actionable feedback.

## Universal scorecard (all roles)

| Dimension | Score (0-3) | Notes |
| --- | --- | --- |
| Technical correctness |  |  |
| Tradeoff quality |  |  |
| Failure/recovery reasoning |  |  |
| Security/operability awareness |  |  |
| Communication clarity |  |  |

Total score (max 15): `____`

## Backend .NET scorecard

- Scenario: API + data access + validation flow
- Required signals:
  - clear contract boundary
  - idempotency/failure-path handling
  - data-access tradeoff justification

| Checkpoint | Pass/Fail | Notes |
| --- | --- | --- |
| API contract and validation clarity |  |  |
| Data strategy (EF/Dapper rationale) |  |  |
| Reliability path (retry/timeouts/idempotency) |  |  |

## Senior .NET scorecard

- Scenario: architecture + performance/resilience decision quality
- Required signals:
  - metric-backed incident explanation
  - design evolution after failure
  - governance and quality standards

| Checkpoint | Pass/Fail | Notes |
| --- | --- | --- |
| Architecture decision tradeoff depth |  |  |
| Performance/reliability metrics used correctly |  |  |
| Prevention loop (runbook/gates/alerts) |  |  |

## Platform / DevOps scorecard

- Scenario: CI/CD, rollback strategy, and observability operations
- Required signals:
  - concrete gate set
  - rollback trigger thresholds
  - incident triage clarity

| Checkpoint | Pass/Fail | Notes |
| --- | --- | --- |
| Pipeline gate quality |  |  |
| Rollout/rollback design |  |  |
| Operability and incident response depth |  |  |

## Solutions Architect scorecard

- Scenario: whiteboard system design with consistency and ownership
- Required signals:
  - clear bounded contexts
  - consistency/recovery strategy
  - cross-team governance approach

| Checkpoint | Pass/Fail | Notes |
| --- | --- | --- |
| Boundary and contract ownership clarity |  |  |
| Consistency and replay/recovery realism |  |  |
| Scalability and governance strategy |  |  |

## Feedback template

- Strongest signal shown:
- Highest-priority gap:
- One concrete improvement for next round:
- One measurable target for next round:

## Interview Answer Block

- 30-second answer: Scoring sheets keep mock interviews objective by tracking specific technical signals instead of vague impressions.
- 2-minute deep dive: I score by role-specific checkpoints, identify one priority gap, and set one measurable target before the next mock round.
- Common follow-up: How do you avoid score inflation?
- Strong response: Use strict rubric anchors and require evidence examples for high scores.
- Tradeoff callout: Over-scoring form over substance can hide real technical weakness.

## Interview Bad vs Strong Answer

- Bad answer: “The mock interview felt good overall.”
- Strong answer: “I scored 10/15, missed failure-path depth, and next round I’ll include rollback criteria with p95 and error-rate thresholds.”
- Why strong wins: It turns feedback into specific, measurable improvement steps.

## Interview Timed Drill

- Time box: 12 minutes.
- Prompt: Run one role mock question and fill the matching scorecard in real time.
- Required outputs:
  - completed score table
  - one priority gap
  - one measurable next-step target
- Self-check score (0-3 each): rubric discipline, specificity, improvement quality.
