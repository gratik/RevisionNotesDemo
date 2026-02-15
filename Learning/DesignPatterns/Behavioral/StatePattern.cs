// ==============================================================================
// STATE PATTERN - Change Behavior When Internal State Changes
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE STATE PATTERN?
// --------------------------
// Allows an object to alter its behavior when its internal state changes.
// The object appears to change its class. Encapsulates state-specific behavior
// into separate state objects and delegates to the current state.
//
// Think of it as: "Vending machine - behaves differently based on state:
// HasMoney (accept selection), NoMoney (reject selection), Dispensing (release item).
// Same button press, different behavior depending on state."
//
// Core Concepts:
//   • Context: Object whose behavior varies based on state
//   • State Interface: Defines behavior contract for all states
//   • Concrete States: Encapsulate state-specific behavior
//   • State Transitions: States can trigger transitions to other states
//   • Delegation: Context delegates behavior to current state object
//
// WHY IT MATTERS
// --------------
// ✅ ELIMINATE CONDITIONALS: Replace giant if/else or switch with polymorphism
// ✅ SINGLE RESPONSIBILITY: Each state class handles one state's behavior
// ✅ OPEN/CLOSED: Add new states without modifying existing code
// ✅ STATE TRANSITIONS: Explicit, self-documenting state changes
// ✅ MAINTAINABILITY: Behavior changes localized to state classes
// ✅ TESTABILITY: Test each state independently
//
// WHEN TO USE IT
// --------------
// ✅ Object behavior depends on its state (changes at runtime)
// ✅ Operations have large conditional statements based on object state
// ✅ State transitions are well-defined and numerous
// ✅ Different states have substantially different behavior
// ✅ Need to make state transitions explicit in code
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only 2-3 simple states (if/else is clearer)
// ❌ State transitions are trivial or rare
// ❌ State-specific behavior is minimal
// ❌ States don't have significantly different behavior
//
// REAL-WORLD EXAMPLE - Order Processing System
// --------------------------------------------
// E-commerce order lifecycle (Amazon, Shopify):
//   • Order starts as "Pending" → transitions through states
//   • States: Pending → Paid → Shipped → InTransit → Delivered → Completed
//   • Each state has different allowed operations:
//
// WITHOUT STATE PATTERN (Code Smell):
//   ❌ class Order {
//         enum Status { Pending, Paid, Shipped, InTransit, Delivered }
//         Status _status;
//         
//         void Cancel() {
//             if (_status == Pending || _status == Paid) {
//                 // 20 lines: refund logic
//             } else if (_status == Shipped || _status == InTransit) {
//                 // 30 lines: recall shipment logic
//             } else if (_status == Delivered) {
//                 throw new Exception("Cannot cancel delivered order");
//             }
//         }
//         
//         void Ship() {
//             if (_status == Paid) {
//                 // 25 lines: create shipment
//             } else if (_status == Pending) {
//                 throw new Exception("Order not paid");
//             } else if (_status == Shipped) {
//                 throw new Exception("Already shipped");
//             } // ... more conditions
//         }
//         // Every method has 5-10 if/else checking _status
//     }
//   ❌ 10 methods × 5 states = 50+ if/else statements
//   ❌ Adding "Processing" state requires updating ALL methods
//   ❌ Hard to see valid state transitions
//
// WITH STATE PATTERN:
//   ✅ interface IOrderState {
//         void Cancel(Order order);
//         void Ship(Order order);
//         void Deliver(Order order);
//     }
//   
//   ✅ class PendingState : IOrderState {
//         public void Cancel(Order order) { /* Full refund */ order.SetState(new CancelledState()); }
//         public void Ship(Order order) { throw new Exception("Must pay first"); }
//     }
//   
//   ✅ class PaidState : IOrderState {
//         public void Cancel(Order order) { /* Refund */ order.SetState(new CancelledState()); }
//         public void Ship(Order order) { /* Create shipment */ order.SetState(new ShippedState()); }
//     }
//   
//   ✅ class Order {
//         private IOrderState _state = new PendingState();
//         public void Cancel() => _state.Cancel(this); // Delegate to state!
//         public void Ship() => _state.Ship(this);
//     }
//   
//   ✅ Each state = one class with clear behavior
//   ✅ Adding "Processing" state: Create ProcessingState class (no changes to other states)
//   ✅ State transitions explicit: order.SetState(new ShippedState())
//
// ANOTHER EXAMPLE - Document Workflow
// -----------------------------------
// Google Docs collaboration:
//   • States: Draft → InReview → Approved → Published
//   • Behavior changes per state:
//     - Draft: Edit(), Comment(), RequestReview()
//     - InReview: Comment(), Approve(), RejectToAuthor()
//     - Approved: Publish(), Edit() [locks document]
//     - Published: View() [read-only], CreateNewVersion()
//
// Example:
//   class Document {
//       private IDocumentState _state = new DraftState();
//       public void Edit(string content) => _state.Edit(this, content);
//   }
//   
//   class DraftState : IDocumentState {
//       public void Edit(Document doc, string content) {
//           doc.Content = content; // Allow editing
//       }
//   }
//   
//   class PublishedState : IDocumentState {
//       public void Edit(Document doc, string content) {
//           throw new InvalidOperationException("Cannot edit published document");
//       }
//   }
//
// ANOTHER EXAMPLE - Media Player
// ------------------------------
// Spotify / YouTube player:
//   • States: Stopped, Playing, Paused, Buffering
//   • Same button press, different behavior:
//     - Stopped + Play() → Start playback, transition to Playing
//     - Playing + Play() → Already playing, do nothing
//     - Paused + Play() → Resume playback, transition to Playing
//     - Buffering + Play() → Queue play action for after buffering
//
// ANOTHER EXAMPLE - TCP Connection
// --------------------------------
// Network connection lifecycle:
//   • States: Closed, Listen, SynSent, SynReceived, Established, FinWait, CloseWait, Closed
//   • Each state handles packets differently:
//     - Closed + Open() → Transition to Listen
//     - Established + Send(data) → Transmit data → Stay in Established
//     - Established + Close() → Send FIN packet → Transition to FinWait
//
// STATE TRANSITION DIAGRAM EXAMPLE
// --------------------------------
// Traffic Light:
//   Red → Green → Yellow → Red (cycle)
//   
//   class RedLightState : ITrafficLightState {
//       public void Next(TrafficLight context) {
//           Console.WriteLine("Red → Green");
//           context.SetState(new GreenLightState());
//       }
//   }
//
// WHO CONTROLS TRANSITIONS?
// -------------------------
// Option 1: State classes control transitions (most common):
//   state.Handle() internally calls context.SetState(newState)
//   ✅ State knows its successors
//   ❌ States coupled to successor states
//
// Option 2: Context controls transitions:
//   context.Handle() calls state.Handle(), then context.SetState(newState)
//   ✅ Context has full control over flow
//   ❌ Context must know state transition rules
//
// STATE VS SIMILAR PATTERNS
// -------------------------
// State vs Strategy:
//   • State: Context changes state automatically, states know about transitions
//   • Strategy: Client sets strategy explicitly, strategies independent
//
// State vs Finite State Machine:
//   • State Pattern: Object-oriented implementation
//   • FSM: Can be implemented with tables, enums, or State Pattern
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// State pattern in .NET:
//   • Task<T> states: Running, WaitingForActivation, RanToCompletion, Faulted, Canceled
//   • HttpClient: Idle, SendingRequest, WaitingForResponse, ReceivingResponse
//   • Entity Framework: EntityState (Unchanged, Added, Modified, Deleted, Detached)
//
// BEST PRACTICES
// --------------
// ✅ Use enum for state tracking even with State pattern (debugging)
// ✅ Consider flyweight pattern if states are stateless (reuse instances)
// ✅ Keep state classes focused (Single Responsibility)
// ✅ Document valid state transitions (state diagram)
// ✅ Throw exceptions for invalid transitions
// ✅ Consider using state machine libraries for complex scenarios (Stateless, Automatonymous)
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// State interface
public interface ITrafficLightState
{
    void Handle(TrafficLight context);
    string GetColor();
}

// Concrete States
public class RedLightState : ITrafficLightState
{
    public void Handle(TrafficLight context)
    {
        Console.WriteLine("[STATE] 🔴 RED LIGHT - Stop!");
        Console.WriteLine("[STATE] Transitioning to Green...");
        context.SetState(new GreenLightState());
    }

    public string GetColor() => "Red";
}

public class GreenLightState : ITrafficLightState
{
    public void Handle(TrafficLight context)
    {
        Console.WriteLine("[STATE] 🟢 GREEN LIGHT - Go!");
        Console.WriteLine("[STATE] Transitioning to Yellow...");
        context.SetState(new YellowLightState());
    }

    public string GetColor() => "Green";
}

public class YellowLightState : ITrafficLightState
{
    public void Handle(TrafficLight context)
    {
        Console.WriteLine("[STATE] 🟡 YELLOW LIGHT - Caution!");
        Console.WriteLine("[STATE] Transitioning to Red...");
        context.SetState(new RedLightState());
    }

    public string GetColor() => "Yellow";
}

// Context
public class TrafficLight
{
    private ITrafficLightState _currentState;

    public TrafficLight()
    {
        _currentState = new RedLightState();
        Console.WriteLine($"[STATE] Traffic light initialized: {_currentState.GetColor()}");
    }

    public void SetState(ITrafficLightState state)
    {
        _currentState = state;
    }

    public void Change()
    {
        _currentState.Handle(this);
    }

    public string GetCurrentColor() => _currentState.GetColor();
}

// Usage demonstration
public class StateDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== STATE PATTERN DEMO ===\n");

        var trafficLight = new TrafficLight();

        for (int i = 0; i < 6; i++)
        {
            Console.WriteLine($"\n[STATE] Current: {trafficLight.GetCurrentColor()}");
            trafficLight.Change();
            System.Threading.Thread.Sleep(500);
        }

        Console.WriteLine("\n💡 Benefit: Behavior changes based on internal state");
        Console.WriteLine("💡 Benefit: Eliminates conditional logic");
    }
}
