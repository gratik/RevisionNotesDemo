# Docker Commands

> Subject: [Deployment-DevOps](../README.md)

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


