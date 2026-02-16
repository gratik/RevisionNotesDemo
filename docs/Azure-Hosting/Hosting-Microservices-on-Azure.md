# Hosting Microservices on Azure

> Subject: [Azure-Hosting](../README.md)

## Hosting Microservices on Azure

### Core Building Blocks

- **Gateway**: API Management / ingress
- **Compute**: AKS or Container Apps
- **Messaging**: Service Bus / Event Grid
- **Identity**: Managed Identity + Entra ID
- **Telemetry**: Azure Monitor + OpenTelemetry

### Operational Baseline

- Independent deploy per service
- Canary or blue/green rollout
- SLO-backed alerts and runbooks
- Rollback by immutable artifact version

---


