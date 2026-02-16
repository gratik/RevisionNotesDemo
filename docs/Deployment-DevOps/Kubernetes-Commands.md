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


