# ADR-0002: Canonical vs Reference Folder Ownership

- Status: Accepted
- Date: February 15, 2026
- Deciders: Maintainers

## Context

Some domains used duplicate files across folders for different purposes. Without ownership rules, concept catalogs could drift from runnable implementations.

## Decision

- `Learning/DataAccess` is the canonical implementation track for database/data-access behavior.
- `Learning/Database` is reference-only and must not be treated as authoritative runtime implementation.
- `Learning/Database/**/*.cs` is excluded from app compile items.
- Files in `Learning/Database` must have matching canonical counterparts in `Learning/DataAccess`.

## Consequences

- Runtime behavior is defined in one place.
- Concept catalogs remain useful for quick comparison without impacting executable builds.
- CI structure validation catches ownership drift early.
