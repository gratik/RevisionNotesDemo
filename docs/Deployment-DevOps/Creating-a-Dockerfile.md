# Creating a Dockerfile

> Subject: [Deployment-DevOps](../README.md)

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


