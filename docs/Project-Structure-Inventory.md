# Project Structure and Inventory

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Repository layout familiarity
- Related examples: Learning/Architecture/README.md, Demo/DemoOrchestrator.cs

This page holds the detailed inventory previously embedded in the top-level README.

## High-Level Structure

```
RevisionNotesDemo/
├── Learning/                       Topic-organized examples and guides
├── Demo/                           Section orchestration and execution flow
├── Tests/                          Unit test and testing-example projects
├── Program.cs                      Application entry point
├── RevisionNotesDemo.csproj        Main project definition
└── docs/                  Detailed documentation set
```

## Learning Folder Inventory

### Core sections

- `Learning/OOPPrinciples/` - SOLID and clean design principles
- `Learning/CoreCSharpFeatures/` - Generics, delegates, extension methods, polymorphism
- `Learning/ModernCSharp/` - Pattern matching, records, nullable references
- `Learning/MemoryManagement/` - Heap/stack, GC behavior, leak detection
- `Learning/AsyncMultithreading/` - Task/await internals, deadlock prevention
- `Learning/LINQAndQueries/` - Query syntax and provider boundary behavior
- `Learning/DesignPatterns/` - Creational, structural, and behavioral implementations
- `Learning/WebAPI/` - Minimal API, controller API, MVC, middleware, versioning, gRPC
- `Learning/Security/` - Authentication, authorization, secure API and identity patterns
- `Learning/Testing/` - xUnit, NUnit, MSTest, mocking, integration test examples
- `Learning/PracticalPatterns/` - Caching, mapping, options, exception handling

### Expansion sections

- `Learning/Cloud/` - Azure app hosting, functions, storage, deployment
- `Learning/Microservices/` - Service communication, discovery, consistency, saga patterns
- `Learning/Architecture/` - Architecture decisions and end-to-end delivery examples
- `Learning/DevOps/` - CI/CD, Terraform, Kubernetes, Helm, workflows
- `Learning/Observability/` - Logs, metrics, traces, health heartbeat patterns
- `Learning/DataAccess/` - Canonical runnable data-access implementation track (single source of truth)
- `Learning/Database/` - Reference-only index that redirects to canonical `Learning/DataAccess/` examples
- `Learning/Appendices/` - Quick-reference and interview-focused companion material

## Organization Principles

- Group by domain first, then by pattern/concern.
- Keep runnable implementation behavior canonical in one location.
- Keep reference catalogs concise and non-authoritative.
- Require topic-level README onboarding sections for each top-level folder.
- Keep docs and code discoverable through explicit cross-links.

## Example Snippets Index

- Minimal API + controller comparisons: `Learning/WebAPI/`
- Outbox/idempotency and distributed consistency: `Learning/Microservices/DistributedConsistencyPatterns.cs`
- Integrated cross-cutting delivery slices: `Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs`
- Observability operational patterns: `Learning/Observability/OpenTelemetrySetup.cs`

## Project Snapshot

- Language/runtime: .NET 10 / C#
- Scope: multi-topic educational reference with runnable demonstrations
- Quality gates: docs validator, metadata validator, content-structure validator, warning gate

For navigation by level and estimated study time, use [Learning Path](Learning-Path.md) and [Docs Index](README.md).

---

## Interview Answer Block

- 30-second answer: This topic covers Repository structure and module ownership and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know repository structure and ownership boundaries and I would just follow best practices."
- Strong answer: "For repository structure and ownership boundaries, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply repository structure and ownership boundaries in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
