// ==============================================================================
// SIGNALR BASICS - Real-Time Web Communication
// ==============================================================================
// WHAT IS THIS?
// -------------
// SignalR hubs for real-time, bidirectional communication.
//
// WHY IT MATTERS
// --------------
// ✅ Enables live updates without polling
// ✅ Supports groups and targeted messaging
//
// WHEN TO USE
// -----------
// ✅ Chat, dashboards, notifications, collaboration
// ✅ Real-time status updates to clients
//
// WHEN NOT TO USE
// ---------------
// ❌ Simple request-response APIs with no real-time needs
// ❌ Large file transfers (use regular endpoints)
//
// REAL-WORLD EXAMPLE
// ------------------
// Broadcast order status updates to clients.
// ==============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RevisionNotesDemo.RealTime;

/// <summary>
/// EXAMPLE 1: BASIC HUB - Server and Client Communication
/// 
/// THE PROBLEM:
/// Traditional HTTP = client pulls from server.
/// Need server to push updates to clients immediately.
/// 
/// THE SOLUTION:
/// SignalR Hub = persistent connection for bidirectional messaging.
/// 
/// WHY IT MATTERS:
/// - Real-time notifications
/// - No polling (efficient)
/// - Automatic protocol fallback (WebSockets → SSE → Long Polling)
/// </summary>
public class BasicHubExamples
{
    // ✅ GOOD: Create a Hub
    public class ChatHub : Hub
    {
        // ✅ Client can invoke this method
        public async Task SendMessage(string user, string message)
        {
            // ✅ Broadcast to ALL connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // ✅ Clients can join a named group
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // ✅ Notify group members
            await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId);
        }

        // ✅ Send message to specific group
        public async Task SendGroupMessage(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }
    }

    // ✅ Configure SignalR in Program.cs
    public static void ConfigureSignalR(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;  // ⚠️ Development only
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        });
    }

    public static void MapHub(WebApplication app)
    {
        // ✅ Map hub to endpoint
        app.MapHub<ChatHub>("/chatHub");
    }

    // CLIENT (JavaScript):
    // const connection = new signalR.HubConnectionBuilder()
    //     .withUrl("/chatHub")
    //     .build();
    //
    // // Listen for messages
    // connection.on("ReceiveMessage", (user, message) => {
    //     console.log(`${user}: ${message}`);
    // });
    //
    // // Send message
    // await connection.invoke("SendMessage", "John", "Hello!");
    //
    // CLIENT (C# / Blazor):
    // var connection = new HubConnectionBuilder()
    //     .WithUrl("https://localhost:5001/chatHub")
    //     .Build();
    // 
    // connection.On<string, string>("ReceiveMessage", (user, message) =>
    // {
    //     Console.WriteLine($"{user}: {message}");
    // });
    //
    // await connection.StartAsync();
    // await connection.InvokeAsync("SendMessage", "John", "Hello!");
}

/// <summary>
/// EXAMPLE 2: GROUPS - Targeted Messaging
/// 
/// THE PROBLEM:
/// Don't want to send messages to ALL clients.
/// Need chat rooms, user-specific notifications.
/// 
/// THE SOLUTION:
/// Groups organize connections for targeted messaging.
/// </summary>
public class GroupsExamples
{
    public class NotificationHub : Hub
    {
        // ✅ Client joins room on connection
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // ✅ Notify others in room
            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        // ✅ Client leaves room
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
        }

        // ✅ Send to specific room
        public async Task SendToRoom(string roomId, string message)
        {
            await Clients.Group(roomId).SendAsync("RoomMessage", message);
        }

        // ✅ Connection lifecycle
        public override async Task OnConnectedAsync()
        {
            // Client connected
            var userId = Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                // ✅ Auto-join personal notification group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Client disconnected (cleanup if needed)
            await base.OnDisconnectedAsync(exception);
        }
    }

    // ✅ GOOD: Service to send notifications from anywhere
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // ✅ Send to specific user
        public async Task NotifyUser(string userId, string message)
        {
            await _hubContext.Clients.Group($"user-{userId}")
                .SendAsync("Notification", message);
        }

        // ✅ Send to all users
        public async Task NotifyAll(string message)
        {
            await _hubContext.Clients.All.SendAsync("Notification", message);
        }

        // ✅ Send to specific connection
        public async Task NotifyConnection(string connectionId, string message)
        {
            await _hubContext.Clients.Client(connectionId)
                .SendAsync("Notification", message);
        }
    }
}

/// <summary>
/// EXAMPLE 3: STRONGLY-TYPED HUBS - Type Safety
/// 
/// THE PROBLEM:
/// SendAsync uses strings - typos cause runtime errors.
/// 
/// THE SOLUTION:
/// Strongly-typed hub with interface.
/// </summary>
public class StronglyTypedHubExamples
{
    // ✅ GOOD: Define client interface
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
        Task UserJoined(string connectionId);
        Task UserLeft(string connectionId);
        Task TypingStarted(string user);
    }

    // ✅ GOOD: Strongly-typed hub
    public class StrongChatHub : Hub<IChatClient>
    {
        // ✅ IntelliSense and compile-time checking
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);  // ✅ Type-safe!
        }

        public async Task StartTyping(string user)
        {
            await Clients.Others.TypingStarted(user);  // ✅ IntelliSense works
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.UserJoined(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.UserLeft(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    // ✅ Use with IHubContext
    public class ChatService
    {
        private readonly IHubContext<StrongChatHub, IChatClient> _hubContext;

        public ChatService(IHubContext<StrongChatHub, IChatClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastSystemMessage(string message)
        {
            // ✅ Type-safe client calls
            await _hubContext.Clients.All.ReceiveMessage("System", message);
        }
    }
}

/// <summary>
/// EXAMPLE 4: AUTHENTICATION - Secure SignalR Endpoints
/// 
/// THE PROBLEM:
/// Don't want anonymous users accessing hubs.
/// 
/// THE SOLUTION:
/// [Authorize] attribute on hub or methods.
/// </summary>
public class AuthenticationExamples
{
    // ✅ GOOD: Require authentication for entire hub
    [Authorize]
    public class SecureHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var user = Context.User?.Identity?.Name ?? "Anonymous";

            // ✅ Access user context
            var userId = Context.UserIdentifier;

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // ✅ Require specific role
        [Authorize(Roles = "Admin")]
        public async Task BroadcastAnnouncement(string announcement)
        {
            await Clients.All.SendAsync("Announcement", announcement);
        }

        // ✅ Custom authorization
        [Authorize(Policy = "PremiumUser")]
        public async Task AccessPremiumFeature()
        {
            await Clients.Caller.SendAsync("PremiumFeature", "data");
        }
    }

    // ✅ Configure authentication
    public static void ConfigureAuth(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                // ✅ SignalR sends token in query string
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                        {
                            context.Token = accessToken;  // ✅ Extract from query
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("PremiumUser", policy =>
                policy.RequireClaim("Subscription", "Premium"));
        });
    }

    // CLIENT (JavaScript with auth):
    // const connection = new signalR.HubConnectionBuilder()
    //     .withUrl("/secureHub", {
    //         accessTokenFactory: () => getUserToken()  // ✅ Provide token
    //     })
    //     .build();
}

/// <summary>
/// EXAMPLE 5: CONNECTION MANAGEMENT - Tracking Online Users
/// </summary>
public class ConnectionManagementExamples
{
    // ✅ GOOD: Track connections in singleton service
    public class ConnectionTracker
    {
        private readonly Dictionary<string, List<string>> _userConnections = new();
        private readonly object _lock = new();

        public void AddConnection(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_userConnections.ContainsKey(userId))
                    _userConnections[userId] = new List<string>();

                _userConnections[userId].Add(connectionId);
            }
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                        _userConnections.Remove(userId);
                }
            }
        }

        public List<string> GetOnlineUsers()
        {
            lock (_lock)
            {
                return _userConnections.Keys.ToList();
            }
        }

        public bool IsUserOnline(string userId)
        {
            lock (_lock)
            {
                return _userConnections.ContainsKey(userId);
            }
        }
    }

    // ✅ Use in hub
    public class PresenceHub : Hub
    {
        private readonly ConnectionTracker _tracker;

        public PresenceHub(ConnectionTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                _tracker.AddConnection(userId, Context.ConnectionId);

                // ✅ Broadcast online users
                var onlineUsers = _tracker.GetOnlineUsers();
                await Clients.All.SendAsync("OnlineUsers", onlineUsers);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                _tracker.RemoveConnection(userId, Context.ConnectionId);

                var onlineUsers = _tracker.GetOnlineUsers();
                await Clients.All.SendAsync("OnlineUsers", onlineUsers);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    // ✅ Register as singleton
    public static void Register(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ConnectionTracker>();
    }
}

/// <summary>
/// EXAMPLE 6: REAL-WORLD PATTERNS - Production Use Cases
/// </summary>
public class RealWorldPatternsExamples
{
    // ✅ GOOD: Dashboard with live updates
    public class DashboardHub : Hub
    {
        private readonly IMetricsService _metricsService;

        public DashboardHub(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        public async Task SubscribeToMetrics()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard-subscribers");
        }

        // Background service calls this periodically
        public async Task PushMetricsUpdate(MetricsData data)
        {
            await Clients.Group("dashboard-subscribers").SendAsync("MetricsUpdate", data);
        }
    }

    // ✅ Background service pushes updates
    public class MetricsUpdater : BackgroundService
    {
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IMetricsService _metricsService;

        public MetricsUpdater(
            IHubContext<DashboardHub> hubContext,
            IMetricsService metricsService)
        {
            _hubContext = hubContext;
            _metricsService = metricsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var metrics = await _metricsService.GetCurrentMetricsAsync();

                // ✅ Push to all dashboard subscribers
                await _hubContext.Clients.Group("dashboard-subscribers")
                    .SendAsync("MetricsUpdate", metrics, stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    // ✅ GOOD: Order status notifications
    public class OrderHub : Hub
    {
        public async Task TrackOrder(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }
    }

    public class OrderService
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            // Update database...

            // ✅ Notify clients tracking this order
            await _hubContext.Clients.Group($"order-{orderId}")
                .SendAsync("OrderStatusChanged", orderId, status);
        }
    }

    public interface IMetricsService
    {
        Task<MetricsData> GetCurrentMetricsAsync();
    }

    public class MetricsData
    {
        public int ActiveUsers { get; set; }
        public decimal Revenue { get; set; }
    }
}

// SUMMARY - SignalR Best Practices:
//
// ✅ DO:
// - Use strongly-typed hubs (Hub<TClient>)
// - Implement OnConnectedAsync/OnDisconnectedAsync
// - Use groups for targeted messaging
// - Authenticate with [Authorize]
// - Track connections in singleton service
// - Handle reconnection on client
// - Use IHubContext to send from services
// - Scale out with Redis backplane or Azure SignalR Service
//
// ❌ DON'T:
// - Store state in hub instance (transient lifetime)
// - Do heavy processing in hub methods (offload to services)
// - Forget to handle disconnections
// - Send huge payloads (>32KB limit)
// - Use SignalR for simple REST APIs
//
// CLIENT TARGETS:
// - Clients.All: All connected clients
// - Clients.Caller: Current client only
// - Clients.Others: All except caller
// - Clients.Client(id): Specific connection
// - Clients.User(userId): All connections for user
// - Clients.Group(name): All in group
// - Clients.OthersInGroup(name): Others in group
//
// SCALE-OUT:
// For multiple servers, need backplane:
// - Azure SignalR Service (recommended)
// - Redis backplane
// - SQL Server backplane
//
// USE CASES:
// ✅ Chat applications
// ✅ Live dashboards
// ✅ Real-time notifications
// ✅ Collaborative editing
// ✅ Live sports scores
// ✅ Stock tickers
// ✅ Gaming
// ❌ REST APIs (use regular controllers)
// ❌ File uploads (use regular endpoints)
