# Build Warning Triage

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: C# compiler warnings and analyzers
- Related examples: Directory.Build.props, Properties/GlobalSuppressions.cs


**Last Updated**: 2026-02-15

This document tracks analyzer/compiler warnings and how we handle them without hiding meaningful quality signals.

## Current Snapshot (Full Rebuild)

Top warning families from full rebuild:

- `None` (all current analyzer/compiler warnings resolved or scoped-suppressed by policy).

Full rebuild total: **0 warnings**, **0 errors**.

## Triage Buckets

### 1. Intentional for teaching examples

These warnings are expected in sections that explicitly compare good/bad patterns or preserve readable benchmark labels.

- `CA1848` in `RevisionNotesDemo.Logging`
- `CA1707` in `RevisionNotesDemo.Performance`
- `CA1707` in `RevisionNotesDemo.DataAccess`

Action taken:

- Added targeted suppressions in `Properties/GlobalSuppressions.cs` for those namespaces only.

### 2. Easy, low-risk fixes

Action taken:

- Renamed underscore-style unit test method names in `Tests/RevisionNotesDemo.UnitTests/*.cs` to remove avoidable `CA1707` noise.
- Applied explicit `CultureInfo.InvariantCulture` casing in:
  - `Learning/ModernCSharp/NullableReferenceTypes.cs`
  - `Learning/PracticalPatterns/GlobalExceptionHandling.cs`
- Removed per-call allocation warnings in `GlobalExceptionHandling.cs` by:
  - reusing static validation message arrays
  - reusing a static `JsonSerializerOptions` instance
- Converted non-instance helper methods to `static` in:
  - `Learning/Configuration/FeatureFlags.cs`
  - `Learning/Logging/LoggingBestPractices.cs`
  - `Learning/WebAPI/ControllerAPI/WebAPIBestPractices.cs`
  - `Learning/WebAPI/MVC/MVCBestPractices.cs`
  - `Learning/Logging/ILoggerDeepDive.cs`
  - `Learning/Performance/SpanAndMemory.cs`
  - `Learning/Configuration/OptionsPatternDeepDive.cs`
  - `Learning/OOPPrinciples/KISSDRYYAGNIExamples.cs`
- Reduced formatting/culture warnings with explicit format providers in:
  - `Learning/Performance/OptimizationTechniques.cs`
  - `Learning/Performance/SpanAndMemory.cs`
- Reduced string API warnings in:
  - `Learning/OOPPrinciples/KISSDRYYAGNIExamples.cs` (`Contains("@")` -> `Contains('@')`)
- Replaced `ILogger` extension calls with cached `LoggerMessage` delegates in:
  - `Learning/Resilience/CircuitBreakerPattern.cs`
  - `Learning/Resilience/TimeoutAndBulkhead.cs`
  - `Learning/Resilience/PollyRetryPatterns.cs`
  - `Learning/WebAPI/Middleware/MiddlewareBestPractices.cs`
  - `Learning/Configuration/FeatureFlags.cs`
  - `Learning/WebAPI/MVC/MVCBestPractices.cs`
  - `Learning/WebAPI/MinimalAPI/MinimalAPIBestPractices.cs`
  - `Learning/WebAPI/ControllerAPI/WebAPIBestPractices.cs`
  - `Learning/PracticalPatterns/GlobalExceptionHandling.cs`
- Cleared remaining `CA1822` static-candidate warnings by converting safe helper methods in:
  - `Learning/WebAPI/Versioning/VersioningBestPractices.cs`
  - `Learning/DesignPatterns/Behavioral/TemplateMethodPattern.cs`
  - `Learning/DesignPatterns/Behavioral/StrategyPattern.cs`
  - `Learning/DesignPatterns/Structural/DecoratorPattern.cs`
  - `Learning/Performance/BenchmarkingExamples.cs`
  - `Learning/Security/AuthenticationExamples.cs`
  - `Learning/Security/AuthorizationExamples.cs`
- Cleared `CA1018` by adding explicit `[AttributeUsage(...)]` in:
  - `Learning/Logging/ILoggerDeepDive.cs`
  - `Learning/WebAPI/MVC/MVCBestPractices.cs`
  - `Learning/WebAPI/ControllerAPI/WebAPIBestPractices.cs`
  - `Learning/WebAPI/Versioning/VersioningBestPractices.cs`

### 3. Keep visible (actionable backlog)

Warnings in this repository are now either:
- fixed directly (preferred for correctness/safety), or
- explicitly scoped-suppressed when intentionally pedagogical.

## Policy

- Suppress only when warning is pedagogically intentional and scoped.
- Prefer fixing warnings in tests and operational code over suppressing.
- Avoid project-wide blanket suppressions.
- Keep suppression justifications concrete and namespace-scoped.

## Next Suggested Wave

1. Re-evaluate scoped suppressions periodically and convert suppressions to fixes when examples are refactored.
2. Keep new additions warning-clean by default; only add scoped suppressions with explicit educational rationale.
3. Preserve this warning policy in PR reviews (no blanket `NoWarn` additions without discussion).

---

## Interview Answer Block

- 30-second answer: This topic covers Build warning triage and suppression policy and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know warning triage and suppression decisions and I would just follow best practices."
- Strong answer: "For warning triage and suppression decisions, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply warning triage and suppression decisions in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
