# Topic README Template

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Topic folder and docs index familiarity
- Related examples: Learning/Architecture/README.md


Use this template for every top-level topic folder under `Learning/`.

## Required sections

1. Learning goals
2. Prerequisites
3. Runnable examples
4. Bad vs good examples summary
5. Related docs

## Template

```md
# <Topic Name> Guide

## Learning goals

- Goal 1
- Goal 2
- Goal 3

## Prerequisites

- Prerequisite 1
- Prerequisite 2

## Runnable examples

- `<ExampleFile1.cs>` - What this example demonstrates
- `<ExampleFile2.cs>` - What this example demonstrates

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- **Bad:** anti-pattern or risky approach shown in the module
- **Good:** recommended approach shown in the module
- **Why it matters:** impact on reliability, readability, or performance

## Related docs

- [Primary doc](../docs/<Topic-Doc>.md)
- [Secondary doc](../docs/<Related-Doc>.md)
```

## Author checklist

- README exists in the topic folder root.
- README includes all required sections.
- Example filenames listed are present in that folder.
- Related doc links resolve correctly.

---

## Interview Answer Block

- 30-second answer: This topic covers Topic README authoring standards and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know topic README authoring standards and I would just follow best practices."
- Strong answer: "For topic README authoring standards, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply topic README authoring standards in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
