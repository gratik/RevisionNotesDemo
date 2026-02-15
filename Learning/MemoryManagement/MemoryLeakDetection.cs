// ============================================================================
// DETECTING MEMORY LEAKS
// Reference: Revision Notes - Page 9
// ============================================================================
// WHAT IS THIS?
// -------------
// Techniques to spot leaks from events, statics, and undisposed resources.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents unbounded memory growth
// ✅ Improves long-term stability of services
//
// WHEN TO USE
// -----------
// ✅ Long-running services and event-heavy UI apps
// ✅ Systems using unmanaged resources
//
// WHEN NOT TO USE
// ---------------
// ❌ Short-lived scripts where leaks are irrelevant
// ❌ Code paths that never persist beyond a request
//
// REAL-WORLD EXAMPLE
// ------------------
// Unsubscribe event handlers in `Dispose()`.
// ============================================================================

namespace RevisionNotesDemo.MemoryManagement;

// From Revision Notes - Page 9
// Pattern to avoid leaks: IDisposable + using + event unsubscription
public sealed class Publisher
{
    public event EventHandler? OnEvent;

    public void Raise()
    {
        Console.WriteLine("[MEMORY] Publisher raising event");
        OnEvent?.Invoke(this, EventArgs.Empty);
    }
}

public sealed class Subscriber : IDisposable
{
    private readonly Publisher _p;

    public Subscriber(Publisher p)
    {
        _p = p;
        _p.OnEvent += Handle; // subscribe
        Console.WriteLine("[MEMORY] Subscriber subscribed to event");
    }

    private void Handle(object? s, EventArgs e)
    {
        Console.WriteLine("[MEMORY] Subscriber handling event");
    }

    public void Dispose()
    {
        _p.OnEvent -= Handle; // unsubscribe to prevent leak
        Console.WriteLine("[MEMORY] Subscriber unsubscribed (leak prevented)");
        GC.SuppressFinalize(this);
    }
}

public class MemoryLeakDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== MEMORY LEAK DETECTION DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 9\n");

        var pub = new Publisher();

        Console.WriteLine("--- Proper pattern (using statement prevents leaks) ---");
        using (var sub = new Subscriber(pub))
        {
            pub.Raise();
        } // Dispose called automatically, event unsubscribed

        Console.WriteLine("\n--- After disposal ---");
        pub.Raise(); // Subscriber won't receive this

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Always unsubscribe from events");
        Console.WriteLine("   - Use IDisposable for cleanup");
        Console.WriteLine("   - Use 'using' statement");
        Console.WriteLine("   - Watch for static references holding objects");
    }
}
