// ==============================================================================
// PARTITIONING AND DATA LIFECYCLE PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class PartitioningAndDataLifecyclePatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- PARTITIONING AND DATA LIFECYCLE PATTERNS ---\n");
        ShowPartitioningStrategy();
        ShowSlidingWindow();
        ShowRetentionGuidance();
    }

    private static void ShowPartitioningStrategy()
    {
        Console.WriteLine("Partitioning strategy:");
        Console.WriteLine("- Partition very large tables by aligned date or tenant key.");
        Console.WriteLine("- Ensure common predicates include partitioning key.");
        Console.WriteLine("- Align clustered index with partitioning strategy where practical.\n");
    }

    private static void ShowSlidingWindow()
    {
        Console.WriteLine("Sliding window pattern:");
        Console.WriteLine("- Add new partition for incoming period.");
        Console.WriteLine("- Switch out oldest partition to archive table.");
        Console.WriteLine("- Truncate or archive switched partition safely.\n");
    }

    private static void ShowRetentionGuidance()
    {
        Console.WriteLine("Retention guidance:");
        Console.WriteLine("- Define hot/warm/cold storage classes.");
        Console.WriteLine("- Keep business retention requirements explicit.");
        Console.WriteLine("- Test restore and audit access for archived data.\n");
    }
}

