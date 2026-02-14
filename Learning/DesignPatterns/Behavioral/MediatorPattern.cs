// ============================================================================
// MEDIATOR PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// PURPOSE: "Reduces coupling between classes by centralizing communication."
// EXAMPLE: Chat room message routing.
// ============================================================================

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
