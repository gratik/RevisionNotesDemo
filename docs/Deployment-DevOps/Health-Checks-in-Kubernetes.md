# Health Checks in Kubernetes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Git workflows, CI/CD concepts, and container/runtime basics.
- Related examples: docs/Deployment-DevOps/README.md
> Subject: [Deployment-DevOps](../README.md)

## Health Checks in Kubernetes

### Liveness Probe

**Purpose**: Restart unhealthy container

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 30
  timeoutSeconds: 5
  failureThreshold: 3
```

### Readiness Probe

**Purpose**: Stop sending traffic to not-ready pod

```yaml
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
  timeoutSeconds: 3
  failureThreshold: 3
```

### Startup Probe

**Purpose**: Wait for slow-starting container

```yaml
startupProbe:
  httpGet:
    path: /health/startup
    port: 8080
  initialDelaySeconds: 0
  periodSeconds: 5
  failureThreshold: 60 # 5s * 60 = 5 min max startup time
```

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Health Checks in Kubernetes before implementation work begins.
- Keep boundaries explicit so Health Checks in Kubernetes decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Health Checks in Kubernetes in production-facing code.
- When performance, correctness, or maintainability depends on consistent Health Checks in Kubernetes decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Health Checks in Kubernetes as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Health Checks in Kubernetes is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Health Checks in Kubernetes are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Health Checks in Kubernetes is about delivery automation and runtime operational practices. It matters because pipeline quality determines release safety and iteration speed.
- Use it when building repeatable CI/CD with rollout safeguards.

2-minute answer:
- Start with the problem Health Checks in Kubernetes solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: deployment velocity vs risk controls and verification depth.
- Close with one failure mode and mitigation: shipping without rollback and observability guardrails.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Health Checks in Kubernetes but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Health Checks in Kubernetes, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Health Checks in Kubernetes and map it to one concrete implementation in this module.
- 3 minutes: compare Health Checks in Kubernetes with an alternative, then walk through one failure mode and mitigation.