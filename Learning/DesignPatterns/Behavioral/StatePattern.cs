// ============================================================================
// STATE PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// PURPOSE: "Allows an object to alter its behavior when its internal state changes."
// EXAMPLE: Traffic light system.
// ============================================================================

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
