# Structural Necessities (Enterprise)

> Subject: [DotNet-API-React](../README.md)

## Structural Necessities (Enterprise)

### Recommended solution boundaries

```text
src/
  Api/                 # Controllers/endpoints + transport concerns
  Application/         # Use cases, orchestration, DTO mapping
  Domain/              # Business rules and invariants
  Infrastructure/      # EF, external services, messaging, auth adapters
  Frontend.React/      # React app (features, services, shared UI)
  Contracts/           # API contracts/OpenAPI schemas
tests/
  Api.Tests/
  Application.Tests/
  Contract.Tests/      # API compatibility checks
  Frontend.Tests/      # component + integration tests
```

### Operational and governance requirements

- Contract governance: OpenAPI contract published and versioned in CI artifacts.
- Release safety: blue/green or canary deployment with rollback trigger thresholds.
- Security gates: SAST, dependency scanning, and secret scanning in pipeline.
- Configuration governance: environment-specific config with audited overrides.
- Incident readiness: runbooks for auth failures, CORS incidents, and latency regressions.

### Testing minimum for this architecture

| Layer | Required tests |
| --- | --- |
| API | authz policy tests, input validation tests, error envelope tests |
| Application | business rule unit tests, idempotency tests |
| Contract | schema compatibility tests across versions |
| Frontend | component state tests + API integration tests with mock server |
| E2E | login + critical user journey + failure state path |


