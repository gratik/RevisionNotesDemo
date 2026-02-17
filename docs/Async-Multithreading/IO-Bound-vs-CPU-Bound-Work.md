# I/O-Bound vs CPU-Bound Work

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Async-Multithreading](../README.md)

## I/O-Bound vs CPU-Bound Work

### The Fundamental Difference

**I/O-Bound:** Waiting for external operations (network, disk, database)

- **Use**: `async`/`await` with `Task`
- **Why**: Thread is freed while waiting, doesn't block
- **Examples**: HTTP calls, file I/O, database queries

**CPU-Bound:** Computing results (calculations, encryption, parsing)

- **Use**: `Task.Run()` or `Parallel` APIs
- **Why**: Distributes work across multiple cores
- **Examples**: Image processing, data analysis, cryptography

```csharp
// ✅ I/O-Bound: Use async/await
public async Task<string> GetDataAsync()
{
    // Thread is freed during HTTP call
    return await _httpClient.GetStringAsync("https://api.example.com/data");
}

// ✅ CPU-Bound: Use Task.Run to offload to thread pool
public async Task<int> CalculatePrimesAsync(int max)
{
    // Offload heavy computation to thread pool
    return await Task.Run(() =>
    {
        var primes = new List<int>();
        for (int i = 2; i < max; i++)
            if (IsPrime(i)) primes.Add(i);
        return primes.Count;
    });
}
```

### Decision Tree

```
Does work involve waiting (network/disk/DB)?
    → Yes: Use async/await (I/O-bound)
    → No: Is it heavy computation?
        → Yes: Use Task.Run or Parallel (CPU-bound)
        → No: Run synchronously
```

---

## Detailed Guidance

I/O-Bound vs CPU-Bound Work guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for I/O-Bound vs CPU-Bound Work before implementation work begins.
- Keep boundaries explicit so I/O-Bound vs CPU-Bound Work decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring I/O-Bound vs CPU-Bound Work in production-facing code.
- When performance, correctness, or maintainability depends on consistent I/O-Bound vs CPU-Bound Work decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying I/O-Bound vs CPU-Bound Work as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where I/O-Bound vs CPU-Bound Work is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for I/O-Bound vs CPU-Bound Work are documented and reviewable.
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

