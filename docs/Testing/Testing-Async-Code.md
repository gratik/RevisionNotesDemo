# Testing Async Code

> Subject: [Testing](../README.md)

## Testing Async Code

### Common Mistakes

```csharp
// ❌ BAD: Not awaiting async method
[Fact]
public void Test_Bad()  // ❌ Not async
{
    var result = service.GetDataAsync();  // ❌ Not awaited
    Assert.NotNull(result);  // ❌ Testing Task, not result!
}

// ✅ GOOD: Properly awaiting
[Fact]
public async Task Test_Good()  // ✅ async Task
{
    var result = await service.GetDataAsync();  // ✅ Awaited
    Assert.NotNull(result);  // ✅ Testing actual result
}
```

### Testing Timeouts

```csharp
[Fact(Timeout = 5000)]  // ✅ Fail if takes > 5 seconds
public async Task GetData_RespondsQuickly()
{
    var result = await service.GetDataAsync();
    Assert.NotNull(result);
}
```

### Testing Cancellation

```csharp
[Fact]
public async Task GetData_Cancelled_ThrowsOperationCanceledException()
{
    using var cts = new CancellationTokenSource();
    cts.Cancel();  // Cancel immediately
    
    await Assert.ThrowsAsync<OperationCanceledException>(() =>
        service.GetDataAsync(cts.Token));
}
```

---


