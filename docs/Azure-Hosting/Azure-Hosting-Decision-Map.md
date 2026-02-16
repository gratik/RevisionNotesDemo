# Azure Hosting Decision Map

> Subject: [Azure-Hosting](../README.md)

## Azure Hosting Decision Map

| Workload Type | Best First Choice | Why |
| --- | --- | --- |
| Simple web API in container | App Service for Containers | Fastest setup, managed platform |
| Event-driven bursty processing | Azure Functions (optionally Dockerized) | Auto-scale and pay-per-use |
| Microservices with moderate complexity | Azure Container Apps | Revisions, scaling, lower ops overhead |
| Complex platform with custom networking | AKS | Full Kubernetes control |

---


