# Deployment Strategies

> Subject: [Deployment-DevOps](../README.md)

## Deployment Strategies

### Rolling Update (Default)

**How**: Replace pods gradually (one by one)

```yaml
spec:
  replicas: 4
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1 # Max 1 extra pod during update
      maxUnavailable: 1 # Max 1 pod can be unavailable
```

**Timeline**:

1. Start 1 new pod (v2)
2. Terminate 1 old pod (v1)
3. Repeat until all pods are v2

✅ Zero downtime
✅ Gradual rollout
❌ Both versions running simultaneously

### Blue-Green Deployment

**How**: Run two identical environments (blue=old, green=new), switch traffic

```yaml
# Blue deployment (v1.0)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-blue
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
      version: blue
  template:
    metadata:
      labels:
        app: myapp
        version: blue
    spec:
      containers:
        - name: myapp
          image: myregistry.azurecr.io/myapp:1.0.0
---
# Green deployment (v2.0)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-green
spec:
  replicas: 3
  selector:
    matchLabels:
      app: myapp
      version: green
  template:
    metadata:
      labels:
        app: myapp
        version: green
    spec:
      containers:
        - name: myapp
          image: myregistry.azurecr.io/myapp:2.0.0
---
# Service routes to blue OR green
apiVersion: v1
kind: Service
metadata:
  name: myapp-service
spec:
  selector:
    app: myapp
    version: blue # Change to 'green' to switch
  ports:
    - port: 80
      targetPort: 8080
```

✅ Instant rollback (change selector)
✅ No mixed versions
❌ 2x resource usage

### Canary Deployment

**How**: Route small percentage to new version, gradually increase

```yaml
# Stable version (90% traffic)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-stable
spec:
  replicas: 9
  selector:
    matchLabels:
      app: myapp
      track: stable
---
# Canary version (10% traffic)
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-canary
spec:
  replicas: 1
  selector:
    matchLabels:
      app: myapp
      track: canary
---
# Service routes to both
apiVersion: v1
kind: Service
metadata:
  name: myapp-service
spec:
  selector:
    app: myapp # Matches both stable and canary
  ports:
    - port: 80
      targetPort: 8080
```

✅ Low-risk testing
✅ Real user feedback
❌ Requires monitoring

---


