# Azure Deployment Topology for Observability

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


## Typical layout

- Azure API Management (optional ingress)
- App Service or AKS hosting .NET APIs
- Azure Service Bus for async workflows
- Application Insights + Log Analytics for telemetry storage/query
- Azure Monitor alerts for latency/error/reliability thresholds

## Non-negotiable controls

- Correlation id and W3C trace propagation
- SLO-driven alerting (error rate, p95 latency, queue lag)
- Runbooks linked directly from alerts

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

