# Microservices Guide

## Learning goals

- Understand the main concepts covered in Microservices.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- REST and asynchronous messaging basics
- Service boundary design awareness

## Runnable examples

- APIGatewayPatterns.cs - Topic implementation and demonstration code
- DistributedCachingAndCoherence.cs - Topic implementation and demonstration code
- DistributedConsistencyPatterns.cs - Topic implementation and demonstration code
- DistributedTransactionsAndSaga.cs - Topic implementation and demonstration code
- EventDrivenArchitecture.cs - Topic implementation and demonstration code
- MicroservicesIntroduction.cs - Topic implementation and demonstration code
- MonolithVsMicroservices.cs - Topic implementation and demonstration code
- ServiceCommunicationPatterns.cs - Topic implementation and demonstration code
- ServiceDiscoveryPatterns.cs - Topic implementation and demonstration code
- ServiceMeshBasics.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../../docs/Message-Architecture.md)
- [Related](../../docs/Distributed-Consistency.md)

