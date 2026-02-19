namespace RevisionNotesDemo.IoTEngineering;

public static class AzureIoTHubPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== AZURE IOT HUB PATTERNS ===\n");
        Console.WriteLine("- Use IoT Hub as secure device gateway with per-device identity.");
        Console.WriteLine("- Separate telemetry ingestion from command/management channels.");
        Console.WriteLine("- Route messages by type to downstream processors (alerts, storage, analytics).");
        Console.WriteLine("- Track ingest latency, dropped messages, and command timeout rate.\n");
    }
}
