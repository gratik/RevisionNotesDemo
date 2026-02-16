# Architecture Decisions - RevisionNotes.Observability.Showcase

## WHAT IS THIS?
This is an observability-focused API demo that teaches logging, tracing, metrics, health probes, and failure-path instrumentation.

## WHY IT MATTERS?
- Systems are only operable if failures and performance issues are visible quickly.
- This demo makes telemetry design a first-class architecture concern.

## WHEN TO USE?
- When teams need a reference for implementing diagnostics before incidents occur.
- When onboarding engineers to metrics/tracing/logging fundamentals.

## WHEN NOT TO USE?
- When the application is a short-lived prototype with no operational support expectations.
- When telemetry overhead is unacceptable for a throwaway experiment.

## REAL-WORLD EXAMPLE?
An API platform team validating dashboards and alerting rules by generating controlled failures and measuring end-to-end traces.

## ADR-01: Manual instrumentation-first approach
- Decision: Use native `ActivitySource` and `Meter` directly.
- Why: Keeps signal creation explicit and understandable for learning.
- Tradeoff: More boilerplate than auto-instrumentation packages.

## ADR-02: Middleware for unified request telemetry
- Decision: Capture request timing and status from one middleware.
- Why: Ensures consistent metrics/log shape across endpoints.
- Tradeoff: Endpoint-specific dimensions still require additional tags.

## ADR-03: Failure endpoint for incident rehearsal
- Decision: Keep an intentional failing endpoint.
- Why: Enables realistic testing of alerts, logs, and trace flows.
- Tradeoff: Must be gated/disabled in production environments.

## ADR-04: Centralized exception handling
- Decision: Use `IExceptionHandler` with ProblemDetails response.
- Why: Standardizes error contract while preserving detailed server logs.
- Tradeoff: Additional mapping may be needed for domain-specific error types.

## ADR-05: Health split (live/ready)
- Decision: Separate liveness from readiness checks.
- Why: Supports orchestrators and zero-downtime rollouts with clear semantics.
- Tradeoff: Readiness logic needs regular tuning as dependencies evolve.
