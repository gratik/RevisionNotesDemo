# Kubernetes Commands

> Subject: [Deployment-DevOps](../README.md)

## Kubernetes Commands

```bash
# Apply configuration
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml

# View resources
kubectl get pods
kubectl get deployments
kubectl get services

# Describe resource (detailed info)
kubectl describe pod myapp-deployment-xyz123

# View logs
kubectl logs myapp-deployment-xyz123
kubectl logs -f myapp-deployment-xyz123  # Follow

# Scale deployment
kubectl scale deployment myapp-deployment --replicas=5

# Rolling update
kubectl set image deployment/myapp-deployment myapp=myregistry.azurecr.io/myapp:1.1.0

# Rollback
kubectl rollout undo deployment/myapp-deployment

# Execute command in pod
kubectl exec -it myapp-deployment-xyz123 -- bash

# Port forward (local testing)
kubectl port-forward myapp-deployment-xyz123 8080:8080

# Delete resources
kubectl delete deployment myapp-deployment
kubectl delete service myapp-service
```

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Kubernetes Commands before implementation work begins.
- Keep boundaries explicit so Kubernetes Commands decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Kubernetes Commands in production-facing code.
- When performance, correctness, or maintainability depends on consistent Kubernetes Commands decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Kubernetes Commands as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Kubernetes Commands is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Kubernetes Commands are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

