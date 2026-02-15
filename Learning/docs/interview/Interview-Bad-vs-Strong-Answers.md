# Interview Bad vs Strong Answers

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Preparation Guide
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/Resilience/PollyRetryPatterns.cs

Use this page to calibrate answer quality under pressure.

## SOLID and Architecture

- Bad answer: "I always use SOLID and clean architecture everywhere."
- Strong answer: "I apply SOLID selectively. For small modules I optimize for delivery speed; for high-change domains I enforce DIP/SRP and validate with tests."
- Why strong wins: Shows context-based tradeoffs instead of dogma.

## Async/Await

- Bad answer: "I make everything async because it is faster."
- Strong answer: "I use async for I/O-bound calls to improve throughput; CPU-bound work uses dedicated execution paths. I propagate cancellation and track timeout behavior."
- Why strong wins: Correct technical boundary and operational awareness.

## EF Core Performance

- Bad answer: "EF is slow so I avoid it."
- Strong answer: "EF is productive when used correctly: projection, no-tracking reads, controlled includes, and measured query plans. I use raw SQL only for hotspots."
- Why strong wins: Demonstrates practical optimization instead of blanket rejection.

## API Design

- Bad answer: "REST is just CRUD endpoints."
- Strong answer: "API design includes explicit contracts, validation, error semantics, idempotency for retry paths, and version strategy."
- Why strong wins: Covers behavior, reliability, and compatibility.

## Security

- Bad answer: "We use JWT so security is covered."
- Strong answer: "JWT is one layer. I also enforce authorization on handlers, rotate secrets, protect data in transit/at rest, and monitor auth failures with audit-safe logs."
- Why strong wins: Shows layered security model.

## Resilience

- Bad answer: "I added retries so failures are handled."
- Strong answer: "Retries are bounded and jittered, with timeout budgets and circuit breakers to avoid retry storms. We validate behavior against error budgets."
- Why strong wins: Addresses secondary failure modes.

## Distributed Consistency

- Bad answer: "We use distributed transactions across services."
- Strong answer: "We prefer outbox/inbox with idempotent handlers and compensations. This keeps services decoupled and replay-safe under partial failures."
- Why strong wins: Realistic for modern microservice systems.

---

## Interview Answer Block

- 30-second answer: This topic covers Interview answer quality calibration and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know interview answer quality calibration and I would just follow best practices."
- Strong answer: "For interview answer quality calibration, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply answer quality calibration in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
