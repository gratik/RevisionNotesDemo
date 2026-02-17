# Kubernetes Deployment

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Git workflows, CI/CD concepts, and container/runtime basics.
- Related examples: docs/Deployment-DevOps/README.md
> Subject: [Deployment-DevOps](../README.md)

## Kubernetes Deployment

### Basic Deployment YAML

```yaml
# deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-deployment
  namespace: production
spec:
  replicas: 3 # ✅ Run 3 instances
  selector:
    matchLabels:
      app: myapp
  template:
    metadata:
      labels:
        app: myapp
    spec:
      containers:
        - name: myapp
          image: myregistry.azurecr.io/myapp:1.0.0
          ports:
            - containerPort: 8080
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: "Production"
            - name: ConnectionStrings__DefaultConnection
              valueFrom:
                secretKeyRef:
                  name: db-secret
                  key: connection-string
          resources:
            requests:
              memory: "256Mi"
              cpu: "250m"
            limits:
              memory: "512Mi"
              cpu: "500m"
          livenessProbe:
            httpGet:
              path: /health/live
              port: 8080
            initialDelaySeconds: 10
            periodSeconds: 30
          readinessProbe:
            httpGet:
              path: /health/ready
              port: 8080
            initialDelaySeconds: 5
            periodSeconds: 10
```

### Service (Load Balancer)

```yaml
# service.yaml
apiVersion: v1
kind: Service
metadata:
  name: myapp-service
  namespace: production
spec:
  type: LoadBalancer
  selector:
    app: myapp
  ports:
    - protocol: TCP
      port: 80
      targetPort: 8080
```

### ConfigMap

```yaml
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: myapp-config
  namespace: production
data:
  appsettings.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information"
        }
      },
      "FeatureFlags": {
        "NewUI": true
      }
    }
```

### Secret

```yaml
# secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: db-secret
  namespace: production
type: Opaque
data:
  connection-string: U2VydmVyPWRiLnByb2R1Y3Rpb247RGF0YWJhc2U9bXlkYjtVc2VyPWFkbWlu # Base64 encoded
```

---


## Interview Answer Block
30-second answer:
- Kubernetes Deployment is about delivery automation and runtime operational practices. It matters because pipeline quality determines release safety and iteration speed.
- Use it when building repeatable CI/CD with rollout safeguards.

2-minute answer:
- Start with the problem Kubernetes Deployment solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: deployment velocity vs risk controls and verification depth.
- Close with one failure mode and mitigation: shipping without rollback and observability guardrails.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Kubernetes Deployment but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Kubernetes Deployment, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Kubernetes Deployment and map it to one concrete implementation in this module.
- 3 minutes: compare Kubernetes Deployment with an alternative, then walk through one failure mode and mitigation.