# Architecture Decisions - RevisionNotes.StandardApi

## WHAT IS THIS?
This is a controller-based ASP.NET Core Web API demo that emphasizes explicit contracts, policy-based security, caching, and maintainable layering.

## WHY IT MATTERS?
- Many enterprise teams standardize on controllers for predictability and governance.
- This demo shows how to keep that model secure, performant, and testable.

## WHEN TO USE?
- When your team prefers controller conventions and attribute-driven API structure.
- When you need a familiar enterprise API shape with clear separation of concerns.

## WHEN NOT TO USE?
- When the API is very small and minimal endpoints would reduce unnecessary boilerplate.
- When rapid experimentation is more important than controller-level structure.

## REAL-WORLD EXAMPLE?
A customer-account API for a line-of-business platform where multiple teams collaborate and rely on stable controller contracts.

## ADR-01: Controllers for explicit API surface
- Decision: Use controller classes and action methods.
- Why: Better fit for teams needing conventions, filters, model binding control.
- Tradeoff: More boilerplate than minimal APIs.

## ADR-02: Security first middleware order
- Decision: Apply secure headers, HTTPS, authN/authZ before endpoint execution.
- Why: Prevent accidental exposure and enforce policy globally.
- Tradeoff: Slightly more startup configuration complexity.

## ADR-03: EF Core repository boundary
- Decision: Hide EF queries behind repository interfaces.
- Why: Reduces controller data-access knowledge and supports replacement testing.
- Tradeoff: Over-abstraction risk in very small APIs.

## ADR-04: Dual-layer caching with explicit invalidation
- Decision: Use in-process cache + output cache tags.
- Why: Fast reads and clear write invalidation model.
- Tradeoff: Requires careful invalidation discipline.

## ADR-05: Rate limiting for write actions
- Decision: Bind create/update actions to `write-policy`.
- Why: Protects from abuse/spikes while preserving read throughput.
- Tradeoff: Legitimate burst traffic may need policy tuning.

## ADR-06: Operational readiness by default
- Decision: Include health checks and structured failure endpoint.
- Why: Improves deploy-time diagnostics and load balancer checks.
- Tradeoff: Health endpoints should be restricted in production networks.
