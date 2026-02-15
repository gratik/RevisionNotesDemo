# Appendix: Pattern Decision Guide

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Design patterns fundamentals
- Related examples: Learning/Appendices/PatternsOverratedNow.cs


**Last Updated**: 2026-02-15

A practical framework for deciding when design patterns are warranted and when simpler solutions are better.

## Module Metadata

- **Prerequisites**: Design Patterns, Domain-Driven Design, Practical Patterns
- **When to Study**: During architecture reviews and before major refactors.
- **Related Files**: `../Appendices/PatternsOverratedNow.cs`
- **Estimated Time**: 30-45 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Design-Patterns.md)
- **Next Step**: [Appendix-Quick-Reference.md](Appendix-Quick-Reference.md)
<!-- STUDY-NAV-END -->

---

## Decision Heuristic

Use this sequence before introducing architectural patterns:

1. What concrete pain exists today?
2. What is the simplest viable fix?
3. What operational burden will the pattern add?
4. Which metrics will prove it helped?

If there is no measurable pain and no success metric, defer the pattern.

---

## High-Risk Overuse Areas

- **Service Locator**: hidden dependencies and runtime surprises
- **Repository-per-entity**: duplicated CRUD abstraction over ORM
- **Abstract Factory too early**: indirection without variation
- **Mediator everywhere**: excessive ceremony for simple flows
- **Event sourcing by default**: heavy operational burden without replay requirement

---

## Pattern Scoring Model

Rate each item 0-10:

- Current pain severity
- Simpler alternative viability
- Operational complexity
- Long-term maintainability benefit

A positive composite should be required before adoption.

---

## Refactoring Playbook

1. Expose hidden dependencies in constructors.
2. Collapse duplicate abstractions.
3. Preserve behavior with focused tests.
4. Capture tradeoffs in ADR notes.
5. Re-check incident metrics after release.

---

## Reference Example

See the executable appendix demo:

- `../Appendices/PatternsOverratedNow.cs`

---

## Interview Answer Block

- 30-second answer: This topic covers Appendix Pattern Decision Guide and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Appendix Pattern Decision Guide and I would just follow best practices."
- Strong answer: "For Appendix Pattern Decision Guide, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Appendix Pattern Decision Guide in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
