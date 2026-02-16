# Common Pitfalls

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


