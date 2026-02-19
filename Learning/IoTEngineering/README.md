# IoT Engineering Guide

## Learning goals

- Understand IoT connectivity, ingestion, and command patterns in .NET on Azure.
- Design reliable device-to-cloud and cloud-to-device workflows at scale.
- Apply production-safe patterns for provisioning, edge/offline, and telemetry pipelines.

## Prerequisites

- Cloud messaging fundamentals
- Basic async and background worker knowledge

## Runnable examples

- AzureIoTHubPatterns.cs - IoT Hub architecture and routing decisions
- MQTTAndAMQPPatterns.cs - Protocol selection and transport tradeoffs
- DeviceProvisioningAndIdentity.cs - Device identity lifecycle and secure onboarding
- DeviceTwinAndDirectMethods.cs - Desired/reported properties and command handling
- IoTEdgeAndOfflinePatterns.cs - Edge buffering and store-and-forward reliability
- TelemetryIngestionPipeline.cs - End-to-end ingestion, enrichment, and alert signals

Run examples from the project root:

```bash
dotnet run --section iot
```

## Bad vs good examples summary

- Bad: unbounded ingress, weak identity controls, and no replay strategy.
- Good: partition-aware ingestion, idempotent processing, and explicit incident signals.
- Why it matters: IoT platforms fail under load or outages without disciplined reliability patterns.

## Related docs

- [Primary](../../docs/IoT-Engineering.md)
- [Related](../../docs/Azure-Hosting.md)
