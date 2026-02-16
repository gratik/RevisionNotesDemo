# Best Practices

> Subject: [gRPC](../README.md)

## Best Practices

✅ **DO:**
- Use for internal service-to-service communication
- Implement deadlines and cancellation
- Use strongly-typed messages
- Leverage streaming for large datasets
- Handle `RpcException` properly
- Use middleware for cross-cutting concerns

❌ **DON''T:**
- Expose gRPC directly to public web (use gRPC-Web or API Gateway)
- Ignore cancellation tokens
- Return large objects in unary calls (use streaming)
- Mix REST and gRPC in same port (use different ports)

---


