# Appendix: Quick Reference Decision Tables

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: General .NET development familiarity
- Related examples: Learning/Appendices/QuickReferenceTables.cs


**Last Updated**: 2026-02-15

Compact decision defaults for runtime, API, data, security, and operational choices.

## Module Metadata

- **Prerequisites**: Performance, Data Access, Security, Deployment and DevOps
- **When to Study**: Before implementation and during code reviews.
- **Related Files**: `../Learning/Appendices/QuickReferenceTables.cs`
- **Estimated Time**: 25-35 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Practical-Patterns.md)
- **Next Step**: [Appendix-Interview-Playbook.md](Appendix-Interview-Playbook.md)
<!-- STUDY-NAV-END -->

---

## Core Principle

Use these as defaults, then override only when constraints are explicit and measured.

---

## Runtime Defaults

- Prefer async end-to-end for I/O paths.
- Use bounded concurrency for CPU-heavy tasks.
- Use structured logs in hot paths.
- Combine timeout + retry + jitter for transient faults.

## API Defaults

- Version explicitly.
- Standardize ProblemDetails responses.
- Support idempotency keys for retried writes.
- Use pagination/projection by default.

## Data Defaults

- Use `AsNoTracking` for read-only queries.
- Avoid loading full entities when projections suffice.
- Keep DbContext scoped per request.
- Apply bounded cache strategy with expiry.

## Security Defaults

- Short-lived access tokens.
- Secure/HttpOnly/SameSite for auth cookies.
- Secrets in vault with identity-based access.
- No internal details in client-facing errors.

## Operations Defaults

- Canary or blue/green rollouts.
- Logs + metrics + traces baseline.
- SLO-based alerting.
- Verified rollback runbook.

---

## Reference Example

See the executable appendix demo:

- `../Learning/Appendices/QuickReferenceTables.cs`

---

## Interview Answer Block

- 30-second answer: This topic covers Appendix Quick Reference and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Appendix Quick Reference and I would just follow best practices."
- Strong answer: "For Appendix Quick Reference, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Appendix Quick Reference in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.


