// ==============================================================================
// DELEGATES AND EVENTS
// Reference: Revision Notes - .NET Framework - Page 11
// ==============================================================================
// WHAT IS THIS?
// -------------
// Delegates for callbacks and events for publish-subscribe patterns.
//
// WHY IT MATTERS
// --------------
// ✅ Decouples producers and consumers
// ✅ Enables multicast notifications
//
// WHEN TO USE
// -----------
// ✅ UI events, observer patterns, domain notifications
// ✅ Callback-based APIs and extensibility points
//
// WHEN NOT TO USE
// ---------------
// ❌ Simple direct calls with no subscribers
// ❌ Overusing events where async messaging fits better
//
// REAL-WORLD EXAMPLE
// ------------------
// Temperature sensor notifying displays.
// ==============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

// Custom delegate
public delegate void MessageDelegate(string message);
public delegate int MathOperation(int a, int b);

// ========================================================================
// EVENT EXAMPLE - Temperature Monitor
// ========================================================================

public class TemperatureChangedEventArgs : EventArgs
{
    public double OldTemperature { get; set; }
    public double NewTemperature { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TemperatureSensor
{
    private double _temperature;

    // Event using built-in EventHandler<T>
    public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

    public double Temperature
    {
        get => _temperature;
        set
        {
            if (Math.Abs(_temperature - value) > 0.01)
            {
                var oldTemp = _temperature;
                _temperature = value;
                OnTemperatureChanged(new TemperatureChangedEventArgs
                {
                    OldTemperature = oldTemp,
                    NewTemperature = value,
                    Timestamp = DateTime.Now
                });
            }
        }
    }

    protected virtual void OnTemperatureChanged(TemperatureChangedEventArgs e)
    {
        TemperatureChanged?.Invoke(this, e);
    }
}

public class TemperatureDisplay
{
    public void Subscribe(TemperatureSensor sensor)
    {
        sensor.TemperatureChanged += OnTemperatureChanged;
    }

    public void Unsubscribe(TemperatureSensor sensor)
    {
        sensor.TemperatureChanged -= OnTemperatureChanged;
    }

    private void OnTemperatureChanged(object? sender, TemperatureChangedEventArgs e)
    {
        Console.WriteLine($"[EVENT] 🌡️  Temperature changed: {e.OldTemperature:F1}°C → {e.NewTemperature:F1}°C");
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class DelegatesAndEventsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== DELEGATES AND EVENTS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - .NET Framework - Page 11\n");

        // 1. Custom Delegate
        Console.WriteLine("--- 1. Custom Delegates ---");
        MessageDelegate messageHandler = PrintMessage;
        messageHandler += LogMessage;  // Multicast
        messageHandler("Hello Delegates!");
        Console.WriteLine();

        // 2. Built-in Delegates
        Console.WriteLine("--- 2. Built-in Delegates ---");

        // Action<T> - void return
        Action<string> action = msg => Console.WriteLine($"[DELEGATE] Action: {msg}");
        action("Using Action<T>");

        // Func<T, TResult> - returns value
        Func<int, int, int> multiply = (a, b) => a * b;
        Console.WriteLine($"[DELEGATE] Func result: {multiply(5, 3)}");

        // Predicate<T> - returns bool
        Predicate<int> isEven = n => n % 2 == 0;
        Console.WriteLine($"[DELEGATE] Predicate (4 is even): {isEven(4)}\n");

        // 3. Lambda Expressions
        Console.WriteLine("--- 3. Lambda Expressions ---");
        MathOperation add = (a, b) => a + b;
        MathOperation subtract = (a, b) => a - b;
        Console.WriteLine($"[LAMBDA] 10 + 5 = {add(10, 5)}");
        Console.WriteLine($"[LAMBDA] 10 - 5 = {subtract(10, 5)}\n");

        // 4. Events
        Console.WriteLine("--- 4. Events (Publisher-Subscriber) ---");
        var sensor = new TemperatureSensor();
        var display = new TemperatureDisplay();

        display.Subscribe(sensor);

        sensor.Temperature = 20.5;
        sensor.Temperature = 25.0;
        sensor.Temperature = 30.5;

        Console.WriteLine("[EVENT] Unsubscribing display...");
        display.Unsubscribe(sensor);
        sensor.Temperature = 35.0;  // No output (unsubscribed)
        Console.WriteLine();

        // 5. Multicast Delegates
        Console.WriteLine("--- 5. Multicast Delegates ---");
        MathOperation operations = Add;
        operations += Multiply;
        operations += Subtract;

        Console.WriteLine("[MULTICAST] Invoking all operations with (6, 3):");
        operations(6, 3);  // All methods called in order
        Console.WriteLine();

        Console.WriteLine("💡 Delegates & Events Best Practices:");
        Console.WriteLine("   ✅ Use built-in Action/Func instead of custom delegates");
        Console.WriteLine("   ✅ Always check for null: event?.Invoke()");
        Console.WriteLine("   ✅ Unsubscribe from events to prevent memory leaks");
        Console.WriteLine("   ✅ Use EventHandler<T> for custom events");
        Console.WriteLine("   ✅ Make event-raising methods protected virtual");
    }

    private static void PrintMessage(string msg)
    {
        Console.WriteLine($"[DELEGATE] PrintMessage: {msg}");
    }

    private static void LogMessage(string msg)
    {
        Console.WriteLine($"[DELEGATE] LogMessage: Logged '{msg}'");
    }

    private static int Add(int a, int b)
    {
        Console.WriteLine($"[MULTICAST]   Add: {a} + {b} = {a + b}");
        return a + b;
    }

    private static int Multiply(int a, int b)
    {
        Console.WriteLine($"[MULTICAST]   Multiply: {a} * {b} = {a * b}");
        return a * b;
    }

    private static int Subtract(int a, int b)
    {
        Console.WriteLine($"[MULTICAST]   Subtract: {a} - {b} = {a - b}");
        return a - b;
    }
}
