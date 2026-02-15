using System.Reflection;
using RevisionNotesDemo.Demo;

namespace RevisionNotesDemo.UnitTests;

public class DemoOrchestratorSectionsTests
{
    [Fact]
    public void SectionsIncludeExpansionAreaRunners()
    {
        var sectionsField = typeof(DemoOrchestrator).GetField("Sections", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(sectionsField);

        var sections = Assert.IsAssignableFrom<IReadOnlyList<DemoSection>>(sectionsField!.GetValue(null));
        var keys = sections.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("cloud", keys);
        Assert.Contains("microservices", keys);
        Assert.Contains("architecture", keys);
        Assert.Contains("devops", keys);
        Assert.Contains("observability", keys);
        Assert.Contains("security", keys);
    }

    [Fact]
    public void SectionsMaintainExpectedOrderForCoreAndExpansion()
    {
        var sectionsField = typeof(DemoOrchestrator).GetField("Sections", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(sectionsField);

        var sections = Assert.IsAssignableFrom<IReadOnlyList<DemoSection>>(sectionsField!.GetValue(null));
        var keys = sections.Select(s => s.Key).ToList();

        Assert.Equal("oop", keys[0]);
        Assert.Equal("appendices", keys[9]);
        Assert.Equal("cloud", keys[10]);
        Assert.Equal("security", keys[^1]);
    }
}
