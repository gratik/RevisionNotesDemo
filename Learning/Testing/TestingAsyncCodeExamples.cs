// ==============================================================================
// TESTING ASYNC CODE - Comprehensive Async/Await Testing
// Reference: Revision Notes - Unit Testing Best Practices
// ==============================================================================
// PURPOSE: Demonstrate testing async methods, cancellation, concurrency, timeouts
// KEY CONCEPTS: async/await, CancellationToken, Task.WhenAll, TaskCompletionSource
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RevisionNotesDemo.Testing;

// Sample async service
public class AsyncDataService
{
    public async Task<string> FetchDataAsync()
    {
        await Task.Delay(100);
        return "Data fetched";
    }

    public async Task<string> FetchDataAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
        return "Data fetched";
    }

    public async Task<int> ProcessItemsAsync(List<int> items, CancellationToken cancellationToken = default)
    {
        int count = 0;
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(10, cancellationToken);
            count++;
        }
        return count;
    }

    public async Task ProcessConcurrentlyAsync(int count)
    {
        var tasks = new List<Task>();
        for (int i = 0; i < count; i++)
        {
            tasks.Add(Task.Run(async () => await Task.Delay(50)));
        }
        await Task.WhenAll(tasks);
    }

    public async Task<string> SlowOperationAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        return "Completed";
    }
}

public class TestingAsyncCodeExamples
{
    /// <summary>
    /// EXAMPLE 1: Basic Async Test Methods
    /// </summary>
    public static async Task BasicAsyncTestExample()
    {
        Console.WriteLine("\n=== EXAMPLE 1: Basic Async Tests ===");

        Console.WriteLine("\n📝 xUnit Async Test:");
        Console.WriteLine(@"
[Fact]
public async Task FetchData_ReturnsData()
{
    // Arrange
    var service = new AsyncDataService();
    
    // Act
    var result = await service.FetchDataAsync();
    
    // Assert
    Assert.Equal(""Data fetched"", result);
}");

        Console.WriteLine("\n📝 NUnit Async Test:");
        Console.WriteLine(@"
[Test]
public async Task FetchData_ReturnsData()
{
    var service = new AsyncDataService();
    var result = await service.FetchDataAsync();
    Assert.That(result, Is.EqualTo(""Data fetched""));
}");

        Console.WriteLine("\n✅ Key: Return Task, use async/await");
    }

    /// <summary>
    /// EXAMPLE 2: Testing Async Exceptions
    /// </summary>
    public static async Task AsyncExceptionsExample()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Testing Async Exceptions ===");

        Console.WriteLine("\n📝 xUnit:");
        Console.WriteLine(@"
[Fact]
public async Task Method_ThrowsException()
{
    var service = new AsyncService();
    
    await Assert.ThrowsAsync<InvalidOperationException>(
        async () => await service.FailingMethodAsync()
    );
}");

        Console.WriteLine("\n📝 NUnit:");
        Console.WriteLine(@"
[Test]
public void Method_ThrowsException()
{
    var service = new AsyncService();
    
    Assert.ThrowsAsync<InvalidOperationException>(
        async () => await service.FailingMethodAsync()
    );
}");

        Console.WriteLine("\n✅ Use ThrowsAsync for async exceptions");
    }

    /// <summary>
    /// EXAMPLE 3: Testing Cancellation
    /// </summary>
    public static async Task CancellationExample()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Testing Cancellation ===");

        Console.WriteLine("\n📝 Basic Cancellation Test:");
        Console.WriteLine(@"
[Fact]
public async Task FetchData_CancelsOperation()
{
    // Arrange
    var service = new AsyncDataService();
    var cts = new CancellationTokenSource();
    
    // Act
    var task = service.FetchDataAsync(cts.Token);
    cts.Cancel();  // Cancel immediately
    
    // Assert
    await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
}");

        Console.WriteLine("\n📝 Mid-Operation Cancellation:");
        Console.WriteLine(@"
[Fact]
public async Task ProcessItems_CancelsMidOperation()
{
    var service = new AsyncDataService();
    var items = Enumerable.Range(1, 1000).ToList();
    var cts = new CancellationTokenSource();
    
    // Cancel after 50ms
    var task = service.ProcessItemsAsync(items, cts.Token);
    await Task.Delay(50);
    cts.Cancel();
    
    await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
}");

        Console.WriteLine("\n✅ Test both immediate and mid-operation cancellation");
    }

    /// <summary>
    /// EXAMPLE 4: Testing Concurrency
    /// </summary>
    public static async Task ConcurrencyExample()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Testing Concurrency ===");

        Console.WriteLine("\n📝 Concurrent Operations Test:");
        Console.WriteLine(@"
[Fact]
public async Task ProcessConcurrently_ProcessesAll()
{
    // Arrange
    var service = new AsyncDataService();
    var items = new List<int>();
    var processedCount = 0;
    
    // Act  - 100 concurrent tasks
    var tasks = Enumerable.Range(1, 100).Select(async i =>
    {
        await Task.Delay(10);
        Interlocked.Increment(ref processedCount);
    });
    
    await Task.WhenAll(tasks);
    
    // Assert
    Assert.Equal(100, processedCount);
}");

        Console.WriteLine("\n✅ Use Task.WhenAll to test concurrent operations");
    }

    /// <summary>
    /// EXAMPLE 5: Testing Timeouts
    /// </summary>
    public static async Task TimeoutsExample()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Testing Timeouts ===");

        Console.WriteLine("\n📝 CancellationTokenSource with Timeout:");
        Console.WriteLine(@"
[Fact]
public async Task SlowOperation_TimesOut()
{
    var service = new AsyncDataService();
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    
    await Assert.ThrowsAsync<OperationCanceledException>(
        async () => await service.SlowOperationAsync(cts.Token)
    );
}");

        Console.WriteLine("\n📝 xUnit Timeout Attribute:");
        Console.WriteLine(@"
[Fact(Timeout = 5000)]  // 5 seconds
public async Task Operation_CompletesInTime()
{
    var service = new AsyncDataService();
    await service.FastOperationAsync();
}");

        Console.WriteLine("\n✅ Test that operations complete or timeout as expected");
    }

    /// <summary>
    /// EXAMPLE 6: Mocking Async Methods
    /// </summary>
    public static void MockingAsyncExample()
    {
        Console.WriteLine("\n=== EXAMPLE 6: Mocking Async Methods ===");

        Console.WriteLine("\n📝 Moq - ReturnsAsync:");
        Console.WriteLine(@"
var mockService = new Mock<IAsyncService>();
mockService.Setup(s => s.FetchDataAsync())
    .ReturnsAsync(""Mocked data"");

// Or with Task.FromResult
mockService.Setup(s => s.FetchDataAsync())
    .Returns(Task.FromResult(""Mocked data""));");

        Console.WriteLine("\n📝 Mock with Delay:");
        Console.WriteLine(@"
mockService.Setup(s => s.FetchDataAsync())
    .ReturnsAsync(""Data"", TimeSpan.FromMilliseconds(100));

// Or custom async lambda
mockService.Setup(s => s.FetchDataAsync())
    .Returns(async () =>
    {
        await Task.Delay(100);
        return ""Data"";
    });");

        Console.WriteLine("\n✅ Use ReturnsAsync or async lambda for mocking");
    }

    /// <summary>
    /// EXAMPLE 7: Testing Task Completion States
    /// </summary>
    public static async Task TaskCompletionStatesExample()
    {
        Console.WriteLine("\n=== EXAMPLE 7: Task Completion States ===");

        Console.WriteLine("\n📝 Check Task States:");
        Console.WriteLine(@"
[Fact]
public async Task Task_CompletesSuccessfully()
{
    var service = new AsyncDataService();
    var task = service.FetchDataAsync();
    
    await task;
    
    Assert.True(task.IsCompleted);
    Assert.False(task.IsFaulted);
    Assert.False(task.IsCanceled);
    Assert.Equal(""Data fetched"", task.Result);
}

[Fact]
public async Task Task_IsCanceled()
{
    var cts = new CancellationTokenSource();
    cts.Cancel();
    
    var task = service.FetchDataAsync(cts.Token);
    
    try { await task; }
    catch { }
    
    Assert.True(task.IsCanceled);
    Assert.False(task.IsCompleted);
}

[Fact]
public async Task Task_IsFaulted()
{
    var task = service.FailingMethodAsync();
    
    try { await task; }
    catch { }
    
    Assert.True(task.IsFaulted);
    Assert.NotNull(task.Exception);
}");

        Console.WriteLine("\n✅ Check IsCompleted, IsFaulted, IsCanceled states");
    }

    /// <summary>
    /// EXAMPLE 8: Testing Parallel Execution Performance
    /// </summary>
    public static async Task ParallelExecutionExample()
    {
        Console.WriteLine("\n=== EXAMPLE 8: Testing Parallel Execution ===");

        Console.WriteLine("\n📝 Verify Parallel Performance:");
        Console.WriteLine(@"
[Fact]
public async Task ParallelExecution_IsFasterThanSequential()
{
    var items = Enumerable.Range(1, 10).ToList();
    
    // Sequential
    var sw = Stopwatch.StartNew();
    foreach (var item in items)
    {
        await Task.Delay(100);  // 10 * 100ms = 1000ms
    }
    sw.Stop();
    var sequentialTime = sw.ElapsedMilliseconds;
    
    // Parallel
    sw.Restart();
    await Task.WhenAll(items.Select(async item => await Task.Delay(100)));
    sw.Stop();
    var parallelTime = sw.ElapsedMilliseconds;
    
    Assert.True(parallelTime < sequentialTime / 2);  // At least 2x faster
}");

        Console.WriteLine("\n✅ Verify parallel operations are actually parallel");
    }

    /// <summary>
    /// EXAMPLE 9: Testing Async Void (TaskCompletionSource)
    /// </summary>
    public static void AsyncVoidExample()
    {
        Console.WriteLine("\n=== EXAMPLE 9: Testing Async Void ===");

        Console.WriteLine("\n❌ Problem: Can't await async void");
        Console.WriteLine(@"
public async void ProcessAsync()
{
    await Task.Delay(100);
    // Can't test this easily!
}");

        Console.WriteLine("\n✅ Solution: TaskCompletionSource");
        Console.WriteLine(@"
public class Service
{
    public event EventHandler<string> DataProcessed;
    
    public async void ProcessAsync()
    {
        await Task.Delay(100);
        DataProcessed?.Invoke(this, ""Processed"");
    }
}

[Fact]
public async Task ProcessAsync_RaisesEvent()
{
    var service = new Service();
    var tcs = new TaskCompletionSource<string>();
    
    service.DataProcessed += (sender, data) => tcs.SetResult(data);
    
    service.ProcessAsync();
    
    var result = await tcs.Task;
    Assert.Equal(""Processed"", result);
}");

        Console.WriteLine("\n✅ Use TaskCompletionSource to test async void");
    }

    /// <summary>
    /// Best Practices
    /// </summary>
    public static void BestPractices()
    {
        Console.WriteLine("\n=== TESTING ASYNC CODE - BEST PRACTICES ===");
        Console.WriteLine("✅ Always await async calls in tests");
        Console.WriteLine("✅ Test both success and failure paths");
        Console.WriteLine("✅ Test cancellation (immediate and mid-operation)");
        Console.WriteLine("✅ Use timeouts to prevent hanging tests");
        Console.WriteLine("✅ Test concurrent scenarios with Task.WhenAll");
        Console.WriteLine("✅ Mock async methods with ReturnsAsync");
        Console.WriteLine("✅ Be careful with timing-based assertions");
        Console.WriteLine("✅ Use TaskCompletionSource for async void");
        Console.WriteLine("✅ Verify task states (IsCompleted, IsFaulted, IsCanceled)");
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== TESTING ASYNC CODE EXAMPLES ===\n");
        await BasicAsyncTestExample();
        await AsyncExceptionsExample();
        await CancellationExample();
        await ConcurrencyExample();
        await TimeoutsExample();
        MockingAsyncExample();
        await TaskCompletionStatesExample();
        await ParallelExecutionExample();
        AsyncVoidExample();
        BestPractices();
        Console.WriteLine("\nTesting Async Code examples completed!\n");
    }
}
