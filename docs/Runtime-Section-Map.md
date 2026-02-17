# Runtime Section Map

## Metadata
- Owner: Maintainers
- Last updated: February 16, 2026
- Prerequisites: `dotnet run --section ...` usage
- Related examples: Demo/DemoOrchestrator.cs

This page maps runtime sections to learning folders so learners can quickly locate source files.

## Runtime Sections

- `oop` -> `Learning/OOPPrinciples`
- `clean-code` -> `Learning/OOPPrinciples` (KISS/DRY/YAGNI/TDA)
- `creational` -> `Learning/DesignPatterns/Creational`
- `structural` -> `Learning/DesignPatterns/Structural`
- `behavioral` -> `Learning/DesignPatterns/Behavioral`
- `memory` -> `Learning/MemoryManagement`
- `async` -> `Learning/AsyncMultithreading`
- `dotnet` -> `Learning/CoreCSharpFeatures`, `Learning/LINQAndQueries`, `Learning/AdvancedCSharp`
- `practical` -> `Learning/PracticalPatterns`, `Learning/Models`
- `appendices` -> `Learning/Appendices`
- `cloud` -> `Learning/Cloud`
- `microservices` -> `Learning/Microservices`
- `architecture` -> `Learning/Architecture`
- `devops` -> `Learning/DevOps`
- `observability` -> `Learning/Observability`
- `security` -> `Learning/Security`
- `iot` -> `Learning/IoTEngineering`
- `operations` -> `Learning/OperationalExcellence`
- `engineering-process` -> `Learning/EngineeringProcess`

## Topics Not Run As Standalone Sections

These are discoverable as docs and examples but are not separate runtime section keys:

- `Learning/Configuration`
- `Learning/DataAccess`
- `Learning/DomainDrivenDesign`
- `Learning/FrontEnd`
- `Learning/HealthChecks`
- `Learning/Logging`
- `Learning/ModernCSharp`
- `Learning/RealTime`
- `Learning/Resilience`
- `Learning/Testing` (compiled out of main demo app)
- `Learning/WebAPI`

Use docs index and learning path for navigation:

- [Docs Index](../README.md)
- [Learning Path](Learning-Path.md)

## Detailed Guidance

Runtime Section Map guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Runtime Section Map before implementation work begins.
- Keep boundaries explicit so Runtime Section Map decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Runtime Section Map in production-facing code.
- When performance, correctness, or maintainability depends on consistent Runtime Section Map decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Runtime Section Map as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Runtime Section Map is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Runtime Section Map are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

