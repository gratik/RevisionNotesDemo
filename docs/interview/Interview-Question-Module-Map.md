# Interview Question-to-Module Map

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Preparation Guide, Interview Role Tracks
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs

Use this map to convert common interview prompts into focused study and practice actions.

## Question routing table

| Interview question pattern | Primary doc module | Primary runnable example | Practice action |
| --- | --- | --- | --- |
| Explain DI lifetimes and common mistakes. | `Configuration.md` | `Learning/Configuration/DependencyInjectionExamples.cs` | Give one singleton/scoped/transient scenario and one captive dependency failure mode. |
| When should you use async/await and when not? | `Async-Multithreading.md` | `Learning/AsyncMultithreading/AsyncAwaitBestPractices.cs` | Contrast I/O-bound vs CPU-bound work and include cancellation/token usage. |
| EF Core seems slow. How do you improve it? | `Entity-Framework.md` | `Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs` | Show projection, no-tracking query, and N+1 prevention with measured before/after signal. |
| How do you version APIs safely? | `API-Documentation.md` | `Learning/WebAPI/OpenAPISwaggerAdvanced.cs` | State contract policy, deprecation path, and compatibility test approach. |
| How do you secure a modern .NET service? | `Security.md` | `Learning/Security/JwtAuthenticationAndAuthorization.cs` | Describe authN/authZ separation, secret rotation, and one auditability control. |
| What resilience patterns do you apply first? | `Resilience.md` | `Learning/Resilience/PollyRetryPatterns.cs` | Explain timeout + capped retry + jitter + circuit breaker with one rollback path. |
| How do you prevent duplicate events in microservices? | `Distributed-Consistency.md` | `Learning/Microservices/DistributedConsistencyPatterns.cs` | Walk through outbox/inbox + idempotency key + replay-safe handler design. |
| How do you monitor service health in production? | `Logging-Observability.md` | `Learning/Observability/OpenTelemetrySetup.cs` | Give logs/metrics/traces set and one alert threshold tied to business impact. |
| How should tests be layered in .NET systems? | `Testing.md` | `Learning/Testing/TestingFrameworksComparison.cs` | Define unit/integration/contract boundaries and failure ownership by layer. |
| Explain a design tradeoff you made recently. | `End-to-End-Case-Study.md` | `Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs` | Use context, alternatives, tradeoff, mitigation, and metric outcome format. |

## Fast response recipe

1. Restate the question with scope (service boundary, scale, and risk).
2. Give one decision and one alternative.
3. Explain the tradeoff and failure mode.
4. State mitigation and measurable validation signal.
5. Close with one outcome or lesson.

## Role-targeted question focus

| Role | Highest-value question clusters |
| --- | --- |
| Backend .NET Engineer | DI lifetimes, async boundaries, EF performance, API contracts |
| Senior .NET Engineer | resilience strategy, observability depth, architecture tradeoffs |
| Platform / DevOps Engineer | deployment safety, health checks, rollback triggers, telemetry gating |
| Solutions Architect | boundary design, consistency guarantees, cross-service failure strategy |

## Interview Answer Block

- 30-second answer: This map links interview prompts directly to the best study module and runnable example so practice stays targeted.
- 2-minute deep dive: For each question type, I route to one primary doc and one executable reference, then rehearse a five-part answer with tradeoffs and metrics.
- Common follow-up: How do you use this under time pressure?
- Strong response: I pick the question cluster for my target role, drill only mapped items, and track weak signals in the progress log.
- Tradeoff callout: Covering too many unmapped topics lowers depth and weakens answer quality.

## Interview Bad vs Strong Answer

- Bad answer: “I’ll just revise everything and hope my examples match.”
- Strong answer: “I map each common prompt to one doc and one runnable file, then practice a structured answer with risk and metric evidence.”
- Why strong wins: It demonstrates focused preparation and applied technical judgment.

## Interview Timed Drill

- Time box: 12 minutes.
- Prompt: Pick two question patterns from the table and deliver a 90-second answer for each.
- Required outputs:
  - one concrete tradeoff per answer
  - one failure mode and mitigation per answer
  - one measurable validation signal per answer
- Self-check score (0-3 each): relevance, technical depth, measurable clarity.
