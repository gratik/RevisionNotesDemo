# Docker Commands

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Docker Commands before implementation work begins.
- Keep boundaries explicit so Docker Commands decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Docker Commands in production-facing code.
- When performance, correctness, or maintainability depends on consistent Docker Commands decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Docker Commands as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Docker Commands is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Docker Commands are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

