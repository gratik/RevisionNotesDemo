# Distributed Consistency Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Microservices communication basics
- Related examples: Learning/Microservices/DistributedConsistencyPatterns.cs, Learning/Microservices/DistributedTransactionsAndSaga.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Module Metadata

- **Prerequisites**: Data Access, Message Architecture, Resilience
- **When to Study**: When introducing asynchronous cross-service workflows or replacing direct distributed transactions.
- **Related Files**: `../Microservices/DistributedConsistencyPatterns.cs`, `../Microservices/DistributedTransactionsAndSaga.cs`, `../Microservices/EventDrivenArchitecture.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Design-Patterns.md)
- **Next Step**: [Resilience.md](Resilience.md)
<!-- STUDY-NAV-END -->

## Overview

This module covers the practical reliability patterns used to keep distributed systems consistent under retries, duplicates, and partial failures.

---

## Core Patterns

### Outbox Pattern

Write domain state and integration event in one local transaction, then publish later from an outbox processor.

### Inbox Pattern

Track processed message ids at consumers so duplicate deliveries do not create duplicate business effects.

### Idempotency Keys

Require retry-safe request keys on write APIs and cache/return the first successful response.

### Deduplication Window

Retain processed keys/ids long enough to cover realistic retry/replay windows.

### Saga Compensation

Model reversible steps so partial success can be safely compensated after downstream failures.

---

## Exactly-Once Myth (Practical View)

- Transport is usually at-most-once or at-least-once.
- Exactly-once end-to-end is not free.
- Practical target: **exactly-once business effect** with idempotency + dedupe + compensations.

---

## Reference Implementation

- [Microservices/DistributedConsistencyPatterns.cs](../Microservices/DistributedConsistencyPatterns.cs)

Includes:
- outbox + inbox simulation
- idempotency replay handling
- saga compensation walkthrough

---

## Failure Playbook

If duplicate effects occur:
1. Verify idempotency key propagation end-to-end.
2. Check inbox dedupe store retention and key hashing.
3. Validate compensation handlers are idempotent.
4. Inspect replay/retry policies in brokers and clients.

---

## See Also

- [Message Architecture](Message-Architecture.md)
- [Resilience](Resilience.md)
- [Data Access](Data-Access.md)
- [End-to-End Case Study](End-to-End-Case-Study.md)

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Resilience.md](Resilience.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Distributed Consistency and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Distributed Consistency and I would just follow best practices."
- Strong answer: "For Distributed Consistency, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Distributed Consistency in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
