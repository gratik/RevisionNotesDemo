# Configuration

> Subject: [RealTime](../README.md)

## Configuration

### SignalR Options

`csharp
builder.Services.AddSignalR(options =>
{
    // ✅ Enable detailed errors (development only)
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    
    // ✅ Keep-alive interval
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    
    // ✅ Client timeout
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    
    // ✅ Maximum message size
    options.MaximumReceiveMessageSize = 32 * 1024;  // 32 KB
    
    // ✅ Handshake timeout
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
`

### Scaling with Redis

`csharp
// ✅ Scale across multiple servers with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp";
    });
`

---


