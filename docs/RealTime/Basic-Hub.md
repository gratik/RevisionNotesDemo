# Basic Hub

> Subject: [RealTime](../README.md)

## Basic Hub

### Creating a Hub

`csharp
// ✅ Simple chat hub
public class ChatHub : Hub
{
    // ✅ Clients can invoke this method
    public async Task SendMessage(string user, string message)
    {
        // ✅ Broadcast to ALL connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

// ✅ Register SignalR
builder.Services.AddSignalR();

// ✅ Map hub endpoint
app.MapHub<ChatHub>("/chatHub");
`

### JavaScript Client

`javascript
// ✅ Connect to hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// ✅ Listen for messages from server
connection.on("ReceiveMessage", (user, message) => {
    console.log(${user}: );
    // Update UI
});

// ✅ Start connection
await connection.start();
console.log("Connected!");

// ✅ Invoke server method
await connection.invoke("SendMessage", "Alice", "Hello World!");

// ✅ Stop connection
await connection.stop();
`

---


