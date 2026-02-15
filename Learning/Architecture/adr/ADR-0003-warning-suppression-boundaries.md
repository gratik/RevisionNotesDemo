# ADR-0003: Warning Suppression Boundaries

- Status: Accepted
- Date: February 15, 2026
- Deciders: Maintainers

## Context

Educational samples intentionally include patterns that trigger analyzers. Without boundaries, suppressions can become broad, undocumented, and hard to review.

## Decision

- Default policy is fix-first, suppress-second.
- Suppressions must be scoped as narrowly as possible.
- All suppressions require clear justifications.
- Suppression inventory must be documented in:
  - `Learning/docs/Build-Warning-Triage.md`
  - `Learning/docs/Suppression-Audit.md`
- CI warning gate enforces `-warnaserror` for:
  - `RevisionNotesDemo.csproj`
  - `RevisionNotesDemo.UnitTests.csproj`
- `TestingExamples` remains build-validated but not warning-gated because it intentionally demonstrates analyzer-triggering naming/style patterns.

## Consequences

- Warning debt is explicit and reviewable.
- Production-style projects stay clean.
- Educational intent remains preserved without lowering default quality for core builds.
