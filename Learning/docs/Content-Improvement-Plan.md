# Content Improvement Plan

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Repository structure overview
- Related examples: scripts/validate-docs.sh, scripts/validate-content-structure.sh


This plan turns the current improvement backlog into a concrete, trackable sequence.

## Objectives

1. Increase navigation clarity between docs and runnable code.
2. Standardize topic-level onboarding quality.
3. Add automated quality checks for documentation integrity.

## Phase Plan

## Phase 1 (Implemented)

### 1) Docs-to-code coverage map

- Deliverable: `Content-Coverage.md` with doc-to-code mapping and gap flags.
- Success criteria:
  - Every major document maps to one or more canonical code locations.
  - Missing-example or weak-coverage areas are explicitly marked.
- Status: Completed.

### 2) Standard topic README template

- Deliverable: Topic README standard (goals, prerequisites, runnable examples, bad-vs-good summary, related docs).
- Success criteria:
  - All top-level topic folders in `Learning/` contain a README.
  - New READMEs use consistent sections and wording conventions.
- Status: Completed.

### 5) Docs quality gates in CI

- Deliverable: CI step that checks docs links/references and orphan docs.
- Success criteria:
  - Build fails on broken internal links.
  - Build fails on missing referenced local files.
  - Build fails on unindexed docs pages.
- Status: Completed.

## Phase 2 (Next)

### 3) Split reference vs runnable demo paths

- Deliverable: Clear separation between conceptual catalog content and executable canonical examples.
- Implementation:
  - `Learning/DataAccess` remains canonical runnable source.
  - `Learning/Database` is now explicitly reference-only and excluded from compile items.
  - CI validates canonical matching between `Learning/Database` and `Learning/DataAccess`.
- Status: Completed.

### 4) ADRs for content conventions

- Deliverable: `Learning/Architecture/adr/` with decisions for naming, folder ownership, suppression boundaries, and content quality thresholds.
- Status: Completed.

### 6) Progressive learning path metadata

- Deliverable: add level tags (`Beginner`, `Intermediate`, `Advanced`) and estimated time to docs index entries.
- Status: Completed.

## Phase 3 (Later)

### 7) Docs metadata linting

- Deliverable: enforce per-doc metadata block (owner, updated date, prerequisites, related examples).
- Status: Completed.

### 8) Expanded end-to-end integrated examples

- Deliverable: one cohesive cross-cutting example per domain slice (API + data + resilience + observability + deployment).
- Status: Completed.

### 9) Trim top-level README and move deep inventories to docs

- Deliverable: concise orientation README; detailed matrices moved to dedicated docs pages.
- Status: Completed.

### 10) Content backlog with acceptance criteria

- Deliverable: prioritized backlog with owner, scope, and definition of done.

## Phase 4 (Interview Prep Expansion)

### 11) Interview answer blocks in docs

- Deliverable: concise `30-second` and `2-minute` answer blocks with common follow-ups and tradeoffs.
- Initial implementation: `Interview-Answer-Blocks.md` plus links from Interview Preparation.
- Status: Completed.

### 12) Bad vs strong answer calibration

- Deliverable: topic-based examples showing weak vs strong interview responses.
- Initial implementation: `Interview-Bad-vs-Strong-Answers.md`.
- Status: Completed.

### 13) Timed interview practice sets

- Deliverable: 15/30/45-minute timed rounds with scoring rubric.
- Initial implementation: `Interview-Timed-Practice-Sets.md`.
- Status: Completed.

### 14) Role-specific interview prep tracks

- Deliverable: role-aligned preparation paths with priority modules and expected outcomes.
- Initial implementation: `Interview-Role-Tracks.md`.
- Status: Completed.

### 15) Interview trap catalog

- Deliverable: concise list of common weak-answer patterns and stronger alternatives.
- Initial implementation: `Interview-Common-Traps.md`.
- Status: Completed.

### 16) Whiteboard and system-design prompt bank

- Deliverable: reusable system-design prompts with objective scoring rubric and expected answer sections.
- Initial implementation: `Interview-Whiteboard-Prompts.md`.
- Status: Completed.

### 17) Last-day interview revision sheet

- Deliverable: compact final-day checklist of anchors, traps, metrics, and timed rehearsal sequence.
- Initial implementation: `Interview-Last-Day-Revision-Sheet.md`.
- Status: Completed.

### 18) Behavioral-to-technical bridge pack

- Deliverable: STAR-style scenarios that tie behavioral responses to technical decisions, metrics, and prevention lessons.
- Initial implementation: `Interview-Behavioral-Technical-Bridges.md`.
- Status: Completed.

### 19) Interview cheat-card quick reference

- Deliverable: compact table-first review sheet for high-frequency interview anchors, risks, and metrics.
- Initial implementation: `Interview-Cheat-Card.md`.
- Status: Completed.

### 20) Role-specific mock interview scripts

- Deliverable: guided role-based mock interview flows with question sets and scoring signals.
- Initial implementation: `Interview-Role-Mock-Scripts.md`.
- Status: Completed.

### 21) Mock interviewer scoring sheets

- Deliverable: fillable score templates with role-specific checkpoints and measurable improvement targets.
- Initial implementation: `Interview-Scoring-Sheets.md`.
- Status: Completed.

### 22) Mock interview runbook

- Deliverable: end-to-end mock session setup with timer cadence, scoring workflow, and retrospective loop.
- Initial implementation: `Interview-Mock-Runbook.md`.
- Status: Completed.

### 23) Interview readiness dashboard

- Deliverable: consolidated role-based readiness tracker with score trends, weak-signal register, and exit criteria.
- Initial implementation: `Interview-Readiness-Dashboard.md`.
- Status: Completed.

### 24) Docs folder interview reorganization

- Deliverable: grouped interview-focused docs under `Learning/docs/interview/` with hub index and recursive validators.
- Initial implementation: `interview/README.md` and validator updates for nested docs.
- Status: Completed.

### 25) Weekly interview sprint planner

- Deliverable: repeatable 7-day prep plan tied to role scripts, scoring, and dashboard updates.
- Initial implementation: `Interview-Weekly-Sprint-Planner.md`.
- Status: Completed.

### 26) Interview progress logging template

- Deliverable: session-by-session and weekly progress templates to track measurable interview improvement signals.
- Initial implementation: `Interview-Progress-Log.md`.
- Status: Completed.

### 27) Interview question-to-module routing map

- Deliverable: common interview prompts mapped to primary docs, runnable examples, and focused practice actions.
- Initial implementation: `Interview-Question-Module-Map.md`.
- Status: Completed.

### 28) .NET API to React integration scenario pack

- Deliverable: dedicated guide and examples for production-grade React SPA integration with ASP.NET Core APIs.
- Initial implementation: `DotNet-API-React.md` and `Learning/FrontEnd/ReactApiIntegrationExamples.cs`.
- Status: Completed.

### 29) .NET API to Vue integration scenario pack

- Deliverable: dedicated guide and examples for production-grade Vue SPA integration with ASP.NET Core APIs.
- Initial implementation: `DotNet-API-Vue.md` and `Learning/FrontEnd/VueApiIntegrationExamples.cs`.
- Status: Completed.

### 30) Enterprise hardening expansion for React/Vue API integrations

- Deliverable: security baseline, logging/observability standards, and structural architecture requirements added to both SPA integration guides.
- Initial implementation: expanded `DotNet-API-React.md`, `DotNet-API-Vue.md`, plus hardened snippets in `Learning/FrontEnd/ReactApiIntegrationExamples.cs` and `Learning/FrontEnd/VueApiIntegrationExamples.cs`.
- Status: Completed.

## Tracking

- Plan owner: Maintainers
- Review cadence: Bi-weekly
- Last updated: February 15, 2026

---

## Interview Answer Block

- 30-second answer: This topic covers Content roadmap planning and prioritization and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know roadmap planning and prioritization and I would just follow best practices."
- Strong answer: "For roadmap planning and prioritization, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply roadmap planning and prioritization in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
