// ==============================================================================
// CHAIN OF RESPONSIBILITY PATTERN - Pass Request Through Handler Chain
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE CHAIN OF RESPONSIBILITY PATTERN?
// ---------------------------------------------
// Passes a request along a chain of handlers. Each handler decides either to
// process the request or pass it to the next handler in the chain. Decouples
// sender from receiver and allows multiple objects a chance to handle the request.
//
// Think of it as: "Customer support escalation - Level 1 support tries to help.
// Can't solve? → Escalate to Level 2 → Still stuck? → Escalate to Level 3 manager.
// Request flows through chain until someone handles it."
//
// Core Concepts:
//   • Handler: Interface/abstract class defining handling method and next link
//   • Concrete Handlers: Process request or pass to next handler
//   • Chain: Linked list of handlers
//   • Request: Information passed through chain
//   • Dynamic Chain: Handlers can be added/removed at runtime
//
// WHY IT MATTERS
// --------------
// ✅ DECOUPLING: Sender doesn't know which handler processes request
// ✅ FLEXIBILITY: Add/remove/reorder handlers dynamically
// ✅ SINGLE RESPONSIBILITY: Each handler has one clear responsibility
// ✅ OPEN/CLOSED: Add new handlers without modifying existing code
// ✅ RESPONSIBILITY SHARING: Multiple objects can handle without explicit assignment
// ✅ ELIMINATES CONDITIONALS: Replace if/else chain with handler chain
//
// WHEN TO USE IT
// --------------
// ✅ Multiple objects can handle request, handler not known in advance
// ✅ Want to issue request to one of several objects without specifying receiver
// ✅ Set of handlers and their order should be dynamic
// ✅ Need to try multiple approaches until one succeeds
// ✅ Have hierarchy of handlers (escalation, priority levels)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only one handler ever processes request (use direct call)
// ❌ All handlers must process request (use Observer pattern)
// ❌ Handler order is fixed and trivial (use direct calls)
// ❌ Performance critical (chain traversal has overhead)
//
// REAL-WORLD EXAMPLE - Expense Approval System
// --------------------------------------------
// Corporate expense approval (SAP, Workday):
//   • Employee submits expense report: $4,500
//   • Approval chain based on amount:
//     1. **Manager** (approves up to $1,000)
//        → $4,500 > $1,000 → Pass to Director
//     2. **Director** (approves up to $5,000)
//        → $4,500 ≤ $5,000 → ✅ APPROVED (chain stops here)
//     3. **VP** (approves up to $10,000)
//        → Never reached in this case
//     4. **CFO** (approves anything)
//        → Never reached in this case
//
// WITHOUT CHAIN PATTERN:
//   ❌ if (amount <= 1000) manager.Approve();
//      else if (amount <= 5000) director.Approve();
//      else if (amount <= 10000) vp.Approve();
//      else cfo.Approve();
//   ❌ Hard-coded hierarchy
//   ❌ Can't change approval limits dynamically
//   ❌ Can't add "Regional Manager" level easily
//
// WITH CHAIN PATTERN:
//   ✅ abstract class ExpenseHandler {
//         protected ExpenseHandler _next;
//         protected decimal _approvalLimit;
//         public void SetNext(ExpenseHandler next) => _next = next;
//         public void HandleRequest(Expense expense) {
//             if (expense.Amount <= _approvalLimit) {
//                 Approve(expense); // Handle it
//             } else {
//                 _next?.HandleRequest(expense); // Pass along
//             }
//         }
//     }
//   
//   ✅ var manager = new Manager(1000);
//      var director = new Director(5000);
//      var vp = new VP(10000);
//      var cfo = new CFO(decimal.MaxValue);
//      
//      manager.SetNext(director);
//      director.SetNext(vp);
//      vp.SetNext(cfo);
//      
//      manager.HandleRequest(new Expense(4500)); // Automatically escalates to director
//   
//   ✅ Dynamic: Add regional manager between manager and director
//   ✅ Flexible: Change approval limits at runtime
//
// ANOTHER EXAMPLE - Logging Pipeline
// ----------------------------------
// Log4Net / Serilog style multi-level logging:
//   • Log message with level: ERROR
//   • Handler chain:
//     1. **ConsoleLogHandler** (logs INFO, WARNING, ERROR)
//        → ERROR → Logs "❌ ERROR: Database connection failed" to console
//        → Passes to next handler
//     2. **FileLogHandler** (logs WARNING, ERROR)
//        → ERROR → Appends to error.log file
//        → Passes to next handler
//     3. **EmailLogHandler** (logs only ERROR, CRITICAL)
//        → ERROR → Sends email to dev team
//        → Chain ends
//
// Key difference: All handlers in chain can process (not just first one)
//
// Code:
//   abstract class LogHandler {
//       protected LogHandler _next;
//       public void Handle(string msg, LogLevel level) {
//           if (CanHandle(level)) WriteLog(msg, level);
//           _next?.Handle(msg, level); // Always pass to next (multi-handler)
//       }
//   }
//
// ANOTHER EXAMPLE - Middleware Pipeline (ASP.NET Core)
// ----------------------------------------------------
// HTTP request processing:
//   • Request: GET /api/users
//   • Middleware chain:
//     1. **ExceptionHandlerMiddleware** → Try/catch wrapper → Pass to next
//     2. **AuthenticationMiddleware** → Validate JWT token → Pass to next
//     3. **AuthorizationMiddleware** → Check permissions → Pass to next
//     4. **LoggingMiddleware** → Log request → Pass to next
//     5. **RoutingMiddleware** → Find endpoint → Pass to next
//     6. **EndpointMiddleware** → Execute controller action → Return response
//
// Code (ASP.NET Core):
//   app.UseExceptionHandler();
//   app.UseAuthentication();
//   app.UseAuthorization();
//   app.UseLogging();
//   app.UseRouting();
//   app.UseEndpoints(...);
//
// Each middleware:
//   async Task Invoke(HttpContext context, RequestDelegate next) {
//       // Pre-processing
//       await next(context); // Call next in chain
//       // Post-processing
//   }
//
// ANOTHER EXAMPLE - UI Event Bubbling
// -----------------------------------
// WPF/HTML event propagation:
//   • Click on <button> inside <div> inside <form>
//   • Event bubbles up:
//     1. Button.OnClick() → If not handled → Bubble to parent
//     2. Div.OnClick() → If not handled → Bubble to parent
//     3. Form.OnClick() → If not handled → Bubble to parent
//     4. Document.OnClick() → Top level
//
// Handler can stop propagation:
//   button.OnClick = (e) => {
//       e.StopPropagation(); // Chain stops here
//   };
//
// PURE CHAIN VS MULTI-HANDLER CHAIN
// ---------------------------------
// **Pure Chain** (first handler stops chain):
//   • Expense approval: Only one approver handles request
//   • Error handling: First catch block handles exception
//   • Support escalation: Solved at one level
//
// **Multi-Handler Chain** (all handlers can process):
//   • Logging: All loggers write (console + file + email)
//   • Middleware: All middleware processes request/response
//   • Event handlers: All subscribers notified
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Chain of Responsibility in .NET:
//   • ASP.NET Core Middleware: app.Use(), app.UseWhen()
//   • Exception handling: try/catch/catch hierarchy
//   • WPF event routing: Tunneling and bubbling events
//   • IMessageHandler in MassTransit
//   • Delegating handlers in HttpClient
//
// IMPLEMENTATION VARIATIONS
// -------------------------
// Variation 1: Abstract base class
//   abstract class Handler {
//       protected Handler _next;
//       public abstract void Handle(Request req);
//   }
//
// Variation 2: Interface
//   interface IHandler {
//       IHandler Next { get; set; }
//       void Handle(Request req);
//   }
//
// Variation 3: Func<> delegates (functional approach)
//   Func<Request, bool> handler = request => {
//       if (CanHandle(request)) { Process(); return true; }
//       return false;
//   };
//
// BEST PRACTICES
// --------------
// ✅ Default handler at end of chain (catch-all)
// ✅ Clear naming: Handler responsibility obvious from name
// ✅ Avoid infinite loops (handler can't call itself)
// ✅ Consider request priority (order matters)
// ✅ Log when request passes unhandled
// ✅ Make chain configuration explicit (builder pattern)
// ✅ Unit test each handler independently
//
// CHAIN BUILDER PATTERN
// ---------------------
// Fluent API for building chain:
//   var chain = new ChainBuilder<Expense>()
//       .Add(new Manager(1000))
//       .Add(new Director(5000))
//       .Add(new VP(10000))
//       .Add(new CFO(decimal.MaxValue))
//       .Build();
//   
//   chain.Handle(expense);
//
// CHAIN VS SIMILAR PATTERNS
// -------------------------
// Chain of Responsibility vs Decorator:
//   • Chain: Request stops at first handler that processes
//   • Decorator: All decorators wrap and enhance behavior
//
// Chain vs Observer:
//   • Chain: One handler processes (or all in multi-handler)
//   • Observer: All observers notified regardless
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Handler interface
public abstract class LogHandler
{
    protected LogHandler? NextHandler;

    public void SetNext(LogHandler handler)
    {
        NextHandler = handler;
    }

    public abstract void Handle(string message, LogLevel level);
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

// Concrete Handlers
public class ConsoleLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Info)
        {
            Console.WriteLine($"[CHAIN] 💻 Console: [INFO] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

public class FileLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Warning)
        {
            Console.WriteLine($"[CHAIN] 📄 File: [WARNING] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

public class EmailLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Error)
        {
            Console.WriteLine($"[CHAIN] 📧 Email: [ERROR] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

// Usage demonstration
public class ChainOfResponsibilityDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== CHAIN OF RESPONSIBILITY PATTERN DEMO ===\n");

        // Build the chain
        var consoleHandler = new ConsoleLogHandler();
        var fileHandler = new FileLogHandler();
        var emailHandler = new EmailLogHandler();

        consoleHandler.SetNext(fileHandler);
        fileHandler.SetNext(emailHandler);

        Console.WriteLine("[CHAIN] Logging various messages:\n");

        consoleHandler.Handle("Application started", LogLevel.Info);
        consoleHandler.Handle("Low disk space", LogLevel.Warning);
        consoleHandler.Handle("Database connection failed", LogLevel.Error);

        Console.WriteLine("\n💡 Benefit: Request passes through chain until handled");
        Console.WriteLine("💡 Benefit: Decouples sender from receivers");
    }
}
