# Deployment and DevOps Essentials

**Last Updated**: 2026-02-14

Complete guide to deploying .NET applications with Docker, Kubernetes, and CI/CD pipelines. 
Covers containerization, orchestration, monitoring, and deployment strategies for production systems.

---

## Why DevOps Matters

**Development to Production Gap**:
- Code works on your machine → fails in production
- Manual deployments → human errors
- No versioning → rollback nightmares
- No automation → slow releases

**DevOps Solves**:
- Consistent environments (Docker)
- Automated deployments (CI/CD)
- Easy scaling (Kubernetes)
- Fast rollback (versioned containers)
- Infrastructure as code

---

## Docker Fundamentals

### What is Docker?

**Container** = Lightweight, standalone package with:
- Application code
- Runtime (e.g., .NET SDK)
- System tools and libraries
- Dependencies

**Benefits**:
- ✅ Same environment everywhere (dev, test, prod)
- ✅ Isolated processes
- ✅ Fast startup (vs VMs)
- ✅ Easy scaling
- ✅ Efficient resource usage

### Docker vs Virtual Machine

| Aspect | Docker Container | Virtual Machine |
|--------|------------------|-----------------|
| **Size** | MBs | GBs |
| **Startup** | Seconds | Minutes |
| **Isolation** | Process-level | Full OS |
| **Performance** | Near-native | Overhead |
| **Density** | Many per host | Few per host |

---

## Creating a Dockerfile

### Basic .NET Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

### Multi-Stage Build Explained

```dockerfile
# ✅ Stage 1: BUILD (Large image with SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
# Result: ~1GB image with build tools

# ✅ Stage 2: RUNTIME (Small image with runtime only)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .  # Copy only compiled output
ENTRYPOINT ["dotnet", "MyApp.dll"]
# Result: ~200MB final image

# ✅ Why multi-stage?
# - Smaller final image (SDK not included)
# - Faster deployments
# - Reduced attack surface
# - Build tools not in production
```

### Optimized Dockerfile with Caching

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# ✅ Copy only csproj first (changes less frequently)
COPY *.csproj ./
RUN dotnet restore
# Docker caches this layer - only re-runs if csproj changes

# ✅ Copy source code last (changes frequently)
COPY . ./
RUN dotnet publish -c Release -o out
# Only this layer rebuilds when code changes

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

---

## Docker Commands

### Build and Run

```bash
# Build image
docker build -t myapp:1.0.0 .

# Run container
docker run -d -p 8080:8080 --name myapp-container myapp:1.0.0

# Run with environment variables
docker run -d -p 8080:8080 \
    -e ConnectionStrings__DefaultConnection="Server=db;Database=mydb" \
    -e ASPNETCORE_ENVIRONMENT=Production \
    myapp:1.0.0

# Run with volume mount (for logs)
docker run -d -p 8080:8080 \
    -v /host/logs:/app/logs \
    myapp:1.0.0
```

### Managing Containers

```bash
# List running containers
docker ps

# View logs
docker logs myapp-container
docker logs -f myapp-container  # Follow logs

# Enter container shell
docker exec -it myapp-container bash

# Stop and remove
docker stop myapp-container
docker rm myapp-container

# View resource usage
docker stats myapp-container
```

### Image Management

```bash
# List images
docker images

# Tag image
docker tag myapp:1.0.0 myregistry.azurecr.io/myapp:1.0.0

# Push to registry
docker push myregistry.azurecr.io/myapp:1.0.0

# Pull from registry
docker pull myregistry.azurecr.io/myapp:1.0.0

# Remove image
docker rmi myapp:1.0.0

# Clean up unused images
docker image prune -a
```

---

## Docker Compose

### What is Docker Compose?

Tool for defining and running **multi-container applications** with a single YAML file.

### Example: Web App + Database

```yaml
# docker-compose.yml
version: '3.8'

services:
  # Database service
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: mydb
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: secret
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U admin"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Web application service
  web:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__DefaultConnection: "Host=db;Database=mydb;Username=admin;Password=secret"
      ASPNETCORE_ENVIRONMENT: Development
    depends_on:
      db:
        condition: service_healthy
    volumes:
      - ./logs:/app/logs

volumes:
  postgres-data:
```

### Docker Compose Commands

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f web

# Stop all services
docker-compose down

# Rebuild services
docker-compose build

# Scale service
docker-compose up -d --scale web=3

# View running services
docker-compose ps
```

---

## Kubernetes Fundamentals

### What is Kubernetes?

**Container orchestration platform** for:
- Deploying containerized apps
- Scaling automatically
- Self-healing (restart failed containers)
- Load balancing
- Rolling updates with zero downtime

### Core Concepts

| Concept | Description | Example |
|---------|-------------|---------|
| **Pod** | Smallest deployable unit (1+ containers) | Your app container |
| **Deployment** | Manages replica pods | 3 instances of your app |
| **Service** | Stable network endpoint for pods | Load balancer |
| **ConfigMap** | Configuration data | App settings |
| **Secret** | Sensitive data (passwords, keys) | Database password |
| **Ingress** | HTTP routing to services | Domain routing |
| **Namespace** | Virtual cluster isolation | dev/staging/prod |

---

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
  replicas: 3  # ✅ Run 3 instances
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
  connection-string: U2VydmVyPWRiLnByb2R1Y3Rpb247RGF0YWJhc2U9bXlkYjtVc2VyPWFkbWlu  # Base64 encoded
```

---

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
  failureThreshold: 60  # 5s * 60 = 5 min max startup time
```

---

## CI/CD Pipelines

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Build and Deploy

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

env:
  REGISTRY: myregistry.azurecr.io
  IMAGE_NAME: myapp

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
  docker-build-push:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.event_name == 'push'
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Login to Azure Container Registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ secrets.ACR_USERNAME }}
        password: ${{ secrets.ACR_PASSWORD }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=sha,prefix={{branch}}-
          type=semver,pattern={{version}}
    
    - name: Build and push
      uses: docker/build-push-action@v5
      with:
        context: .
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
  
  deploy-to-kubernetes:
    needs: docker-build-push
    runs-on: ubuntu-latest
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
    
    - name: Setup kubectl
      uses: azure/setup-kubectl@v3
    
    - name: Set Kubernetes context
      uses: azure/k8s-set-context@v3
      with:
        kubeconfig: ${{ secrets.KUBE_CONFIG }}
    
    - name: Deploy to Kubernetes
      run: |
        kubectl apply -f k8s/deployment.yaml
        kubectl apply -f k8s/service.yaml
        kubectl rollout status deployment/myapp-deployment
```

### Azure DevOps Pipeline

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
    - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dockerRegistryServiceConnection: 'MyACR'
  imageRepository: 'myapp'
  containerRegistry: 'myregistry.azurecr.io'
  tag: '$(Build.BuildId)'

stages:
- stage: Build
  jobs:
  - job: BuildAndTest
    steps:
    - task: UseDotNet@2
      inputs:
        version: '10.0.x'
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore'
      inputs:
        command: 'restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build'
      inputs:
        command: 'build'
        arguments: '--configuration $(buildConfiguration)'
    
    - task: DotNetCoreCLI@2
      displayName: 'Test'
      inputs:
        command: 'test'
        arguments: '--configuration $(buildConfiguration) --no-build'

- stage: Docker
  dependsOn: Build
  jobs:
  - job: BuildPushImage
    steps:
    - task: Docker@2
      displayName: 'Build and Push'
      inputs:
        containerRegistry: '$(dockerRegistryServiceConnection)'
        repository: '$(imageRepository)'
        command: 'buildAndPush'
        Dockerfile: '**/Dockerfile'
        tags: |
          $(tag)
          latest

- stage: Deploy
  dependsOn: Docker
  jobs:
  - deployment: DeployToK8s
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: KubernetesManifest@0
            inputs:
              action: 'deploy'
              manifests: |
                k8s/deployment.yaml
                k8s/service.yaml
              containers: '$(containerRegistry)/$(imageRepository):$(tag)'
```

---

## Deployment Strategies

### Rolling Update (Default)

**How**: Replace pods gradually (one by one)

```yaml
spec:
  replicas: 4
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1        # Max 1 extra pod during update
      maxUnavailable: 1  # Max 1 pod can be unavailable
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
    version: blue  # Change to 'green' to switch
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
    app: myapp  # Matches both stable and canary
  ports:
  - port: 80
    targetPort: 8080
```

✅ Low-risk testing
✅ Real user feedback
❌ Requires monitoring

---

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

## Related Files

- [HealthChecks/BasicHealthCheck.cs](../HealthChecks/BasicHealthCheck.cs) - Kubernetes health endpoints
- [Configuration/EnvironmentConfiguration.cs](../Configuration/EnvironmentConfiguration.cs) - Environment-specific settings

---

## See Also

- [Health Checks](HealthChecks.md) - Liveness and readiness probes
- [Configuration](Configuration.md) - Environment configuration
- [Logging and Observability](Logging-Observability.md) - Production monitoring
- [Security](Security.md) - Secrets management
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14