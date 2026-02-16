using RevisionNotesDemo.AdvancedCSharp;
using RevisionNotesDemo.Appendices;
using RevisionNotesDemo.AsyncMultithreading;
using RevisionNotesDemo.Architecture;
using RevisionNotesDemo.Cloud;
using RevisionNotesDemo.CoreCSharpFeatures;
using RevisionNotesDemo.DevOps;
using RevisionNotesDemo.DesignPatterns.Behavioral;
using RevisionNotesDemo.DesignPatterns.Creational;
using RevisionNotesDemo.DesignPatterns.Structural;
using RevisionNotesDemo.LINQAndQueries;
using RevisionNotesDemo.MemoryManagement;
using RevisionNotesDemo.Microservices;
using RevisionNotesDemo.Models;
using RevisionNotesDemo.OOPPrinciples;
using RevisionNotesDemo.Observability;
using RevisionNotesDemo.PracticalPatterns;
using RevisionNotesDemo.Security;

namespace RevisionNotesDemo.Demo;

public static class DemoOrchestrator
{
    private static readonly IReadOnlyList<DemoSection> Sections =
    [
        new("oop", "PART 1: OOP PRINCIPLES", ["1", "oop", "principles", "oopprinciples"], OopSectionRunner.RunAsync),
        new("clean-code", "PART 2: KISS, DRY, YAGNI, TDA", ["2", "clean-code", "kiss", "dry", "yagni", "tda"], CleanCodeSectionRunner.RunAsync),
        new("creational", "PART 3: DESIGN PATTERNS - CREATIONAL", ["3", "creational", "designpatterns-creational"], CreationalPatternsSectionRunner.RunAsync),
        new("structural", "PART 4: DESIGN PATTERNS - STRUCTURAL", ["4", "structural", "designpatterns-structural"], StructuralPatternsSectionRunner.RunAsync),
        new("behavioral", "PART 5: DESIGN PATTERNS - BEHAVIORAL", ["5", "behavioral", "designpatterns-behavioral"], BehavioralPatternsSectionRunner.RunAsync),
        new("memory", "PART 6: MEMORY MANAGEMENT", ["6", "memory", "memorymanagement"], MemorySectionRunner.RunAsync),
        new("async", "PART 7: MULTITHREADING & ASYNC", ["7", "async", "multithreading", "asyncmultithreading"], AsyncSectionRunner.RunAsync),
        new("dotnet", "PART 8: .NET FRAMEWORK CONCEPTS", ["8", "dotnet", "core-csharp", "linq", "advanced-csharp", "corecsharpfeatures", "linqandqueries", "advancedcsharp"], DotNetConceptsSectionRunner.RunAsync),
        new("practical", "PART 9: PRACTICAL SCENARIOS", ["9", "practical", "practicalpatterns"], PracticalSectionRunner.RunAsync),
        new("appendices", "PART 10: APPENDICES", ["10", "appendices"], AppendicesSectionRunner.RunAsync),
        new("cloud", "PART 11: CLOUD & AZURE HOSTING", ["11", "azure"], CloudSectionRunner.RunAsync),
        new("microservices", "PART 12: MICROSERVICES", ["12", "services"], MicroservicesSectionRunner.RunAsync),
        new("architecture", "PART 13: ARCHITECTURE", ["13", "arch"], ArchitectureSectionRunner.RunAsync),
        new("devops", "PART 14: DEVOPS", ["14", "ci-cd", "deployment"], DevOpsSectionRunner.RunAsync),
        new("observability", "PART 15: OBSERVABILITY", ["15", "monitoring", "telemetry"], ObservabilitySectionRunner.RunAsync),
        new("security", "PART 16: SECURITY", ["16", "sec"], SecuritySectionRunner.RunAsync)
    ];

    private static readonly IReadOnlyDictionary<string, string> SectionFolders =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["oop"] = "Learning/OOPPrinciples",
            ["clean-code"] = "Learning/OOPPrinciples",
            ["creational"] = "Learning/DesignPatterns/Creational",
            ["structural"] = "Learning/DesignPatterns/Structural",
            ["behavioral"] = "Learning/DesignPatterns/Behavioral",
            ["memory"] = "Learning/MemoryManagement",
            ["async"] = "Learning/AsyncMultithreading",
            ["dotnet"] = "Learning/CoreCSharpFeatures + Learning/LINQAndQueries + Learning/AdvancedCSharp",
            ["practical"] = "Learning/PracticalPatterns + Learning/Models",
            ["appendices"] = "Learning/Appendices",
            ["cloud"] = "Learning/Cloud",
            ["microservices"] = "Learning/Microservices",
            ["architecture"] = "Learning/Architecture",
            ["devops"] = "Learning/DevOps",
            ["observability"] = "Learning/Observability",
            ["security"] = "Learning/Security"
        };

    public static async Task RunAsync(string[] args)
    {
        var selection = DemoSectionSelector.Resolve(args, Sections);

        if (selection.UnknownKeys.Count > 0)
        {
            Console.WriteLine($"\nUnknown section(s): {string.Join(", ", selection.UnknownKeys)}");
            PrintUsage();
            return;
        }

        var selectedSections = Sections
            .Where(section => selection.SelectedKeys.Contains(section.Key, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (selectedSections.Count == 0)
        {
            PrintUsage();
            return;
        }

        Console.WriteLine($"\nRunning sections: {string.Join(", ", selectedSections.Select(x => x.Key))}");

        foreach (var section in selectedSections)
        {
            PrintSectionHeader(section.Title);
            await section.RunAsync();
        }

        PrintCompletionMessage(selectedSections.Count == Sections.Count);
        Console.WriteLine("\n\nPress Ctrl+C to exit, or the application will continue running...\n");
    }

    private static void PrintSectionHeader(string title)
    {
        Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
        Console.WriteLine($"  {title}");
        Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
    }

    private static void PrintCompletionMessage(bool ranAllSections)
    {
        if (ranAllSections)
        {
            Console.WriteLine("\n\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✅ ALL DEMONSTRATIONS COMPLETED SUCCESSFULLY!                  ║");
            Console.WriteLine("║  Every principle from the Revision Notes has been demonstrated ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            return;
        }

        Console.WriteLine("\n\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  ✅ SELECTED DEMONSTRATIONS COMPLETED SUCCESSFULLY!             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run [--section <name[,name...]>]");
        Console.WriteLine("Available sections:");

        foreach (var section in Sections)
        {
            var folder = SectionFolders.TryGetValue(section.Key, out var value) ? value : "Learning/";
            Console.WriteLine($"  - {section.Key} ({folder})");
        }

        Console.WriteLine("Tip: folder-name aliases are supported (example: --section memorymanagement).");
    }
}

internal static class DemoSectionSelector
{
    public static DemoSectionSelection Resolve(string[] args, IReadOnlyList<DemoSection> sections)
    {
        var sectionArg = GetSectionArgument(args);

        if (string.IsNullOrWhiteSpace(sectionArg))
        {
            return new DemoSectionSelection(sections.Select(x => x.Key).ToList(), []);
        }

        var requested = sectionArg
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .Distinct()
            .ToList();

        if (requested.Count == 0 || requested.Contains("all"))
        {
            return new DemoSectionSelection(sections.Select(x => x.Key).ToList(), []);
        }

        var unknown = requested
            .Where(request => !sections.Any(section => section.Matches(request)))
            .ToList();

        if (unknown.Count > 0)
        {
            return new DemoSectionSelection([], unknown);
        }

        var selectedKeys = sections
            .Where(section => requested.Any(request => section.Matches(request)))
            .Select(section => section.Key)
            .ToList();

        return new DemoSectionSelection(selectedKeys, []);
    }

    private static string? GetSectionArgument(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--section", StringComparison.OrdinalIgnoreCase))
            {
                return i + 1 < args.Length ? args[i + 1] : string.Empty;
            }

            if (args[i].StartsWith("--section=", StringComparison.OrdinalIgnoreCase))
            {
                return args[i]["--section=".Length..];
            }
        }

        return null;
    }
}

internal sealed record DemoSectionSelection(IReadOnlyList<string> SelectedKeys, IReadOnlyList<string> UnknownKeys);

internal sealed record DemoSection(string Key, string Title, IReadOnlyList<string> Aliases, Func<Task> RunAsync)
{
    public bool Matches(string value)
    {
        if (Key.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Aliases.Any(alias => alias.Equals(value, StringComparison.OrdinalIgnoreCase));
    }
}

internal static class OopSectionRunner
{
    public static Task RunAsync()
    {
        SRPDemo.RunDemo();
        OCPDemo.RunDemo();
        LSPDemo.RunDemo();
        ISPDemo.RunDemo();
        DIPDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class CleanCodeSectionRunner
{
    public static Task RunAsync()
    {
        KISSDRYYAGNIDemo.RunDemo();
        TDADemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class CreationalPatternsSectionRunner
{
    public static Task RunAsync()
    {
        SingletonDemo.RunDemo();
        FactoryMethodDemo.RunDemo();
        AbstractFactoryDemo.RunDemo();
        BuilderDemo.RunDemo();
        PrototypeDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class StructuralPatternsSectionRunner
{
    public static async Task RunAsync()
    {
        AdapterDemo.RunDemo();
        DecoratorDemo.RunDemo();
        FacadeDemo.RunDemo();
        CompositeDemo.RunDemo();
        ProxyDemo.RunDemo();
        CQRSDemo.RunDemo();
        await RepositoryDemo.RunDemoAsync();
        UnitOfWorkDemo.RunDemo();
        FlyweightDemo.RunDemo();
        BridgeDemo.RunDemo();
    }
}

internal static class BehavioralPatternsSectionRunner
{
    public static Task RunAsync()
    {
        ObserverDemo.RunDemo();
        StrategyDemo.RunDemo();
        CommandDemo.RunDemo();
        MediatorDemo.RunDemo();
        StateDemo.RunDemo();
        ChainOfResponsibilityDemo.RunDemo();
        SpecificationDemo.RunDemo();
        NullObjectDemo.RunDemo();
        TemplateMethodDemo.RunDemo();
        VisitorDemo.RunDemo();
        MementoDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class MemorySectionRunner
{
    public static Task RunAsync()
    {
        StackVsHeapDemo.RunDemo();
        GarbageCollectionDemo.RunDemo();
        MemoryLeakDemo.RunDemo();
        StructVsClassDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class AsyncSectionRunner
{
    public static async Task RunAsync()
    {
        await TaskThreadValueTaskDemo.RunDemo();
        await AsyncAwaitDemo.RunDemo();
        await DeadlockPreventionDemo.RunDemo();
        ConcurrentCollectionsDemo.RunDemo();
    }
}

internal static class DotNetConceptsSectionRunner
{
    public static Task RunAsync()
    {
        AbstractVsInterfaceDemo.RunDemo();
        PolymorphismDemo.RunDemo();
        CovarianceContravarianceDemo.RunDemo();
        ExtensionMethodsDemo.RunDemo();
        IQueryableVsIEnumerableDemo.RunDemo();
        LINQExamples.RunDemo();
        GenericsDemo.RunDemo();
        DelegatesAndEventsDemo.RunDemo();
        ReflectionDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class PracticalSectionRunner
{
    public static async Task RunAsync()
    {
        OptionsPatternDemo.RunDemo();
        await BackgroundServicesDemo.RunDemoAsync();
        ValidationPatternsDemo.RunDemo();
        MappingPatternsDemo.RunDemo();
        CachingDemo.RunDemo();
        await ApiOptimizationDemo.RunDemoAsync();
        GlobalExceptionHandlingDemo.RunDemo();
        CommonModelsDemo.RunDemo();
        SerializationDemo.RunDemo();
    }
}

internal static class AppendicesSectionRunner
{
    public static Task RunAsync()
    {
        PatternsOverratedNowDemo.RunDemo();
        QuickReferenceTablesDemo.RunDemo();
        CommonInterviewQuestionsDemo.RunDemo();
        return Task.CompletedTask;
    }
}

internal static class CloudSectionRunner
{
    public static Task RunAsync()
    {
        AzureAppServicePatterns.RunAll();
        AzureFunctionsServerless.RunAll();
        AzureStoragePatterns.RunAll();
        AzureCosmosDBPatterns.RunAll();
        AzureKeyVaultSecrets.RunAll();
        AzureDockerHostingPatterns.RunAll();
        AzureFunctionsWithDocker.RunAll();
        AzureMicroservicesHosting.RunAll();
        AzureStorageAndDataHosting.RunAll();
        AzureDeploymentAndDevOps.RunAll();
        return Task.CompletedTask;
    }
}

internal static class MicroservicesSectionRunner
{
    public static Task RunAsync()
    {
        MonolithVsMicroservices.RunAll();
        MicroservicesIntroduction.RunAll();
        ServiceDiscoveryPatterns.RunAll();
        APIGatewayPatterns.RunAll();
        EventDrivenArchitecture.RunAll();
        ServiceCommunicationPatterns.RunAll();
        DistributedCachingAndCoherence.RunAll();
        ServiceMeshBasics.RunAll();
        DistributedTransactionsAndSaga.RunAll();
        DistributedConsistencyPatterns.RunAll();
        return Task.CompletedTask;
    }
}

internal static class ArchitectureSectionRunner
{
    public static Task RunAsync()
    {
        ArchitectureDecisionRecords.RunAll();
        ScalableProjectStructure.RunAll();
        CleanArchitectureAdvanced.RunAll();
        HexagonalArchitectureExamples.RunAll();
        VerticalSliceArchitecture.RunAll();
        EndToEndCaseStudy.RunAll();
        IntegratedDomainSlicesCaseStudy.RunAll();
        return Task.CompletedTask;
    }
}

internal static class DevOpsSectionRunner
{
    public static Task RunAsync()
    {
        GitWorkflowsAndBestPractices.RunAll();
        GitHubActionsWorkflows.RunAll();
        DockerComposeDevelopment.RunAll();
        KubernetesDeploymentPatterns.RunAll();
        HelmChartPackaging.RunAll();
        InfrastructureAsCodeTerraform.RunAll();
        AzureDevOpsPipelines.RunAll();
        return Task.CompletedTask;
    }
}

internal static class ObservabilitySectionRunner
{
    public static Task RunAsync()
    {
        StructuredLoggingAdvanced.RunAll();
        PrometheusAndGrafana.RunAll();
        OpenTelemetrySetup.RunAll();
        DistributedTracingJaegerZipkin.RunAll();
        ApplicationInsightsIntegration.RunAll();
        HealthChecksAndHeartbeats.RunAll();
        return Task.CompletedTask;
    }
}

internal static class SecuritySectionRunner
{
    public static Task RunAsync()
    {
        ManagedIdentityAndAuthentication.RunAll();
        OAuth2FlowsInDepth.RunAll();
        EncryptionAtRestAndInTransit.RunAll();
        CertificateManagementAndTLS.RunAll();
        CookieSessionAndTokenManagement.RunAll();
        MultiTenantAuthentication.RunAll();
        ContentSecurityPolicyCSP.RunAll();
        BiometricAndMFAPatterns.RunAll();
        SecureAPIDesignPatterns.RunAll();
        IdentityServerAndOpenIDConnect.RunAll();
        OWASPTop10WithExamples.RunAll();
        SecretRotationAndVaultPatterns.RunAll();
        return Task.CompletedTask;
    }
}
