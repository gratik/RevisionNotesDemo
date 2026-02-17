# Azure Deployment Topology for Observability

## Typical layout

- Azure API Management (optional ingress)
- App Service or AKS hosting .NET APIs
- Azure Service Bus for async workflows
- Application Insights + Log Analytics for telemetry storage/query
- Azure Monitor alerts for latency/error/reliability thresholds

## Non-negotiable controls

- Correlation id and W3C trace propagation
- SLO-driven alerting (error rate, p95 latency, queue lag)
- Runbooks linked directly from alerts
