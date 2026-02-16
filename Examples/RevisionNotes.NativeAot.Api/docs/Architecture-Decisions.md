# Architecture Decisions - RevisionNotes.NativeAot.Api

## WHAT IS THIS?
This is a Native AOT-focused API demo using trimming-friendly patterns and lightweight middleware-based security.

## WHY IT MATTERS?
- Native AOT can reduce startup time and memory footprint for cloud workloads.
- AOT requires avoiding reflection-heavy patterns unless explicitly configured.

## WHEN TO USE?
- When fast startup and small runtime footprint are key goals.
- When deployment environment benefits from ahead-of-time compilation.

## WHEN NOT TO USE?
- When dynamic runtime extensibility and reflection-heavy libraries are required.
- When team/tooling is not ready to manage trimming/AOT constraints.

## REAL-WORLD EXAMPLE?
A latency-sensitive edge API for IoT devices where startup speed and binary size are critical.

## ADR-01: Use `CreateSlimBuilder` and minimal features
- Decision: Build API with slim hosting model and simple endpoints.
- Why: Keeps runtime surface small and AOT-friendly.
- Tradeoff: Fewer framework conveniences by default.

## ADR-02: Prefer API-key middleware over complex auth stack
- Decision: Use deterministic header-based auth for demo security.
- Why: Avoids reflection-heavy auth dependencies in this AOT sample.
- Tradeoff: API-key auth is less feature-rich than JWT/OIDC.

## ADR-03: Enable AOT + invariant globalization in project settings
- Decision: Set `PublishAot=true` and `InvariantGlobalization=true`.
- Why: Aligns build output with AOT deployment goals.
- Tradeoff: Some globalization scenarios require additional configuration.
