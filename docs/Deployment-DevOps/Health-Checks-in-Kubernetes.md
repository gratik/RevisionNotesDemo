# Health Checks in Kubernetes

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


