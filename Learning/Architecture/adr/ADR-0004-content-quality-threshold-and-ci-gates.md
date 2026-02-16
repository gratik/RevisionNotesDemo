# ADR-0004: Content Quality Threshold and CI Gates

- Status: Accepted
- Date: February 15, 2026
- Deciders: Maintainers

## Context

Content scale increased across docs and topic modules. Manual review alone is not sufficient to keep links, ownership boundaries, and onboarding structure consistent.

## Decision

- CI must run:
  - `scripts/validate-docs.sh`
  - `scripts/validate-content-structure.sh`
- Docs validation must fail on:
  - broken internal links
  - missing local file references
  - orphaned pages in `docs`
- Content-structure validation must fail on:
  - missing required topic README sections
  - missing DataAccess canonical counterpart for Database reference files
  - missing project exclusion for `Learning/Database/**/*.cs`

## Consequences

- Content regressions are detected before merge.
- Topic onboarding remains predictable.
- Structural conventions become enforceable policy rather than optional guidance.
