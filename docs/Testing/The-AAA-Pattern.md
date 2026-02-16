# The AAA Pattern

> Subject: [Testing](../README.md)

## The AAA Pattern

**Arrange, Act, Assert** - the standard test structure.

```csharp
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
    // Arrange: Set up test data and dependencies
    var calculator = new Calculator();
    int a = 5;
    int b = 3;
    
    // Act: Execute the method being tested
    var result = calculator.Add(a, b);
    
    // Assert: Verify the result
    Assert.Equal(8, result);
}
```

**Why AAA?**
- Clear test structure
- Easy to read and maintain
- Separates test concerns
- Industry standard

---


