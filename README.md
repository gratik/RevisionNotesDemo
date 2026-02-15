# C# & OOP Revision Notes - Comprehensive Demonstration Project

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Build](https://img.shields.io/badge/build-passing-brightgreen)](link)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

A comprehensive .NET learning project that demonstrates modern C#, architecture patterns, APIs, security, performance, observability, and delivery practices.

## Quick Start

```bash
cd RevisionNotesDemo
dotnet restore
dotnet build
dotnet run
```

## Where To Start

- Guided study sequence: [Learning Path](Learning/docs/Learning-Path.md)
- Full doc index with levels and time estimates: [Docs Index](Learning/docs/README.md)
- Architecture and conventions: [Architecture Guide](Learning/Architecture/README.md) and [Architecture ADRs](Learning/Architecture/adr/README.md)

## Core Docs

- Foundations: [OOP Principles](Learning/docs/OOP-Principles.md), [Core C#](Learning/docs/Core-CSharp.md), [Modern C#](Learning/docs/Modern-CSharp.md)
- Backend/API: [Web API & MVC](Learning/docs/Web-API-MVC.md), [Data Access](Learning/docs/Data-Access.md), [Security](Learning/docs/Security.md), [Testing](Learning/docs/Testing.md)
- Scale and delivery: [Design Patterns](Learning/docs/Design-Patterns.md), [Distributed Consistency](Learning/docs/Distributed-Consistency.md), [Deployment and DevOps](Learning/docs/Deployment-DevOps.md), [Azure Hosting](Learning/docs/Azure-Hosting.md)
- Integrated example: [End-to-End Case Study](Learning/docs/End-to-End-Case-Study.md)

## Project Structure and Inventory

Detailed folder inventory, organization principles, examples, and project stats are maintained in:

- [Project Structure and Inventory](Learning/docs/Project-Structure-Inventory.md)

## Quality Gates

- Docs links/references/orphan checks: `scripts/validate-docs.sh`
- Docs metadata checks: `scripts/validate-doc-metadata.sh`
- Content structure checks: `scripts/validate-content-structure.sh`
- Warning gate (`-warnaserror`) for app + unit test projects in CI

## Build and Test

```bash
# Build app
 dotnet build RevisionNotesDemo.csproj

# Run demos
 dotnet run

# Run unit tests
 dotnet test Tests/RevisionNotesDemo.UnitTests/RevisionNotesDemo.UnitTests.csproj
```

## Contributing

1. Create a branch and add/update content.
2. Keep topic README sections consistent with `Learning/docs/Topic-README-Template.md`.
3. Run validation scripts before opening a PR.
4. Keep suppressions scoped and documented in `Learning/docs/Suppression-Audit.md`.

## Quick Links

- [Project Summary](PROJECT_SUMMARY.md)
- [Content Improvement Plan](Learning/docs/Content-Improvement-Plan.md)
- [Content Coverage](Learning/docs/Content-Coverage.md)
- [Build Warning Triage](Learning/docs/Build-Warning-Triage.md)
- [Suppression Audit](Learning/docs/Suppression-Audit.md)
- [Project Structure and Inventory](Learning/docs/Project-Structure-Inventory.md)

## License

MIT License

## Acknowledgments

Based on C# and OO Revision Notes by Barry Compuesto.
