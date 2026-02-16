# Scope and principles

> Subject: [Suppression-Audit](../README.md)

## Scope and principles

- Suppressions are allowed only when examples intentionally demonstrate alternatives, anti-patterns, migration paths, or readability-first teaching code.
- Prefer fixing warnings in production-style examples.
- Keep suppressions narrow: project-level `NoWarn` should be limited to compiler/tooling noise that is intentional across demos; analyzer suppressions should be scoped with `Target`.
- Every new suppression should include a clear justification and should be reviewed periodically.
- CI warning gate is enforced for `RevisionNotesDemo.csproj` and `RevisionNotesDemo.UnitTests.csproj`; `TestingExamples` remains build-validated but not warning-gated because it intentionally contains analyzer-triggering teaching patterns.


