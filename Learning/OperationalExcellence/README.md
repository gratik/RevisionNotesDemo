# Operational Excellence Guide

## Learning goals

- Build reliable operational practices for production .NET services.
- Standardize incident response, support handoffs, and measurable reliability targets.
- Reduce mean time to detect and recover through runbooks and alert quality.

## Prerequisites

- Observability basics
- Resilience patterns and deployment workflows

## Runnable examples

- IncidentResponseRunbook.cs - Incident triage flow and escalation checkpoints
- SLOSLIErrorBudget.cs - Reliability targets and release gating decisions
- OnCallAndEscalationPatterns.cs - On-call operations and escalation policy design

Run examples from the project root:

```bash
dotnet run --section operations
```

## Bad vs good examples summary

- Bad: alert noise, unclear ownership, and no defined mitigation playbook.
- Good: actionable alerts, clear roles, and tested rollback/runbook flows.
- Why it matters: incident speed and consistency directly affect customer outcomes.

## Related docs

- [Primary](../../docs/Operational-Excellence.md)
- [Related](../../docs/Logging-Observability.md)
