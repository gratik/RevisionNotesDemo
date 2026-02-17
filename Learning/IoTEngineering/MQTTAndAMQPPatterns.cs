namespace RevisionNotesDemo.IoTEngineering;

public static class MQTTAndAMQPPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== MQTT AND AMQP PATTERNS ===\n");
        Console.WriteLine("- MQTT: lightweight, device-friendly, intermittent network tolerant.");
        Console.WriteLine("- AMQP: richer broker semantics, stronger enterprise integration options.");
        Console.WriteLine("- Select per workload: constrained edge nodes vs data-center workflows.");
        Console.WriteLine("- Standardize QoS/retry behavior and avoid protocol-specific assumptions in domain code.\n");
    }
}
