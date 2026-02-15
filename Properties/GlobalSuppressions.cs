using System.Diagnostics.CodeAnalysis;

// Intentionally scoped suppressions for educational/demo namespaces where
// patterns are shown in multiple variants (including intentionally suboptimal).
[assembly: SuppressMessage(
    "Performance",
    "CA1848:Use the LoggerMessage delegates",
    Justification = "Logging modules are educational and intentionally demonstrate baseline logging calls.",
    Scope = "namespaceanddescendants",
    Target = "~N:RevisionNotesDemo.Logging")]

[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Performance modules use underscore method names for benchmark/comparison readability.",
    Scope = "namespaceanddescendants",
    Target = "~N:RevisionNotesDemo.Performance")]

[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Data access examples intentionally label anti-pattern and comparison variants.",
    Scope = "namespaceanddescendants",
    Target = "~N:RevisionNotesDemo.DataAccess")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Behavioral pattern examples use underscore suffixes to contrast bad/good variants.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo.DesignPatterns.Behavioral")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Logging test examples keep underscore naming to mirror xUnit naming style comparisons.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo.Logging")]

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Demonstration test-style names prioritize readability of scenario labels.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo.WebAPI.Versioning")]
[assembly: SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Educational examples intentionally use conventional suffixes (Queue/Check/Event) for discoverability.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Examples mirror common API naming patterns across tutorials.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Demo interfaces/classes intentionally preserve concise naming.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]

[assembly: SuppressMessage("Performance", "CA1805:Do not initialize unnecessarily", Justification = "Default initializations are intentionally explicit in teaching examples.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Interface-first samples intentionally demonstrate abstraction boundaries.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1840:Use Environment.CurrentManagedThreadId", Justification = "Thread examples intentionally show Thread.CurrentThread usage for conceptual clarity.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1860:Avoid using 'Enumerable.Any()' extension method", Justification = "LINQ-based samples emphasize readability over micro-optimizations.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Inline literals keep examples concise and easier to follow.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations", Justification = "Some examples intentionally use explicit array allocation for pedagogy.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1847:Use char literal for a single character lookup", Justification = "String-literal examples are kept to match related narrative text.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1866:Use char overload where applicable", Justification = "String API overload choices are intentionally varied for demonstration.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1845:Use span-based 'string.Concat'", Justification = "Examples prioritize readability over low-level span optimizations.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Enumerable.Count", Justification = "LINQ-focused samples intentionally use query-style operators.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]

[assembly: SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Certain DTO/sample types intentionally expose fields for brevity.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Generic examples intentionally show static-member behavior.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]

[assembly: SuppressMessage("Usage", "CA1510:Use ArgumentNullException.ThrowIfNull", Justification = "Examples include classic guard-clause style for comparison.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "ValueTask gotcha samples intentionally demonstrate incorrect double-consumption patterns.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo")]
[assembly: SuppressMessage("Security", "CA5350:Do not use weak cryptographic algorithms", Justification = "TOTP demonstration intentionally uses HMAC-SHA1 for RFC compatibility examples.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo.Security")]
[assembly: SuppressMessage("Usage", "ASPDEPR002:Avoid deprecated ASP.NET APIs", Justification = "Migration examples intentionally include deprecated API usage notes.", Scope = "namespaceanddescendants", Target = "~N:RevisionNotesDemo.WebAPI")]
