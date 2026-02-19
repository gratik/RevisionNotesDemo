namespace RevisionNotesDemo.IoTEngineering;

public static class DeviceTwinAndDirectMethods
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== DEVICE TWIN AND DIRECT METHODS ===\n");
        Console.WriteLine("- Use desired properties for configuration rollout and convergence.");
        Console.WriteLine("- Use reported properties for state and diagnostic visibility.");
        Console.WriteLine("- Use direct methods for bounded operational commands with timeout and retries.");
        Console.WriteLine("- Keep command handlers idempotent and version-aware.\n");
    }
}
