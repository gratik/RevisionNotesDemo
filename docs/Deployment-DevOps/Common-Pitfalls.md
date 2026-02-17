# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Git workflows, CI/CD concepts, and container/runtime basics.
- Related examples: docs/Deployment-DevOps/README.md
> Subject: [Deployment-DevOps](../README.md)

## Common Pitfalls

### ❌ **No Resource Limits**

```yaml
# ❌ BAD: No limits - can consume entire node
containers:
- name: myapp
  image: myapp:1.0.0

# ✅ GOOD: Defined limits
containers:
- name: myapp
  image: myapp:1.0.0
  resources:
    requests:
      memory: "256Mi"
      cpu: "250m"
    limits:
      memory: "512Mi"
      cpu: "500m"
```

### ❌ **Using `latest` Tag**

```bash
# ❌ BAD: Ambiguous, not reproducible
docker build -t myapp:latest .

# ✅ GOOD: Specific version
docker build -t myapp:1.2.3 .
```

### ❌ **Secrets in Environment Variables**

```yaml
# ❌ BAD: Secret in plain text
env:
- name: DB_PASSWORD
  value: "SuperSecret123"

# ✅ GOOD: Reference Kubernetes Secret
env:
- name: DB_PASSWORD
  valueFrom:
    secretKeyRef:
      name: db-secret
      key: password
```

### ❌ **No Health Checks**

```yaml
# ❌ BAD: Kubernetes doesn't know if app is healthy
containers:
- name: myapp
  image: myapp:1.0.0

# ✅ GOOD: Health checks configured
containers:
- name: myapp
  image: myapp:1.0.0
  livenessProbe:
    httpGet:
      path: /health/live
      port: 8080
  readinessProbe:
    httpGet:
      path: /health/ready
      port: 8080
```

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about delivery automation and runtime operational practices. It matters because pipeline quality determines release safety and iteration speed.
- Use it when building repeatable CI/CD with rollout safeguards.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: deployment velocity vs risk controls and verification depth.
- Close with one failure mode and mitigation: shipping without rollback and observability guardrails.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.