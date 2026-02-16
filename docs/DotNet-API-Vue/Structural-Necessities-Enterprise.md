# Structural Necessities (Enterprise)

> Subject: [DotNet-API-Vue](../README.md)

## Structural Necessities (Enterprise)

### Recommended architecture slices

```text
frontend/vue-app/
  src/features/         # feature modules (catalog/orders/profile)
  src/services/         # API service layer + interceptors
  src/composables/      # view-model and lifecycle orchestration
  src/components/       # reusable UI units
backend/api/
  Controllers/          # transport and auth boundary
  Application/          # use-case orchestration
  Domain/               # business invariants
  Infrastructure/       # persistence/integration adapters
shared/contracts/
  openapi/              # contract artifacts and changelog
```

### Enterprise delivery requirements

- Contract checks in CI for breaking schema changes.
- Deployment with automated health-gated rollback.
- Environment baseline for CORS, auth authority, and API base URL documented.
- Observability dashboard with release markers and alert runbooks.
- Error budget tracking to guide release cadence decisions.

### Testing minimum for this architecture

| Layer | Required tests |
| --- | --- |
| API | authorization policy tests + validation tests + ProblemDetails response tests |
| Application | business workflow unit tests + boundary condition tests |
| Contract | OpenAPI diff checks and consumer compatibility tests |
| Vue frontend | composable tests + component behavior tests + API mock integration tests |
| E2E | auth flow + list/search/paging + fault-path behavior |


