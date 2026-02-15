// ============================================================================
// DEPENDENCY INVERSION PRINCIPLE (DIP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
//
// WHAT IS DIP?
// ------------
// High-level modules should not depend on low-level modules. Both should depend
// on abstractions. Details should depend on abstractions, not the other way around.
//
// WHY IT MATTERS
// --------------
// - Reduces coupling between layers
// - Enables easy testing with mocks/fakes
// - Allows swapping implementations without changes
// - Supports modular, maintainable architectures
//
// WHEN TO USE
// -----------
// - YES: When you need multiple implementations of a dependency
// - YES: When code must be unit tested in isolation
// - YES: In layered architectures (UI, domain, infrastructure)
//
// WHEN NOT TO USE
// ---------------
// - NO: For tiny utilities where abstraction adds no value
// - NO: If the dependency is stable and unlikely to change
//
// REAL-WORLD EXAMPLE
// ------------------
// Notifications:
// - NotificationService depends on IMessageSender
// - Implementations: EmailSender, SmsSender, SlackSender
// - Add a new channel without modifying NotificationService
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
