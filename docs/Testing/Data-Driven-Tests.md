# Data-Driven Tests

> Subject: [Testing](../README.md)

## Data-Driven Tests

### MemberData (Complex Data)

```csharp
public class CalculatorTests
{
    public static IEnumerable<object[]> AddTestData =>
        new List<object[]>
        {
            new object[] { 2, 3, 5 },
            new object[] { -1, 1, 0 },
            new object[] { 0, 0, 0 },
            new object[] { int.MaxValue, 0, int.MaxValue }
        };
    
    [Theory]
    [MemberData(nameof(AddTestData))]
    public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        Assert.Equal(expected, calculator.Add(a, b));
    }
}
```

### ClassData (Reusable Data)

```csharp
public class AddTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 2, 3, 5 };
        yield return new object[] { -1, 1, 0 };
        yield return new object[] { 0, 0, 0 };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(AddTestData))]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    Assert.Equal(expected, calculator.Add(a, b));
}
```

---


