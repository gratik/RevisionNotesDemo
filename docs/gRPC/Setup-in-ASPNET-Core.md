# Setup in ASP.NET Core

> Subject: [gRPC](../README.md)

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


