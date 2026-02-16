# Architecture Decisions - RevisionNotes.BlazorBestPractices

## ADR-01: Blazor Server for secure app logic placement
- Decision: Keep UI logic server-side rather than WASM-only.
- Why: Sensitive logic and data access stay on the server boundary.
- Tradeoff: Requires stable server connectivity.

## ADR-02: Cookie auth for interactive UI
- Decision: Use secure cookie authentication with strict flags.
- Why: Fits server-rendered interactive apps and avoids token handling in browser storage.
- Tradeoff: Cross-origin API access patterns need extra configuration.

## ADR-03: Accessibility as first-class concern
- Decision: Add skip links, semantic headings, labeled form fields, and status regions.
- Why: Improves keyboard/screen-reader usability and meets inclusive UX goals.
- Tradeoff: Slightly more markup and style discipline.

## ADR-04: Data access via `IDbContextFactory`
- Decision: Use db context factory in component-driven workflows.
- Why: Avoids long-lived context misuse across interactive operations.
- Tradeoff: More explicit per-operation context creation.

## ADR-05: Read caching + output caching for API endpoints
- Decision: Cache both query results and HTTP responses.
- Why: Improves UI responsiveness and API efficiency.
- Tradeoff: Requires cache invalidation on writes.

## ADR-06: Secure and operational middleware defaults
- Decision: Apply CSP/security headers, HTTPS, rate limiting, health checks.
- Why: Demonstrates production-aligned baseline in learning projects.
- Tradeoff: Policies require environment-specific tuning as traffic grows.