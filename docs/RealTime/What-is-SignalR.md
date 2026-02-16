# What is SignalR?

> Subject: [RealTime](../README.md)

## What is SignalR?

**SignalR** = Persistent connection for server-to-client push messaging

### Traditional HTTP vs SignalR

| | HTTP | SignalR |
|---|------|---------|
| **Direction** | Client → Server | Bidirectional |
| **Connection** | Request/Response | Persistent |
| **Server Push** | ❌ (need polling) | ✅ Native |
| **Use Cases** | REST APIs | Chat, notifications, live updates |

### Transport Fallback

SignalR automatically chooses best transport:
1. **WebSockets** (best) - Full duplex, real-time
2. **Server-Sent Events** (fallback) - Server to client only
3. **Long Polling** (last resort) - Simulates real-time

---


