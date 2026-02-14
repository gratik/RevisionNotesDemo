// ============================================================================
// DEPENDENCY INVERSION PRINCIPLE (DIP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
// DEFINITION:
//   "High-level modules should not depend on low-level modules. Both should depend
//   on abstractions. Abstractions should not depend on details. Details should depend
//   on abstractions."
//
// EXPLANATION:
//   Depend on interfaces or abstract classes, not concrete implementations. This
//   inverts the traditional dependency flow where high-level code depends on low-level
//   code. Now both depend on abstractions, allowing implementations to be swapped easily.
//
// EXAMPLE:
//   ❌ BAD: OrderService directly creates SqlDatabase instance
//   ✅ GOOD: OrderService depends on IDatabase interface; concrete implementation injected
//
// REAL-WORLD ANALOGY:
//   Wall electrical outlets (abstraction) - you don't care about the power plant
//   (implementation). Any compliant device works with any outlet.
//   USB standard - devices work with any USB port.
//
// BENEFITS:
//   • Loose coupling between modules
//   • Easy to test (mock dependencies)
//   • Flexible - swap implementations easily
//   • Better maintainability
//   • Supports plugin architectures
//
// WHEN TO USE:
//   • Always! This is a fundamental principle
//   • When you need to swap implementations
//   • When unit testing (need to mock dependencies)
//   • When building layered architectures
//   • When multiple implementations of same behavior exist
//
// COMMON VIOLATIONS:
//   • Using 'new' keyword to create dependencies inside classes
//   • Direct references to concrete classes
//   • Hardcoded file paths, connection strings
//   • Static method calls to concrete implementations
//
// HOW TO ACHIEVE DIP:
//   • Dependency Injection (Constructor, Property, Method injection)
//   • IoC (Inversion of Control) containers
//   • Factory patterns
//   • Service Locator pattern (less preferred)
//
// BEST PRACTICES:
//   • Inject dependencies via constructor (preferred)
//   • Program to interfaces
//   • Use DI containers (built-in .NET Core DI)
//   • Keep abstractions stable, let implementations change
//   • Follow Hollywood Principle: "Don't call us, we'll call you"
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates DIP
// High-level module depends directly on low-level concrete implementations

public class EmailSenderBad
{
    public void SendEmail(string to, string message)
    {
        Console.WriteLine($"Sending email to {to}: {message}");
    }
}

public class SmsSenderBad
{
    public void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"Sending SMS to {phoneNumber}: {message}");
    }
}

// High-level module tightly coupled to low-level concrete classes
public class NotificationServiceBad
{
    private readonly EmailSenderBad _emailSender;
    private readonly SmsSenderBad _smsSender;

    public NotificationServiceBad()
    {
        // Tightly coupled - hard to test and extend
        _emailSender = new EmailSenderBad();
        _smsSender = new SmsSenderBad();
    }

    public void SendEmailNotification(string to, string message)
    {
        _emailSender.SendEmail(to, message);
    }

    public void SendSmsNotification(string phoneNumber, string message)
    {
        _smsSender.SendSms(phoneNumber, message);
    }

    // Adding push notifications requires modifying this class!
}

// ✅ GOOD EXAMPLE - Follows DIP
// Both high-level and low-level modules depend on abstraction

// Abstraction that both high and low level modules depend on
public interface IMessageSender
{
    void Send(string recipient, string message);
    string GetSenderType();
}

// Low-level module implementations
public class EmailSender : IMessageSender
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[DIP] Sending email to {recipient}");
        Console.WriteLine($"[DIP] Email content: {message}");
    }

    public string GetSenderType() => "Email";
}

public class SmsSender : IMessageSender
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[DIP] Sending SMS to {recipient}");
        Console.WriteLine($"[DIP] SMS content: {message}");
    }

    public string GetSenderType() => "SMS";
}

public class PushNotificationSender : IMessageSender
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[DIP] Sending push notification to device {recipient}");
        Console.WriteLine($"[DIP] Notification: {message}");
    }

    public string GetSenderType() => "Push Notification";
}

public class SlackSender : IMessageSender
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[DIP] Sending Slack message to {recipient}");
        Console.WriteLine($"[DIP] Message: {message}");
    }

    public string GetSenderType() => "Slack";
}

// High-level module depends on abstraction (IMessageSender)
public class NotificationService
{
    private readonly IEnumerable<IMessageSender> _messageSenders;

    // Dependency injection - follows DIP
    public NotificationService(IEnumerable<IMessageSender> messageSenders)
    {
        _messageSenders = messageSenders;
    }

    public void SendNotification(string recipient, string message, string senderType)
    {
        var sender = _messageSenders.FirstOrDefault(s =>
            s.GetSenderType().Equals(senderType, StringComparison.OrdinalIgnoreCase));

        if (sender == null)
        {
            Console.WriteLine($"[DIP] No sender found for type: {senderType}");
            return;
        }

        Console.WriteLine($"[DIP] Using {sender.GetSenderType()} sender...");
        sender.Send(recipient, message);
    }

    public void SendToAll(string recipient, string message)
    {
        Console.WriteLine($"[DIP] Broadcasting message to all channels...");
        foreach (var sender in _messageSenders)
        {
            sender.Send(recipient, message);
        }
    }
}

// Another high-level module that also depends on abstraction
public class AlertSystem
{
    private readonly IMessageSender _urgentChannel;

    public AlertSystem(IMessageSender urgentChannel)
    {
        _urgentChannel = urgentChannel;
    }

    public void SendUrgentAlert(string recipient, string alertMessage)
    {
        Console.WriteLine($"[DIP] URGENT ALERT through {_urgentChannel.GetSenderType()}!");
        _urgentChannel.Send(recipient, $"🚨 URGENT: {alertMessage}");
    }
}

// Usage demonstration
public class DIPDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== DEPENDENCY INVERSION PRINCIPLE DEMO ===\n");

        // Create concrete implementations
        var messageSenders = new List<IMessageSender>
        {
            new EmailSender(),
            new SmsSender(),
            new PushNotificationSender(),
            new SlackSender()
        };

        // Inject dependencies - high-level module doesn't know about concrete types
        var notificationService = new NotificationService(messageSenders);

        Console.WriteLine("Example 1: Sending specific notification type");
        notificationService.SendNotification("user@example.com", "Welcome to our service!", "Email");

        Console.WriteLine("\nExample 2: Sending to all channels");
        notificationService.SendToAll("user@example.com", "Important update!");

        Console.WriteLine("\nExample 3: Alert system with SMS as urgent channel");
        var alertSystem = new AlertSystem(new SmsSender());
        alertSystem.SendUrgentAlert("+1234567890", "System breach detected!");

        Console.WriteLine("\nExample 4: Alert system with Slack as urgent channel");
        var slackAlertSystem = new AlertSystem(new SlackSender());
        slackAlertSystem.SendUrgentAlert("@devops-team", "Server CPU at 95%!");

        Console.WriteLine("\nBenefit: Easy to extend with new senders without modifying existing code!");
        Console.WriteLine("Benefit: Easy to test by injecting mock implementations!");
        Console.WriteLine("Benefit: Loose coupling between high-level and low-level modules!");
    }
}
