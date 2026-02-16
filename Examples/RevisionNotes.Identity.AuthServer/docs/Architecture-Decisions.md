# Architecture Decisions - RevisionNotes.Identity.AuthServer

## WHAT IS THIS?
This is an identity service demo covering token issuance, refresh flow, protected resource access, and operational safeguards.

## WHY IT MATTERS?
- Authentication and token lifecycle logic are high-risk areas that benefit from clear architecture.
- This demo shows separation of identity concerns from business APIs.

## WHEN TO USE?
- When you need a dedicated auth boundary for issuing and validating client tokens.
- When auditing and controlled token rotation are required.

## WHEN NOT TO USE?
- When a trusted external identity provider already fulfills all requirements.
- When a simple internal-only app can rely on platform authentication without custom token flows.

## REAL-WORLD EXAMPLE?
A platform identity service issuing short-lived tokens for multiple internal APIs consumed by web and mobile clients.

## Decision 1: Separate identity concerns into a dedicated service

- Token issuance logic is isolated from business APIs.
- Security updates can be deployed independently of domain services.

## Decision 2: Use short-lived access tokens and refresh rotation

- Access tokens are intentionally short-lived.
- Refresh tokens are one-time use in this sample to reduce replay risk.

## Decision 3: Keep issuance flow explicit and auditable

- `/connect/token` and `/connect/refresh` are simple and transparent for learning.
- Request logging supports forensic tracing and incident response.

## Decision 4: Operational safeguards are mandatory

- Global exception handling keeps response contracts predictable.
- Health checks expose liveness and readiness for orchestrators.

## Decision 5: Design for upgrade to full OIDC server

- Token factory and store abstractions can be replaced with OpenIddict or enterprise identity infrastructure.
