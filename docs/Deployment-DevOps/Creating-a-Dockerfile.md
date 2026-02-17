# Creating a Dockerfile

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Git workflows, CI/CD concepts, and container/runtime basics.
- Related examples: docs/Deployment-DevOps/README.md
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


## Interview Answer Block
30-second answer:
- Creating a Dockerfile is about delivery automation and runtime operational practices. It matters because pipeline quality determines release safety and iteration speed.
- Use it when building repeatable CI/CD with rollout safeguards.

2-minute answer:
- Start with the problem Creating a Dockerfile solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: deployment velocity vs risk controls and verification depth.
- Close with one failure mode and mitigation: shipping without rollback and observability guardrails.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Creating a Dockerfile but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Creating a Dockerfile, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Creating a Dockerfile and map it to one concrete implementation in this module.
- 3 minutes: compare Creating a Dockerfile with an alternative, then walk through one failure mode and mitigation.