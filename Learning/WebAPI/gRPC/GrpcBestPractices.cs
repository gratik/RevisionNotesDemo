// ==============================================================================
// gRPC BEST PRACTICES
// ==============================================================================
// PURPOSE: Modern RPC framework for microservices communication
// WHY: 10x faster than REST, type-safe contracts, bi-directional streaming
// USE WHEN: Microservices, real-time, low-latency, internal APIs
// ==============================================================================
// WHAT IS gRPC?
// gRPC (Google Remote Procedure Call) is a high-performance, open-source
// framework for building service-to-service communication. It uses HTTP/2 for
// transport, Protocol Buffers (protobuf) for serialization, and generates
// strongly-typed client/server code automatically from .proto contract files.
//
// WHY IT MATTERS:
// • PERFORMANCE: 7-10x faster than REST/JSON (binary protocol, HTTP/2 multiplexing)
// • TYPE SAFETY: Compile-time type checking prevents runtime errors
// • STREAMING: First-class support for real-time bidirectional communication
// • POLYGLOT: Works across C#, Go, Java, Python, Node.js seamlessly
// • CONTRACT-FIRST: Proto files serve as the source of truth for APIs
//
// WHEN TO USE:
// ✅ Service-to-service communication in microservices
// ✅ Real-time features (chat, notifications, live data)
// ✅ High-throughput scenarios (1000s requests/sec)
// ✅ Low-latency requirements (gaming, trading, IoT)
// ✅ Internal APIs behind API gateway
//
// WHEN NOT TO USE:
// ❌ Public-facing web APIs (limited browser support without gRPC-Web)
// ❌ Simple CRUD where REST is sufficient
// ❌ Teams unfamiliar with Protocol Buffers
// ❌ When human-readable JSON is required for debugging
//
// REAL-WORLD EXAMPLE:
// Netflix uses gRPC for inter-service communication between 500+ microservices,
// reducing latency by 80% compared to REST and saving millions in infrastructure.
// ==============================================================================

namespace RevisionNotesDemo.WebAPI.gRPC;

/// <summary>
/// gRPC service patterns demonstrating good vs bad practices.
/// 
/// THE PROBLEM:
/// REST APIs have overhead (JSON parsing, HTTP headers, text encoding).
/// For service-to-service communication, this adds latency.
/// 
/// THE SOLUTION:
/// gRPC uses HTTP/2, Protocol Buffers (binary), and generated code.
/// Results: 7-10x faster, smaller payloads, type safety.
/// 
/// WHY IT MATTERS:
/// - Microservices need fast, reliable communication
/// - Type-safe contracts prevent runtime errors
/// - Streaming enables real-time scenarios
/// - Cross-platform (C#, Go, Java, Python, etc.)
/// </summary>
public static class GrpcBestPractices
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== gRPC BEST PRACTICES ===\n");
        Console.WriteLine("gRPC examples demonstrate service definitions and patterns.\n");
        
        Example1_ProtoDefinition();
        Example2_ServiceImplementation();
        Example3_ClientUsage();
        Example4_ErrorHandling();
        Example5_Streaming();
        Example6_DeadlinesAndCancellation();
        
        Console.WriteLine("\n💡 Key Takeaways:");
        Console.WriteLine("   ✅ gRPC is 7-10x faster than REST for service-to-service");
        Console.WriteLine("   ✅ Type-safe contracts via Protocol Buffers");
        Console.WriteLine("   ✅ Supports streaming (server, client, bi-directional)");
        Console.WriteLine("   ✅ Use for internal APIs, avoid for public web APIs");
        Console.WriteLine("   ✅ HTTP/2 required, works with TLS");
    }
    
    // ================================================================
    // EXAMPLE 1: Proto Definition (Contract)
    // ================================================================
    private static void Example1_ProtoDefinition()
    {
        Console.WriteLine("=== EXAMPLE 1: Protocol Buffer Definition ===\n");
        
        Console.WriteLine("❌ BAD: No contract, REST endpoint that drifts from docs\n");
        // Antipattern: REST endpoint without formal contract
        // [HttpGet]
        // public IActionResult GetUser(int id)
        // {
        //     // What shape is the response? Unknown!
        //     return Ok(new { id, name, email, createdAt, role, ... });
        // }
        
        Console.WriteLine("✅ GOOD: Proto file defines exact contract\n");
        Console.WriteLine("// users.proto");
        Console.WriteLine("syntax = \"proto3\";");
        Console.WriteLine("");
        Console.WriteLine("service UserService {");
        Console.WriteLine("  rpc GetUser (GetUserRequest) returns (UserResponse);");
        Console.WriteLine("  rpc ListUsers (ListUsersRequest) returns (stream UserResponse);");
        Console.WriteLine("}");
        Console.WriteLine("");
        Console.WriteLine("message GetUserRequest { int32 id = 1; }");
        Console.WriteLine("");
        Console.WriteLine("message UserResponse {");
        Console.WriteLine("  int32 id = 1;");
        Console.WriteLine("  string name = 2;");
        Console.WriteLine("  string email = 3;");
        Console.WriteLine("  google.protobuf.Timestamp created_at = 4;");
        Console.WriteLine("}");
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Type safety at compile time");
        Console.WriteLine("   • Backward/forward compatible versioning");
        Console.WriteLine("   • Auto-generated client and server code");
        Console.WriteLine("   • Cross-language support");
    }
    
    // ================================================================
    // EXAMPLE 2: Service Implementation
    // ================================================================
    private static void Example2_ServiceImplementation()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Service Implementation ===\n");
        
        Console.WriteLine("❌ BAD: Mix business logic, HTTP concerns, manual validation\n");
        // Antipattern: REST controller with mixed concerns
        // [ApiController]
        // public class UsersController : ControllerBase
        // {
        //     [HttpGet("{id}")]
        //     public async Task<IActionResult> GetUser(int id)
        //     {
        //         if (id <= 0) return BadRequest("Invalid ID");
        //         var user = await _repo.GetAsync(id);
        //         if (user == null) return NotFound();
        //         return Ok(new { id = user.Id, name = user.Name, email = user.Email });
        //     }
        // }
        
        Console.WriteLine("\n✅ GOOD: Clean service logic, gRPC handles HTTP\n");
        // Best practice: gRPC service focuses on business logic
        // public class UserService : UserServiceBase
        // {
        //     public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        //     {
        //         var user = await _repo.GetAsync(request.Id);
        //         if (user == null)
        //             throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        //         return new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email };
        //     }
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • No manual serialization");
        Console.WriteLine("   • Type-safe requests/responses");
        Console.WriteLine("   • Status codes built-in");
        Console.WriteLine("   • Async by default");
    }
    
    // ================================================================
    // EXAMPLE 3: Client Usage
    // ================================================================
    private static void Example3_ClientUsage()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Client Usage ===\n");
        
        Console.WriteLine("❌ BAD: Manual REST client with HttpClient\n");
        // Antipattern:
        // var response = await _httpClient.GetAsync($"users/{id}");
        // response.EnsureSuccessStatusCode();
        // var json = await response.Content.ReadAsStringAsync();
        // var user = JsonSerializer.Deserialize<UserDto>(json);  // Can fail at runtime
        
        Console.WriteLine("\n✅ GOOD: Generated gRPC client, type-safe\n");
        // Best practice:
        // services.AddGrpcClient<UserService.UserServiceClient>(options => {
        //     options.Address = new Uri("https://localhost:5001");
        // });
        // var client = serviceProvider.GetRequiredService<UserService.UserServiceClient>();
        // var response = await client.GetUserAsync(new GetUserRequest { Id = 123 });
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • No manual HTTP calls");
        Console.WriteLine("   • Type safety (errors at compile time)");
        Console.WriteLine("   • Built-in retries, timeouts");
        Console.WriteLine("   • Connection pooling automatic");
    }
    
    // ================================================================
    // EXAMPLE 4: Error Handling
    // ================================================================
    private static void Example4_ErrorHandling()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Error Handling ===\n");
        
        Console.WriteLine("❌ BAD: Inconsistent REST error responses\n");
        // Antipattern: Different error shapes
        // return BadRequest("Invalid ID");
        // return NotFound(new { error = "User not found" });
        // return StatusCode(500, "Something broke");
        
        Console.WriteLine("\n✅ GOOD: Standard gRPC status codes\n");
        // Server:
        // throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        //
        // Client:
        // try {
        //     var user = await client.GetUserAsync(new GetUserRequest { Id = 999 });
        // } catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound) {
        //     Console.WriteLine($"User not found: {ex.Status.Detail}");
        // }
        
        Console.WriteLine("\n📊 Standard Status Codes:");
        Console.WriteLine("   • OK, NotFound, InvalidArgument");
        Console.WriteLine("   • PermissionDenied, Unauthenticated");
        Console.WriteLine("   • ResourceExhausted, Unavailable");
        Console.WriteLine("   • Internal, DeadlineExceeded");
    }
    
    // ================================================================
    // EXAMPLE 5: Streaming
    // ================================================================
    private static void Example5_Streaming()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Streaming ===\n");
        
        Console.WriteLine("❌ BAD: REST returns all data at once (memory spike)\n");
        // Antipattern:
        // [HttpGet]
        // public async Task<IActionResult> GetAllUsers()
        // {
        //     var users = await _repo.GetAllAsync();  // Load 1M users into memory
        //     return Ok(users);  // Send 500MB JSON
        // }
        
        Console.WriteLine("\n✅ GOOD: Server streaming (real-time, low memory)\n");
        // Best practice:
        // public override async Task StreamUsers(StreamUsersRequest request, 
        //     IServerStreamWriter<UserResponse> responseStream, ServerCallContext context)
        // {
        //     await foreach (var user in _repo.GetUsersAsyncEnumerable())
        //     {
        //         await responseStream.WriteAsync(new UserResponse { Id = user.Id, Name = user.Name });
        //     }
        // }
        //
        // Client:
        // var call = client.StreamUsers(new StreamUsersRequest());
        // await foreach (var user in call.ResponseStream.ReadAllAsync())
        // {
        //     Console.WriteLine($"Received: {user.Name}");
        // }
        
        Console.WriteLine("\n📊 Streaming Types:");
        Console.WriteLine("   • Server streaming: Download large datasets");
        Console.WriteLine("   • Client streaming: Upload files, metrics");
        Console.WriteLine("   • Bi-directional: Chat, real-time collaboration");
    }
    
    // ================================================================
    // EXAMPLE 6: Deadlines and Cancellation
    // ================================================================
    private static void Example6_DeadlinesAndCancellation()
    {
        Console.WriteLine("\n=== EXAMPLE 6: Deadlines & Cancellation ===\n");
        
        Console.WriteLine("❌ BAD: REST call without timeout (can hang)\n");
        // Antipattern:
        // var response = await _httpClient.GetAsync("users/slow-endpoint");  // Waits forever
        
        Console.WriteLine("\n✅ GOOD: gRPC deadline (server-enforced timeout)\n");
        // Best practice:
        // var headers = new Metadata { { "grpc-timeout", "5000m" } };  // 5 seconds
        // var response = await client.GetUserAsync(
        //     new GetUserRequest { Id = 123 },
        //     headers,
        //     deadline: DateTime.UtcNow.AddSeconds(5));
        //
        // Server checks deadline:
        // if (context.CancellationToken.IsCancellationRequested)
        //     throw new RpcException(new Status(StatusCode.Cancelled, "Client cancelled"));
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Prevents resource leaks");
        Console.WriteLine("   • Automatic cancellation propagation");
        Console.WriteLine("   • Server can stop work early");
        Console.WriteLine("   • Improves system resilience");
    }
}
