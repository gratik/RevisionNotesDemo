# Azure Storage Strategy

> Subject: [Azure-Hosting](../README.md)

## Azure Storage Strategy

| Data Pattern | Azure Service |
| --- | --- |
| Object/file storage | Blob Storage |
| Relational transactions | Azure SQL |
| Globally distributed JSON | Cosmos DB |
| Cache/session state | Azure Cache for Redis |
| Async decoupling | Queue Storage / Service Bus |

### Storage Design Principles

- Select storage by access pattern, not team preference
- Configure retention/lifecycle to control cost
- Test restore workflows regularly
- Use geo-redundancy based on business RTO/RPO

---


