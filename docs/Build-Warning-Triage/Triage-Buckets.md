# Triage Buckets

> Subject: [Build-Warning-Triage](../README.md)

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

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Triage Buckets before implementation work begins.
- Keep boundaries explicit so Triage Buckets decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Triage Buckets in production-facing code.
- When performance, correctness, or maintainability depends on consistent Triage Buckets decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Triage Buckets as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Triage Buckets is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Triage Buckets are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

