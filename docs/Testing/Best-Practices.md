# Best Practices

> Subject: [Testing](../README.md)

## Best Practices

### ✅ Test Naming
```
MethodName_Scenario_ExpectedBehavior

Examples:
- Add_TwoPositiveNumbers_ReturnsSum
- GetUser_UserNotFound_ReturnsNull
- CreateOrder_InvalidData_ThrowsValidationException
```

### ✅ One Assert Per Test
```csharp
// ❌ BAD: Multiple asserts (which failed?)
[Fact]
public void Test_Bad()
{
    Assert.Equal(5, result.Count);
    Assert.True(result.All(x => x.IsActive));
    Assert.Equal("Alice", result.First().Name);
}

// ✅ GOOD: One logical assertion
[Fact]
public void Test_Good()
{
    Assert.Equal(5, result.Count);
}

[Fact]
public void Test_AllActive()
{
    Assert.All(result, x => Assert.True(x.IsActive));
}
```

### ✅ Test Independence
```csharp
// ❌ BAD: Tests depend on each other
private static int _counter = 0;

[Fact]
public void Test1() { _counter++; }  // ❌ Shared state

[Fact]
public void Test2() { Assert.Equal(1, _counter); }  // ❌ Depends on Test1

// ✅ GOOD: Each test is independent
[Fact]
public void Test1() 
{ 
    int counter = 0;
    counter++;
    Assert.Equal(1, counter);
}
```

### ✅ Fast Tests
- Avoid real databases (use in-memory)
- Avoid real HTTP calls (use mocks)
- Avoid Thread.Sleep (use Task.CompletedTask)
- Keep tests under 100ms

---


