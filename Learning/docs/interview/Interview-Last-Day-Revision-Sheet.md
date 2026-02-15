# Interview Last-Day Revision Sheet

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Preparation Guide
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs

Use this as a final high-signal pass the day before interviews.

## Must-remember technical anchors

- OOP/SOLID: explain one principle with a production example and one tradeoff.
- Async: clarify I/O-bound vs CPU-bound and cancellation propagation.
- Data access: justify EF vs Dapper with measurable context.
- API design: contracts, validation, error semantics, idempotency.
- Security: authN + authZ + secrets + observability.
- Resilience: timeout budgets, jittered retries, breakers, rollback.
- Consistency: outbox/inbox, idempotency keys, replay safety.

## Metrics you should be ready to quote

- p95/p99 latency
- error rate and saturation signal
- queue lag / consumer retry count
- deployment rollback trigger threshold

## One-minute architecture checklist

1. Define boundaries and ownership.
2. Define contracts and versioning stance.
3. Define failure path and recovery path.
4. Define rollout and rollback strategy.
5. Define observability and acceptance metrics.

## Red-flag answer patterns to avoid

- “We used best practices everywhere.”
- “We scaled horizontally” (without bottleneck or boundary detail).
- “Retries solved it” (without timeout/breaker constraints).
- “JWT handled security” (without authorization and secrets).

## Last-day prep sequence (60-90 min)

1. 20 min: Review `Interview-Answer-Blocks.md`.
2. 15 min: Review `Interview-Bad-vs-Strong-Answers.md`.
3. 15 min: Run one set from `Interview-Timed-Practice-Sets.md`.
4. 10 min: Do one prompt from `Interview-Whiteboard-Prompts.md`.
5. 5-10 min: Note three weak spots and one improvement each.

## Interview Answer Block

- 30-second answer: The last-day sheet is a high-signal filter for concepts, metrics, and tradeoffs that interviewers test most often.
- 2-minute deep dive: I use this sheet to rehearse concise technical narratives, measurable outcomes, and failure/recovery reasoning under time pressure.
- Common follow-up: What if you forget details mid-interview?
- Strong response: Return to constraints, pick one concrete example, and give one metric-backed decision.
- Tradeoff callout: Overloading final-day prep with new topics reduces retention and confidence.

## Interview Bad vs Strong Answer

- Bad answer: “I revised everything and feel ready.”
- Strong answer: “I validated readiness on core anchors, rehearsed timed drills, and prepared metric-backed examples for architecture, reliability, and security.”
- Why strong wins: It demonstrates structured preparation and interview-relevant evidence.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Pick one anchor topic and deliver a 90-second answer plus one follow-up answer with a metric.
- Required outputs:
  - One concise core answer
  - One tradeoff statement
  - One measurable proof point
- Self-check score (0-3 each): clarity, technical depth, measurable justification.
