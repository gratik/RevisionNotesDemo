# Parallel Execution Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Async-Multithreading](../README.md)

## Parallel Execution Patterns

### Task.WhenAll - Execute Multiple Tasks Concurrently

```csharp
// ✅ Parallel HTTP requests (all start immediately)
var urls = new[] { "url1", "url2", "url3" };
var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
var results = await Task.WhenAll(tasks);
// All 3 requests happen concurrently

// ❌ Sequential execution (one at a time)
var results = new List<string>();
foreach (var url in urls)
{
    results.Add(await _httpClient.GetStringAsync(url));  // ❌ Waits for each
}
```

### Task.WhenAny - First Completed Wins

```csharp
// Timeout pattern
var dataTask = FetchDataAsync();
var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

var completedTask = await Task.WhenAny(dataTask, timeoutTask);
if (completedTask == timeoutTask)
    throw new TimeoutException("Operation timed out");

return await dataTask;
```

### Parallel.ForEach - CPU-Bound Parallel Work

```csharp
// ✅ Process large collection in parallel (CPU-bound)
var images = GetImages();
Parallel.ForEach(images, image =>
{
    image.Resize(800, 600);
    image.ApplyFilter();
});

// Control parallelism
var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
Parallel.ForEach(items, options, item => ProcessItem(item));
```

---

## Detailed Guidance

Parallel Execution Patterns guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Parallel Execution Patterns before implementation work begins.
- Keep boundaries explicit so Parallel Execution Patterns decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Parallel Execution Patterns in production-facing code.
- When performance, correctness, or maintainability depends on consistent Parallel Execution Patterns decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Parallel Execution Patterns as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Parallel Execution Patterns is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Parallel Execution Patterns are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

