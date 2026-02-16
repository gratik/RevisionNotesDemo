# Key Concepts

> Subject: [gRPC](../README.md)

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


