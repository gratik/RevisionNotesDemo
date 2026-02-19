# Interview Cheat Card

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Preparation Guide
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/Resilience/PollyRetryPatterns.cs

Use this for a fast 5-10 minute pre-interview refresh.

## Core concept quick map

| Topic | One-line anchor | Red flag to avoid | Strong signal to include |
| --- | --- | --- | --- |
| SOLID/OOP | Design for safe change with clear responsibility and dependency direction. | “I always use all patterns.” | One concrete tradeoff and where SRP/DIP improved maintainability. |
| Async | Async for I/O throughput; CPU work needs separate strategy. | “Async makes everything faster.” | Cancellation, timeout handling, and thread-pool awareness. |
| Data access | Choose EF/Dapper/ADO.NET by workload and control needs. | “EF is always too slow.” | N+1 prevention, projection, and measured query performance. |
| API design | Contracts, validation, idempotency, and consistent errors matter. | “REST is just CRUD endpoints.” | Problem details, versioning stance, and retry-safe operations. |
| Security | Layered security: authN, authZ, secrets, data protection, auditability. | “JWT solved security.” | Authorization boundaries and secret rotation discipline. |
| Resilience | Bound failures with timeouts, retries, breakers, and fallbacks. | “Retries fix failures.” | Retry caps + jitter + breaker + rollback policy. |
| Consistency | Outbox/inbox and idempotency prevent duplicate side effects. | “Use distributed transactions everywhere.” | Replay-safe handlers and compensation strategy. |
| Observability | Logs/metrics/traces should answer failure and latency questions quickly. | “We have logs.” | Correlated telemetry with p95/error/queue lag visibility. |

## Metrics cheat card

| Area | Metric | Why it matters |
| --- | --- | --- |
| API latency | p95/p99 | User experience and SLA adherence |
| Reliability | Error rate | Service stability and incident severity |
| Messaging | Queue lag | Backlog health and throughput pressure |
| Deployment | Rollback trigger threshold | Controlled blast radius during rollout |
| Resource usage | CPU/memory saturation | Capacity planning and bottleneck detection |

## 90-second answer template

1. Context and constraint
2. Decision and alternative
3. Tradeoff and risk
4. Mitigation and metric
5. Outcome and lesson

## Detailed Guidance

Interview Cheat Card guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Interview Cheat Card before implementation work begins.
- Keep boundaries explicit so Interview Cheat Card decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Interview Cheat Card in production-facing code.
- When performance, correctness, or maintainability depends on consistent Interview Cheat Card decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Interview Cheat Card as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Interview Cheat Card is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Interview Cheat Card are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
- 30-second answer: This cheat card is a fast recall layer for high-frequency interview topics and measurable signals.
- 2-minute deep dive: I use it to structure concise answers around constraints, decisions, tradeoffs, and metrics so responses stay concrete.
- Common follow-up: What if the interviewer asks for deeper implementation detail?
- Strong response: Expand one row into an end-to-end example with failure mode, mitigation, and measurable outcome.
- Tradeoff callout: Memorizing phrases without contextual understanding leads to shallow answers.

## Interview Bad vs Strong Answer

- Bad answer: “I’m familiar with all these topics.”
- Strong answer: “For each topic I can state the decision boundary, a common failure mode, and the metric I monitor to validate behavior.”
- Why strong wins: It demonstrates practical depth and operational thinking.

## Interview Timed Drill

- Time box: 8 minutes.
- Prompt: Pick three rows from the core map and give one concise example for each.
- Required outputs:
  - One tradeoff per row
  - One risk/mitigation per row
  - One metric per row
- Self-check score (0-3 each): precision, tradeoff quality, measurable thinking.


