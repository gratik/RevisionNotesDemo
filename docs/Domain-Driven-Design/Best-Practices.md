# Best Practices

> Subject: [Domain-Driven-Design](../README.md)

## Best Practices

✅ **DO:**
- Use entities for objects with identity
- Use value objects for descriptive attributes
- Make value objects immutable
- Validate in domain, not application layer
- Use factory methods for creation
- Keep aggregates small
- Reference other aggregates by ID
- Use domain events for cross-aggregate updates

❌ **DON''T:**
- Create anemic domain models
- Allow public setters on entities
- Reference aggregates directly
- Create huge aggregates
- Bypass aggregate root to modify children
- Use primitive obsession (use value objects for domain concepts)

---


