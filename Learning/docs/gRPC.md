# gRPC for ASP.NET Core

## Overview

gRPC is a modern, high-performance RPC (Remote Procedure Call) framework that uses HTTP/2 and Protocol Buffers for efficient service-to-service communication.

---

## When to Use gRPC

✅ **Best for:**
- **Microservices** communication (inter-service calls)
- **Real-time** applications (streaming, live updates)
- **Low-latency** requirements
- **Internal APIs** (service mesh, backend services)
- **High-throughput** scenarios (1000s of requests/sec)
- **Type-safe contracts** between services

❌ **Avoid for:**
- Public web APIs (browser support limited)
- Simple CRUD APIs (REST more appropriate)
- Teams unfamiliar with Proto Buffers
- When JSON human-readability required

---

## gRPC vs REST

| Feature | gRPC | REST |
|---------|------|------|
| **Protocol** | HTTP/2 | HTTP/1.1 |
| **Format** | Protocol Buffers (binary) | JSON (text) |
| **Performance** | 7-10x faster | Baseline |
| **Streaming** | First-class support | Limited |
| **Browser Support** | gRPC-Web required | Native |
| **Type Safety** | Strong (generated code) | Weak (runtime) |
| **Payload Size** | Smaller (binary) | Larger (text) |
| **Use Case** | Internal services | Public APIs |

---

## Key Concepts

### Protocol Buffers (.proto files)

Define service contracts in `.proto` files:

```protobuf
syntax = "proto3";

service UserService {
  rpc GetUser (GetUserRequest) returns (UserResponse);
  rpc ListUsers (ListUsersRequest) returns (stream UserResponse);
}

message GetUserRequest {
  int32 id = 1;
}

message UserResponse {
  int32 id = 1;
  string name = 2;
  string email = 3;
}
```

### Service Implementation

Implement generated service base class:

```csharp
public class UserService : UserServiceBase
{
    public override async Task<UserResponse> GetUser(
        GetUserRequest request,
        ServerCallContext context)
    {
        var user = await _repo.GetAsync(request.Id);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
```

### Streaming Types

1. **Server Streaming**: Server sends multiple responses
2. **Client Streaming**: Client sends multiple requests
3. **Bi-directional Streaming**: Both send multiple messages

---

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

## Setup in ASP.NET Core

### Install Packages


```bash
dotnet add package Grpc.AspNetCore
dotnet add package Grpc.Tools
```

### Configure in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<UserService>();
app.Run();
```

---

## Example Files

📁 **Learning/WebAPI/gRPC/GrpcBestPractices.cs**
- Protocol Buffer definitions
- Service implementation patterns
- Client usage examples
- Error handling
- Streaming patterns
- Deadlines and cancellation

---

## Further Reading

- [Official gRPC Documentation](https://grpc.io/)
- [gRPC for ASP.NET Core](https://learn.microsoft.com/aspnet/core/grpc/)
- [Protocol Buffers Guide](https://protobuf.dev/)
