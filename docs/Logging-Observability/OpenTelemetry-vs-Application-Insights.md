# OpenTelemetry vs Application Insights

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


## Roles

- OpenTelemetry: instrumentation standard and SDK pipeline for traces/metrics/logs.
- Application Insights: Azure backend for storing, correlating, querying, and alerting on telemetry.

## Recommended model

- Instrument with OpenTelemetry.
- Export to Application Insights (Azure Monitor exporter).
- Keep schema and service naming conventions consistent across services.

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

