# Best Practices

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


