using RevisionNotesDemo.Demo;

namespace RevisionNotesDemo.UnitTests;

public class DemoSectionSelectorTests
{
    private static readonly IReadOnlyList<DemoSection> Sections =
    [
        new("oop", "PART 1", ["1", "principles"], () => Task.CompletedTask),
        new("memory", "PART 6", ["6"], () => Task.CompletedTask),
        new("async", "PART 7", ["7", "multithreading"], () => Task.CompletedTask)
    ];

    [Fact]
    public void ResolveNoSectionArgReturnsAllSections()
    {
        var result = DemoSectionSelector.Resolve([], Sections);

        Assert.Equal(["oop", "memory", "async"], result.SelectedKeys);
        Assert.Empty(result.UnknownKeys);
    }

    [Fact]
    public void ResolveSectionByKeyReturnsOnlyMatchingSection()
    {
        var result = DemoSectionSelector.Resolve(["--section", "memory"], Sections);

        Assert.Equal(["memory"], result.SelectedKeys);
        Assert.Empty(result.UnknownKeys);
    }

    [Fact]
    public void ResolveSectionByAliasAndCsvReturnsOrderedMatches()
    {
        var result = DemoSectionSelector.Resolve(["--section=6,async"], Sections);

        Assert.Equal(["memory", "async"], result.SelectedKeys);
        Assert.Empty(result.UnknownKeys);
    }

    [Fact]
    public void ResolveAllKeywordReturnsAllSections()
    {
        var result = DemoSectionSelector.Resolve(["--section", "all"], Sections);

        Assert.Equal(["oop", "memory", "async"], result.SelectedKeys);
        Assert.Empty(result.UnknownKeys);
    }

    [Fact]
    public void ResolveUnknownSectionReturnsUnknownAndNoSelection()
    {
        var result = DemoSectionSelector.Resolve(["--section", "nope"], Sections);

        Assert.Empty(result.SelectedKeys);
        Assert.Equal(["nope"], result.UnknownKeys);
    }

    [Fact]
    public void ResolveMissingSectionValueTreatedAsAllSections()
    {
        var result = DemoSectionSelector.Resolve(["--section"], Sections);

        Assert.Equal(["oop", "memory", "async"], result.SelectedKeys);
        Assert.Empty(result.UnknownKeys);
    }
}
