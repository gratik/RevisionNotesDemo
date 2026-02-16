# ADR-0001: Topic Naming and README Convention

- Status: Accepted
- Date: February 15, 2026
- Deciders: Maintainers

## Context

Topic-level content was uneven in naming and onboarding quality. Some folders had rich guides, while others had no README or inconsistent structure.

## Decision

- Every top-level folder under `Learning/` must include `README.md`.
- Topic README files must contain these sections:
  - `## Learning goals`
  - `## Prerequisites`
  - `## Runnable examples`
  - `## Bad vs good examples summary`
  - `## Related docs`
- Topic folder names remain concise and PascalCase where possible.
- Documentation-only folders (for example `docs`) are not required to follow the topic README template.

## Consequences

- New topic areas are easier to navigate and onboard.
- CI can enforce README consistency automatically.
- Existing rich narrative sections can remain, but the required template blocks must be present first.
