# Best Practices

> Subject: [RealTime](../README.md)

## Best Practices

### ✅ Hub Design
- Keep hub methods simple and fast
- Don't do heavy work in hub methods
- Use background services for long-running tasks
- Return Task for all hub methods
- Use strongly-typed hubs when possible

### ✅ Groups
- Clean up groups on disconnect
- Use meaningful group names
- Consider group size (thousands per group is OK)
- Don't store state in hub (use database or cache)

### ✅ Security
- Always authenticate sensitive operations
- Validate all inputs
- Rate limit hub method invocations
- Don't trust client data

### ✅ Performance
- Use Redis for multi-server scenarios
- Limit message size
- Batch messages when possible
- Monitor connection count

---


