// ==============================================================================
// MEDIATOR PATTERN - Centralize Complex Communications
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE MEDIATOR PATTERN?
// ------------------------------
// Defines an object that encapsulates how a set of objects interact. Promotes
// loose coupling by keeping objects from referring to each other explicitly,
// and lets you vary their interaction independently. All communication goes
// through a central mediator object.
//
// Think of it as: "Air traffic control tower - planes don't talk to each other
// directly. They all communicate through the tower, which coordinates everything."
//
// Core Concepts:
//   • Mediator: Central coordinator that encapsulates interaction logic
//   • Colleagues: Objects that communicate via mediator (not directly)
//   • Indirect Communication: Colleagues know mediator, not each other
//   • Coordination Logic: Mediator contains complex interaction rules
//   • Many-to-Many → Many-to-One: Reduces coupling complexity
//
// WHY IT MATTERS
// --------------
// ✅ LOOSE COUPLING: Components don't know about each other
// ✅ CENTRALIZED CONTROL: All interaction logic in one place
// ✅ SIMPLIFIED COMMUNICATION: N objects need 1 reference (mediator), not N-1 references
// ✅ REUSABILITY: Components can be reused in different contexts
// ✅ FLEXIBILITY: Easy to change interaction logic without changing components
// ✅ SINGLE RESPONSIBILITY: Components focus on their job, mediator handles coordination
//
// WHEN TO USE IT
// --------------
// ✅ Many objects communicate in complex, well-defined ways
// ✅ Reusing objects is difficult due to dependencies on many other objects
// ✅ Object references are spread across multiple classes (tangled web)
// ✅ Behavior distributed between classes should be customizable
// ✅ Central coordinator makes sense for the domain
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple one-to-one communication (direct reference is clearer)
// ❌ Mediator becomes "god object" (too much responsibility)
// ❌ Components need direct communication for performance
// ❌ Interaction rules are trivial or rarely change
//
// REAL-WORLD EXAMPLE - Chat Room Application
// ------------------------------------------
// Slack / Discord / Teams chat:
//   • 100 users in a channel
//   • Without Mediator:
//     - Each user needs reference to 99 other users
//     - User1.SendTo(User2), User1.SendTo(User3), ...
//     - 100 users × 99 connections = 9,900 potential connections!
//     - Can't implement "# general" (send to all) without knowing all users
//     - Can't implement "@mentions" or "DMs" easily
//     - Adding User101 requires updating 100 other users
//
//   • With Mediator (ChatRoom):
//     - Each user knows only ChatRoom
//     - User1.Send("Hello") → chatRoom.Broadcast(User1, "Hello")
//     - ChatRoom decides who receives message
//     - 100 users × 1 connection = 100 connections (99% reduction!)
//     - Easy to add features: @mentions, DMs, typing indicators, read receipts
//     - Adding User101: Just register with ChatRoom
//
// Code structure:
//   interface IChatMediator {
//       void SendMessage(string msg, User sender);
//       void SendPrivateMessage(string msg, User sender, User recipient);
//   }
//   
//   class ChatRoom : IChatMediator {
//       List<User> _users;
//       public void SendMessage(string msg, User sender) {
//           foreach (var user in _users.Where(u => u != sender))
//               user.Receive(msg, sender);
//       }
//   }
//   
//   class User {
//       IChatMediator _chatRoom; // Only knows mediator!
//       public void Send(string msg) => _chatRoom.SendMessage(msg, this);
//       public void Receive(string msg, User sender) => Console.WriteLine($"{sender.Name}: {msg}");
//   }
//
// ANOTHER EXAMPLE - Air Traffic Control
// -------------------------------------
// Airport with 50 planes landing/taking off:
//   • Without Mediator:
//     - Plane1 broadcasts: "I'm landing on runway 3"
//     - All 49 planes must listen, check if they conflict
//     - Plane2: "Am I too close?" checks distances to all planes
//     - Chaos! Collisions! No coordination!
//
//   • With Mediator (Air Traffic Control Tower):
//     - Plane requests landing: tower.RequestLanding(plane)
//     - Tower checks all planes, assigns runway, gives clearance
//     - Tower: "Plane 1, cleared to land runway 3"
//     - Tower tracks all planes, ensures safe distances
//     - Planes follow tower instructions, don't coordinate directly
//
// ANOTHER EXAMPLE - UI Dialog Components
// --------------------------------------
// Complex form with interdependent fields:
//   • Form has: Country dropdown, State dropdown, Zip code field, Submit button
//   • Rules:
//     - State dropdown enabled only if Country = "USA"
//     - Zip code format changes based on Country (USA: 12345, Canada: A1B 2C3)
//     - Submit button enabled only if all required fields valid
//
//   • Without Mediator:
//     - CountryDropdown knows about StateDropdown, ZipCodeField, SubmitButton
//     - StateDropdown knows about ZipCodeField, SubmitButton
//     - ZipCodeField knows about SubmitButton
//     - Tangled dependencies! Hard to test, reuse, modify!
//
//   • With Mediator (FormMediator):
//     - CountryDropdown.OnChange() → mediator.OnCountryChanged(country)
//     - FormMediator decides:
//       → stateDropdown.SetEnabled(country == "USA")
//       → zipCodeField.SetFormat(country)
//       → submitButton.SetEnabled(ValidateAllFields())
//     - Components just notify mediator, mediator orchestrates everything
//
// ANOTHER EXAMPLE - Game Entity Communication
// -------------------------------------------
// Multiplayer game with Player, Enemy, PowerUp, ScoreBoard:
//   • Player collects PowerUp:
//     - Without Mediator: Player.OnCollect() calls PowerUp.Apply(), Player.IncreaseScore(), ScoreBoard.Update()
//     - Player coupled to PowerUp, ScoreBoard
//   
//   • With Mediator (GameCoordinator):
//     - Player detects collision → coordinator.OnPowerUpCollected(player, powerUp)
//     - GameCoordinator:
//       1. powerUp.Apply(player)
//       2. player.AddScore(100)
//       3. scoreBoard.Update(player.Score)
//       4. soundManager.Play("powerup.wav")
//       5. particleSystem.Emit("sparkles")
//     - Player doesn't know about ScoreBoard, SoundManager, ParticleSystem
//
// MEDIATOR VS SIMILAR PATTERNS
// ----------------------------
// Mediator vs Observer:
//   • Mediator: Many-to-many, bidirectional, complex coordination logic
//   • Observer: One-to-many, unidirectional, simple notification
//
// Mediator vs Facade:
//   • Mediator: Coordinates peers, bidirectional communication
//   • Facade: Unidirectional interface to subsystem, simplifies access
//
// Mediator vs MediatR Library:
//   • MediatR: Command/Query handlers, application-level messaging
//   • Mediator Pattern: Object-level communication coordination
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Mediator pattern in .NET:
//   • MediatR library: IMediator, IRequest, INotification
//   • WPF: ICommand + CommandManager coordinate UI actions
//   • ASP.NET Core Middleware: Pipeline mediates request/response
//   • Event Aggregator (Prism): Mediates loosely coupled events
//
// POTENTIAL PROBLEMS
// ------------------
// ⚠️ God Object Anti-Pattern:
//   • Mediator can become too complex with all coordination logic
//   • Solution: Split into multiple mediators if needed (e.g., ChatMediator, NotificationMediator)
//
// ⚠️ Performance:
//   • Extra indirection for every interaction
//   • Solution: Only use for complex coordination, not trivial cases
//
// BEST PRACTICES
// --------------
// ✅ Keep mediator focused (don't make it a "god object")
// ✅ Use interfaces for mediator (testability, flexibility)
// ✅ Colleagues should only know mediator interface, not concrete type
// ✅ Mediator can be stateless or stateful (tracks colleague state)
// ✅ Consider using events within mediator for extensibility
// ✅ Unit test mediator coordination logic separately
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Mediator interface
public interface IChatMediator
{
    void SendMessage(string message, User sender);
    void AddUser(User user);
}

// Concrete Mediator
public class ChatRoom : IChatMediator
{
    private readonly List<User> _users = new();

    public void AddUser(User user)
    {
        _users.Add(user);
        Console.WriteLine($"[MEDIATOR] User {user.Name} joined the chat room");
    }

    public void SendMessage(string message, User sender)
    {
        Console.WriteLine($"\n[MEDIATOR] {sender.Name} sends: {message}");
        foreach (var user in _users)
        {
            if (user != sender) // Don't send to self
            {
                user.Receive(message, sender);
            }
        }
    }
}

// Colleague
public class User
{
    public string Name { get; }
    private readonly IChatMediator _mediator;

    public User(string name, IChatMediator mediator)
    {
        Name = name;
        _mediator = mediator;
    }

    public void Send(string message)
    {
        _mediator.SendMessage(message, this);
    }

    public void Receive(string message, User sender)
    {
        Console.WriteLine($"[MEDIATOR]   → {Name} received from {sender.Name}: {message}");
    }
}

// Usage demonstration
public class MediatorDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== MEDIATOR PATTERN DEMO ===\n");

        var chatRoom = new ChatRoom();

        var alice = new User("Alice", chatRoom);
        var bob = new User("Bob", chatRoom);
        var charlie = new User("Charlie", chatRoom);

        chatRoom.AddUser(alice);
        chatRoom.AddUser(bob);
        chatRoom.AddUser(charlie);

        Console.WriteLine();
        alice.Send("Hello everyone!");
        bob.Send("Hi Alice!");
        charlie.Send("Hey all!");

        Console.WriteLine("\n💡 Benefit: Centralized communication logic");
        Console.WriteLine("💡 Benefit: Reduces direct dependencies between colleagues");
    }
}
