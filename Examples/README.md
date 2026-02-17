# Examples Portfolio

This solution now includes twenty-one standalone architecture examples:

1. `RevisionNotes.BlazorBestPractices`
   - Path: `Examples/RevisionNotes.BlazorBestPractices`
   - Focus: Accessible Blazor Server UI, secure cookie auth, data access, caching, performance controls

2. `RevisionNotes.MinimalApi`
   - Path: `Examples/RevisionNotes.MinimalApi`
   - Focus: Minimal API best practices with JWT auth, repository pattern, output caching, rate limiting

3. `RevisionNotes.StandardApi`
   - Path: `Examples/RevisionNotes.StandardApi`
   - Focus: Controller-based API architecture with explicit contracts, security, caching, rate limits

4. `RevisionNotes.Microservice.CatalogService`
   - Path: `Examples/RevisionNotes.Microservice.CatalogService`
   - Focus: Microservice service boundary, outbox pattern, idempotency, secure write APIs

5. `RevisionNotes.EventDriven.Worker`
   - Path: `Examples/RevisionNotes.EventDriven.Worker`
   - Focus: Event queue processing, retries, idempotency, worker health diagnostics

6. `RevisionNotes.gRPC.Service`
   - Path: `Examples/RevisionNotes.gRPC.Service`
   - Focus: Secure gRPC unary/streaming, interceptor-based error handling, cached data access

7. `RevisionNotes.Observability.Showcase`
   - Path: `Examples/RevisionNotes.Observability.Showcase`
   - Focus: Tracing/metrics/logging patterns, fault simulation, operational health endpoints

8. `RevisionNotes.CleanArchitecture`
   - Path: `Examples/RevisionNotes.CleanArchitecture`
   - Focus: Layered domain/application/infrastructure separation in a single deployable API

9. `RevisionNotes.Identity.AuthServer`
   - Path: `Examples/RevisionNotes.Identity.AuthServer`
   - Focus: Access/refresh token issuance, protected profile API, and identity service boundaries

10. `RevisionNotes.Testing.Pyramid`
   - Path: `Examples/RevisionNotes.Testing.Pyramid`
   - Focus: Unit/integration/contract testing-oriented API design with deterministic endpoints

11. `RevisionNotes.Ddd.ModularMonolith`
   - Path: `Examples/RevisionNotes.Ddd.ModularMonolith`
   - Focus: DDD module boundaries, in-process domain events, and monolith-first architecture

12. `RevisionNotes.BackgroundJobs`
   - Path: `Examples/RevisionNotes.BackgroundJobs`
   - Focus: Worker-based job queue processing, retries, idempotency, and health reporting

13. `RevisionNotes.RealTime.SignalR`
   - Path: `Examples/RevisionNotes.RealTime.SignalR`
   - Focus: Authenticated real-time messaging with hubs, groups, and message history

14. `RevisionNotes.MultiTenant.SaaS`
   - Path: `Examples/RevisionNotes.MultiTenant.SaaS`
   - Focus: Tenant resolution, tenant-scoped data isolation, and SaaS authorization patterns

15. `RevisionNotes.DataAccess.AdvancedEfCore`
   - Path: `Examples/RevisionNotes.DataAccess.AdvancedEfCore`
   - Focus: Compiled queries, split queries, optimistic concurrency, and soft-delete design

16. `RevisionNotes.Resilience.ChaosDemo`
   - Path: `Examples/RevisionNotes.Resilience.ChaosDemo`
   - Focus: Retry/timeout/circuit-breaker patterns with fault injection and fallback behavior

17. `RevisionNotes.ApiGateway.BFF`
   - Path: `Examples/RevisionNotes.ApiGateway.BFF`
   - Focus: Backend-for-Frontend aggregation, downstream fallback, and client-specific gateway shaping

18. `RevisionNotes.EventSourcing.Cqrs`
   - Path: `Examples/RevisionNotes.EventSourcing.Cqrs`
   - Focus: Command/query separation with append-only events and projection-backed reads

19. `RevisionNotes.Workflows.SagaOrchestration`
   - Path: `Examples/RevisionNotes.Workflows.SagaOrchestration`
   - Focus: Long-running workflow orchestration with compensating actions

20. `RevisionNotes.NativeAot.Api`
   - Path: `Examples/RevisionNotes.NativeAot.Api`
   - Focus: Native AOT deployment patterns, trimming-friendly API design, and lightweight security

21. `RevisionNotes.Observability.AzureMonitor`
   - Path: `Examples/RevisionNotes.Observability.AzureMonitor`
   - Focus: OpenTelemetry + Application Insights integration, correlation headers, and HTTP/Service Bus propagation

## Run each project

```bash
dotnet run --project Examples/RevisionNotes.BlazorBestPractices/RevisionNotes.BlazorBestPractices.csproj
dotnet run --project Examples/RevisionNotes.MinimalApi/RevisionNotes.MinimalApi.csproj
dotnet run --project Examples/RevisionNotes.StandardApi/RevisionNotes.StandardApi.csproj
dotnet run --project Examples/RevisionNotes.Microservice.CatalogService/RevisionNotes.Microservice.CatalogService.csproj
dotnet run --project Examples/RevisionNotes.EventDriven.Worker/RevisionNotes.EventDriven.Worker.csproj
dotnet run --project Examples/RevisionNotes.gRPC.Service/RevisionNotes.gRPC.Service.csproj
dotnet run --project Examples/RevisionNotes.Observability.Showcase/RevisionNotes.Observability.Showcase.csproj
dotnet run --project Examples/RevisionNotes.CleanArchitecture/RevisionNotes.CleanArchitecture.csproj
dotnet run --project Examples/RevisionNotes.Identity.AuthServer/RevisionNotes.Identity.AuthServer.csproj
dotnet run --project Examples/RevisionNotes.Testing.Pyramid/RevisionNotes.Testing.Pyramid.csproj
dotnet run --project Examples/RevisionNotes.Ddd.ModularMonolith/RevisionNotes.Ddd.ModularMonolith.csproj
dotnet run --project Examples/RevisionNotes.BackgroundJobs/RevisionNotes.BackgroundJobs.csproj
dotnet run --project Examples/RevisionNotes.RealTime.SignalR/RevisionNotes.RealTime.SignalR.csproj
dotnet run --project Examples/RevisionNotes.MultiTenant.SaaS/RevisionNotes.MultiTenant.SaaS.csproj
dotnet run --project Examples/RevisionNotes.DataAccess.AdvancedEfCore/RevisionNotes.DataAccess.AdvancedEfCore.csproj
dotnet run --project Examples/RevisionNotes.Resilience.ChaosDemo/RevisionNotes.Resilience.ChaosDemo.csproj
dotnet run --project Examples/RevisionNotes.ApiGateway.BFF/RevisionNotes.ApiGateway.BFF.csproj
dotnet run --project Examples/RevisionNotes.EventSourcing.Cqrs/RevisionNotes.EventSourcing.Cqrs.csproj
dotnet run --project Examples/RevisionNotes.Workflows.SagaOrchestration/RevisionNotes.Workflows.SagaOrchestration.csproj
dotnet run --project Examples/RevisionNotes.NativeAot.Api/RevisionNotes.NativeAot.Api.csproj
dotnet run --project Examples/RevisionNotes.Observability.AzureMonitor/RevisionNotes.Observability.AzureMonitor.csproj
```

Each project contains:

- `README.md` for usage and capabilities
- `docs/Architecture-Decisions.md` for architecture rationale and tradeoffs
