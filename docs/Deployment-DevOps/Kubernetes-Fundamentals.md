# Kubernetes Fundamentals

> Subject: [Deployment-DevOps](../README.md)

## Kubernetes Fundamentals

### What is Kubernetes?

**Container orchestration platform** for:

- Deploying containerized apps
- Scaling automatically
- Self-healing (restart failed containers)
- Load balancing
- Rolling updates with zero downtime

### Core Concepts

| Concept        | Description                              | Example                 |
| -------------- | ---------------------------------------- | ----------------------- |
| **Pod**        | Smallest deployable unit (1+ containers) | Your app container      |
| **Deployment** | Manages replica pods                     | 3 instances of your app |
| **Service**    | Stable network endpoint for pods         | Load balancer           |
| **ConfigMap**  | Configuration data                       | App settings            |
| **Secret**     | Sensitive data (passwords, keys)         | Database password       |
| **Ingress**    | HTTP routing to services                 | Domain routing          |
| **Namespace**  | Virtual cluster isolation                | dev/staging/prod        |

---


