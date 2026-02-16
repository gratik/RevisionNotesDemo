# Architecture Decisions - RevisionNotes.MultiTenant.SaaS

## WHAT IS THIS?
This is a multi-tenant SaaS API demo with explicit tenant resolution, tenant-scoped data access, and authenticated endpoints.

## WHY IT MATTERS?
- Tenant isolation mistakes are high-impact security and data-integrity risks.
- This demo shows how to make tenant context explicit in every request path.

## WHEN TO USE?
- When one application instance serves multiple customer tenants.
- When tenant identification and isolation rules must be enforced centrally.

## WHEN NOT TO USE?
- When every tenant has dedicated infrastructure and no shared runtime.
- When requirements are single-tenant and complexity should stay minimal.

## REAL-WORLD EXAMPLE?
A B2B project-management SaaS where each customer only sees its own projects and usage data.

## ADR-01: Header-based tenant resolution middleware
- Decision: Resolve tenant from `X-Tenant-Id` and reject missing/unknown tenants.
- Why: Makes tenant context explicit and auditable.
- Tradeoff: External gateways or clients must consistently set tenant headers.

## ADR-02: Tenant-scoped store boundary
- Decision: Partition data by tenant id in store-level APIs.
- Why: Prevents accidental cross-tenant reads/writes.
- Tradeoff: In-memory storage is only for learning; real systems need persistent tenant-aware databases.

## ADR-03: Auth + tenant context together
- Decision: Require authenticated access in addition to tenant header.
- Why: Identity and tenancy must both be validated for safe access control.
- Tradeoff: Adds request setup complexity for clients and test harnesses.
