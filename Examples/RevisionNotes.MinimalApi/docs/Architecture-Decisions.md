# Architecture Decisions - RevisionNotes.MinimalApi

## WHAT IS THIS?
This is a Minimal API reference showing concise endpoint design with JWT security, layered caching, rate limiting, and health diagnostics.

## WHY IT MATTERS?
- Minimal APIs are fast to build but can become inconsistent without conventions.
- This demo shows how to keep low ceremony without sacrificing operational quality.

## WHEN TO USE?
- When building small-to-medium APIs where speed and clarity are priorities.
- When you want explicit control of pipeline behavior with minimal framework overhead.

## WHEN NOT TO USE?
- When large teams need heavy controller/filter conventions across many endpoints.
- When API complexity requires extensive action-level metadata and cross-cutting filters.

## REAL-WORLD EXAMPLE?
A lightweight backend for a mobile task-management app that needs secure auth, fast reads, and throttled write operations.

## ADR-01: Minimal API over controllers
- Decision: Use Minimal API for concise endpoint composition.
- Why: Lower ceremony, easier to teach endpoint pipeline behavior.
- Tradeoff: Large APIs can become noisy; split by feature folders.

## ADR-02: EF Core InMemory with repository abstraction
- Decision: Use InMemory provider but keep repository interface.
- Why: Keeps sample runnable while preserving swap path to SQL/Postgres.
- Tradeoff: InMemory does not enforce relational constraints like production databases.

## ADR-03: JWT bearer with strict validation
- Decision: Validate issuer, audience, signing key, and expiration.
- Why: Prevents token replay across systems and weak-token acceptance.
- Tradeoff: Key management must be handled outside source code in real deployments.

## ADR-04: Layered caching
- Decision: Combine `IMemoryCache` for data-access memoization with output caching for HTTP responses.
- Why: Demonstrates application-level and transport-level caching controls.
- Tradeoff: Must invalidate both layers on writes.

## ADR-05: Rate limiting + secure headers by default
- Decision: Limit write requests and emit restrictive security headers.
- Why: Reduces abuse and common browser attack vectors.
- Tradeoff: Limits need tuning per endpoint traffic profile.

## ADR-06: Explicit observability and health endpoint
- Decision: Add health checks and clear failure surface (`/error`).
- Why: Better operational diagnosis and load balancer integration.
- Tradeoff: Health endpoints should be protected/network-scoped in production.
