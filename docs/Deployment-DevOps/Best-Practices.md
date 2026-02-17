# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Git workflows, CI/CD concepts, and container/runtime basics.
- Related examples: docs/Deployment-DevOps/README.md
> Subject: [Deployment-DevOps](../README.md)

## Best Practices

### Docker

- ✅ **Multi-stage builds** for smaller images
- ✅ **Layer caching** (copy csproj before code)
- ✅ **Non-root user** for security
- ✅ **Health checks** in Dockerfile
- ✅ **Specific tags** (not `latest`)
- ✅ **.dockerignore** file (exclude obj, bin)
- ✅ **Environment variables** for configuration
- ❌ Don't store secrets in images

### Kubernetes

- ✅ **Resource limits** on all containers
- ✅ **Health probes** (liveness, readiness, startup)
- ✅ **ConfigMaps/Secrets** for configuration
- ✅ **Namespaces** for isolation
- ✅ **Rolling updates** with maxSurge/maxUnavailable
- ✅ **Horizontal Pod Autoscaler** for scaling
- ✅ **Network policies** for security
- ❌ Don't run as root in containers

### CI/CD

- ✅ **Automated testing** before deployment
- ✅ **Code scanning** (security vulnerabilities)
- ✅ **Version tagging** (semantic versioning)
- ✅ **Environment promotion** (dev → staging → prod)
- ✅ **Rollback plan** for failures
- ✅ **Monitoring** after deployment
- ❌ Don't deploy without tests passing

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Best Practices is about delivery automation and runtime operational practices. It matters because pipeline quality determines release safety and iteration speed.
- Use it when building repeatable CI/CD with rollout safeguards.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: deployment velocity vs risk controls and verification depth.
- Close with one failure mode and mitigation: shipping without rollback and observability guardrails.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.