# Docker Compose

> Subject: [Deployment-DevOps](../README.md)

## Docker Compose

### What is Docker Compose?

Tool for defining and running **multi-container applications** with a single YAML file.

### Example: Web App + Database

```yaml
# docker-compose.yml
version: "3.8"

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


