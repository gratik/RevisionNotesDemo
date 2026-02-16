# Common Pitfalls

> Subject: [Testing](../README.md)

## Common Pitfalls

### ❌ Not Testing Edge Cases
```csharp
// ❌ Only tests happy path
[Fact]
public void Divide_ValidNumbers_ReturnsQuotient()
{
    Assert.Equal(5, calculator.Divide(10, 2));
}

// ✅ Tests edge cases
[Theory]
[InlineData(10, 0)]  // Division by zero
[InlineData(int.MaxValue, 1)]  // Large numbers
[InlineData(-10, 2)]  // Negative numbers
public void Divide_EdgeCases_HandlesCorrectly(int a, int b)
{
    // Test appropriate behavior
}
```

### ❌ Testing Implementation Details
```csharp
// ❌ BAD: Testing private methods
[Fact]
public void TestPrivateMethod()
{
    var result = (int)typeof(Calculator)
        .GetMethod("PrivateAdd", BindingFlags.NonPublic)
        .Invoke(calculator, new object[] { 2, 3 });
}

// ✅ GOOD: Test public behavior
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    Assert.Equal(5, calculator.Add(2, 3));
}
```

---


