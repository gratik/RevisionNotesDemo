# Interview Timed Practice Sets

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Core C#, Web API, Data Access fundamentals
- Related examples: Learning/Testing/TestingFrameworksComparison.cs, Learning/WebAPI/MinimalAPI/MinimalAPIBestPractices.cs

Use these rounds for timed rehearsal with scoring.

## Scoring rubric (per set)

- Problem framing and assumptions: 0-3
- Technical correctness: 0-3
- Tradeoff articulation: 0-2
- Communication clarity: 0-2
- Total: 10 points

## 15-minute set (rapid fundamentals)

1. Explain `IEnumerable` vs `IQueryable` and one production pitfall.
2. Write a minimal API endpoint with validation and proper error response.
3. Describe one retry anti-pattern and its fix.

Target score:
- 8+: interview-ready fundamentals
- 6-7: refine clarity and tradeoff detail
- <=5: revisit core docs before next round

## 30-minute set (implementation + debugging)

1. Given an EF query with N+1 behavior, propose a fixed query pattern.
2. Review a pseudo-order API flow and identify idempotency gaps.
3. Define logging/tracing fields needed to debug failed checkout requests.

Expected outputs:
- One corrected code sketch
- One risk list with priority ordering
- One minimal observability checklist

## 45-minute set (system design mini-loop)

Prompt:
- Design an order-placement flow with payment, inventory, and notification services.

Required sections in your answer:
1. Service boundaries and contracts
2. Consistency model (outbox/inbox or alternatives)
3. Failure strategy (timeouts, retries, breaker)
4. Security boundaries (authN/authZ/secrets)
5. Rollout and rollback plan
6. Metrics and alerts (p95 latency, error rate, queue lag)

Evaluation tips:
- Keep assumptions explicit.
- Prefer concrete failure and recovery paths.
- Quantify at least one latency or reliability target.

---

## Interview Answer Block

- 30-second answer: This topic covers Timed interview rehearsal and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know timed interview rehearsal and I would just follow best practices."
- Strong answer: "For timed interview rehearsal, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply timed interview rehearsal in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
