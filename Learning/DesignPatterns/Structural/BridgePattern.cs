// ==============================================================================
// BRIDGE PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Decouples abstraction from implementation so they can vary independently
// BENEFIT: Avoids class explosion, runtime binding, independent extension
// USE WHEN: Variations in both abstraction and implementation, want to avoid Cartesian product
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// ========================================================================
// IMPLEMENTATION INTERFACE (Low-level operations)
// ========================================================================

/// <summary>
/// Implementation interface - defines how messages are sent
/// </summary>
public interface IMessageSender
{
    void SendMessage(string recipient, string message);
}

// ========================================================================
// CONCRETE IMPLEMENTATIONS
// ========================================================================

public class EmailSender : IMessageSender
{
    public void SendMessage(string recipient, string message)
    {
        Console.WriteLine($"📧 [EMAIL] To: {recipient}");
        Console.WriteLine($"   Subject: Message");
        Console.WriteLine($"   Body: {message}\n");
    }
}

public class SMSSender : IMessageSender
{
    public void SendMessage(string recipient, string message)
    {
        Console.WriteLine($"📱 [SMS] To: {recipient}");
        Console.WriteLine($"   Text: {message}\n");
    }
}

public class SlackSender : IMessageSender
{
    public void SendMessage(string recipient, string message)
    {
        Console.WriteLine($"💬 [SLACK] Channel/User: {recipient}");
        Console.WriteLine($"   Message: {message}\n");
    }
}

// ========================================================================
// ABSTRACTION (High-level operations)
// ========================================================================

/// <summary>
/// Abstraction - defines what messages to send
/// </summary>
public abstract class Message
{
    protected IMessageSender _sender;  // Bridge to implementation

    protected Message(IMessageSender sender)
    {
        _sender = sender;
    }

    public abstract void Send(string recipient);
}

// ========================================================================
// REFINED ABSTRACTIONS
// ========================================================================

public class TextMessage : Message
{
    private readonly string _content;

    public TextMessage(IMessageSender sender, string content) : base(sender)
    {
        _content = content;
    }

    public override void Send(string recipient)
    {
        Console.WriteLine($"[TEXT MESSAGE] Sending to {recipient}...");
        _sender.SendMessage(recipient, _content);
    }
}

public class UrgentMessage : Message
{
    private readonly string _content;

    public UrgentMessage(IMessageSender sender, string content) : base(sender)
    {
        _content = content;
    }

    public override void Send(string recipient)
    {
        Console.WriteLine($"[URGENT MESSAGE] ⚠️  URGENT! Sending to {recipient}...");
        _sender.SendMessage(recipient, $"🚨 URGENT: {_content}");
    }
}

public class EncryptedMessage : Message
{
    private readonly string _content;

    public EncryptedMessage(IMessageSender sender, string content) : base(sender)
    {
        _content = content;
    }

    public override void Send(string recipient)
    {
        Console.WriteLine($"[ENCRYPTED MESSAGE] 🔒 Encrypting and sending to {recipient}...");
        var encrypted = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_content));
        _sender.SendMessage(recipient, $"🔐 Encrypted: {encrypted}");
    }
}

// ========================================================================
// EXAMPLE 2: REMOTE CONTROL (Classic Bridge Example)
// ========================================================================

/// <summary>
/// Implementation - Device interface
/// </summary>
public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetVolume(int volume);
    int GetVolume();
    string GetName();
}

public class Television : IDevice
{
    private bool _on = false;
    private int _volume = 50;

    public void TurnOn()
    {
        _on = true;
        Console.WriteLine("  📺 [TV] Powered ON");
    }

    public void TurnOff()
    {
        _on = false;
        Console.WriteLine("  📺 [TV] Powered OFF");
    }

    public void SetVolume(int volume)
    {
        _volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"  📺 [TV] Volume set to {_volume}");
    }

    public int GetVolume() => _volume;

    public string GetName() => "Television";
}

public class Radio : IDevice
{
    private bool _on = false;
    private int _volume = 30;

    public void TurnOn()
    {
        _on = true;
        Console.WriteLine("  📻 [RADIO] Powered ON");
    }

    public void TurnOff()
    {
        _on = false;
        Console.WriteLine("  📻 [RADIO] Powered OFF");
    }

    public void SetVolume(int volume)
    {
        _volume = Math.Max(0, Math.Min(100, volume));
        Console.WriteLine($"  📻 [RADIO] Volume set to {_volume}");
    }

    public int GetVolume() => _volume;

    public string GetName() => "Radio";
}

/// <summary>
/// Abstraction - Remote control
/// </summary>
public abstract class RemoteControl
{
    protected IDevice _device;  // Bridge to implementation

    protected RemoteControl(IDevice device)
    {
        _device = device;
    }

    public virtual void TogglePower()
    {
        Console.WriteLine($"\n[REMOTE] Toggling power for {_device.GetName()}...");
        // Simple toggle (real implementation would check state)
        _device.TurnOn();
    }

    public virtual void VolumeUp()
    {
        Console.WriteLine($"\n[REMOTE] Volume up on {_device.GetName()}...");
        _device.SetVolume(_device.GetVolume() + 10);
    }

    public virtual void VolumeDown()
    {
        Console.WriteLine($"\n[REMOTE] Volume down on {_device.GetName()}...");
        _device.SetVolume(_device.GetVolume() - 10);
    }
}

/// <summary>
/// Refined Abstraction - Advanced remote with mute
/// </summary>
public class AdvancedRemote : RemoteControl
{
    public AdvancedRemote(IDevice device) : base(device)
    {
    }

    public void Mute()
    {
        Console.WriteLine($"\n[ADVANCED REMOTE] Muting {_device.GetName()}...");
        _device.SetVolume(0);
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class BridgeDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== BRIDGE PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Example 1: Message Sending
        Console.WriteLine("=== EXAMPLE 1: Message Sending System ===\n");
        Console.WriteLine("Abstraction: Message types (Text, Urgent, Encrypted)");
        Console.WriteLine("Implementation: Senders (Email, SMS, Slack)\n");

        // Create different combinations
        Console.WriteLine("--- Text Message via Email ---");
        var textEmail = new TextMessage(new EmailSender(), "Hello from the Bridge pattern!");
        textEmail.Send("user@example.com");

        Console.WriteLine("--- Urgent Message via SMS ---");
        var urgentSMS = new UrgentMessage(new SMSSender(), "Server is down!");
        urgentSMS.Send("+1-555-1234");

        Console.WriteLine("--- Encrypted Message via Slack ---");
        var encryptedSlack = new EncryptedMessage(new SlackSender(), "Secret project details");
        encryptedSlack.Send("#secure-channel");

        Console.WriteLine("--- Text Message via SMS ---");
        var textSMS = new TextMessage(new SMSSender(), "Meeting at 3pm");
        textSMS.Send("+1-555-5678");

        Console.WriteLine("--- Urgent Message via Email ---");
        var urgentEmail = new UrgentMessage(new EmailSender(), "Critical bug detected!");
        urgentEmail.Send("team@example.com");

        // Example 2: Remote Control
        Console.WriteLine("\n=== EXAMPLE 2: Remote Control System ===\n");
        Console.WriteLine("Abstraction: Remote types (Basic, Advanced)");
        Console.WriteLine("Implementation: Devices (TV, Radio)\n");

        Console.WriteLine("--- Basic Remote controlling TV ---");
        var tv = new Television();
        var tvRemote = new BasicRemote(tv);
        tvRemote.TogglePower();
        tvRemote.VolumeUp();
        tvRemote.VolumeUp();
        tvRemote.VolumeDown();

        Console.WriteLine("\n--- Advanced Remote controlling Radio ---");
        var radio = new Radio();
        var radioRemote = new AdvancedRemote(radio);
        radioRemote.TogglePower();
        radioRemote.VolumeUp();
        radioRemote.Mute();  // Advanced feature

        Console.WriteLine("\n--- Advanced Remote controlling TV ---");
        var tvAdvancedRemote = new AdvancedRemote(tv);
        tvAdvancedRemote.VolumeUp();
        tvAdvancedRemote.VolumeUp();
        tvAdvancedRemote.Mute();

        Console.WriteLine("\n💡 Bridge Pattern Benefits:");
        Console.WriteLine("   ✅ Decouples abstraction from implementation");
        Console.WriteLine("   ✅ Both can vary independently");
        Console.WriteLine("   ✅ Avoids class explosion (no TextEmailMessage, TextSMSMessage, etc.)");
        Console.WriteLine("   ✅ Runtime binding - can switch implementations");
        Console.WriteLine("   ✅ Single Responsibility - separate concerns");

        Console.WriteLine("\n💡 Without Bridge (Class Explosion):");
        Console.WriteLine("   ❌ TextEmailMessage, TextSMSMessage, TextSlackMessage");
        Console.WriteLine("   ❌ UrgentEmailMessage, UrgentSMSMessage, UrgentSlackMessage");
        Console.WriteLine("   ❌ EncryptedEmailMessage, EncryptedSMSMessage, EncryptedSlackMessage");
        Console.WriteLine("   ❌ Total: 9 classes (3 message types × 3 senders)");

        Console.WriteLine("\n💡 With Bridge:");
        Console.WriteLine("   ✅ 3 message classes + 3 sender classes = 6 classes");
        Console.WriteLine("   ✅ Adding new message type: +1 class (not +3)");
        Console.WriteLine("   ✅ Adding new sender: +1 class (not +3)");

        Console.WriteLine("\n💡 Real-World Examples:");
        Console.WriteLine("   • GUI frameworks (Widget abstraction + Platform implementation)");
        Console.WriteLine("   • Database drivers (Query abstraction + DB implementation)");
        Console.WriteLine("   • Graphics (Shape abstraction + Rendering implementation)");
        Console.WriteLine("   • Notifications (Message abstraction + Channel implementation)");
    }
}

// Wrapper class for basic RemoteControl that allows instantiation
public class BasicRemote : RemoteControl
{
    public BasicRemote(IDevice device) : base(device)
    {
    }
}