# Docker Hosting Patterns

> Subject: [Azure-Hosting](../README.md)

## Docker Hosting Patterns

### Recommended Flow

1. Build immutable Docker image in CI
2. Scan image for vulnerabilities
3. Push to Azure Container Registry
4. Deploy by versioned tag/digest
5. Validate health and shift traffic

### Key Practices

- Use multi-stage Dockerfiles for small runtime images
- Run containers with non-root users where possible
- Store config/secrets in Key Vault and inject at runtime
- Pin base images and apply patching cadence

---


