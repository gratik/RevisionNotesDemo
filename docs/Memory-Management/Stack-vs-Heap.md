# Stack vs Heap

> Subject: [Memory-Management](../README.md)

## Stack vs Heap

### The Fundamental Difference

**Stack:**

- Fast allocation/deallocation (just move stack pointer)
- Automatic cleanup when method returns
- Fixed size per thread (~1MB)
- Stores value types and method call frames
- LIFO (Last In, First Out) structure

**Heap:**

- Slower allocation (GC managed)
- Cleanup happens during garbage collection
- Much larger (~TB available)
- Stores reference types (objects)
- Random access via references

### Allocation Patterns

```csharp
public void AllocationExample()
{
    // STACK: Value types allocated on stack
    int number = 42;              // ✅ Stack (4 bytes)
    DateTime date = DateTime.Now; // ✅ Stack (8 bytes)
    bool flag = true;             // ✅ Stack (1 byte)

    // HEAP: Reference types allocated on heap
    var user = new User();        // ✅ Heap (reference on stack)
    var list = new List<int>();   // ✅ Heap (reference on stack)
    var text = "Hello";           // ✅ Heap (string is reference type)

    // MIXED: Struct containing reference
    var point = new Point         // ✅ Stack (struct)
    {
        Name = "Origin"           // ✅ Heap (string reference)
    };
}

public struct Point  // Value type
{
    public int X;
    public int Y;
    public string Name;  // Reference to heap object
}
```

### What Goes Where?

| Type                         | Allocated On | Lifetime               | Examples                             |
| ---------------------------- | ------------ | ---------------------- | ------------------------------------ |
| **Value types** (local vars) | Stack        | Until method returns   | `int`, `bool`, `DateTime`, `struct`  |
| **Reference types**          | Heap         | Until GC collects      | `class`, `string`, `array`, `object` |
| **Value types in classes**   | Heap         | With containing object | Fields in classes                    |
| **Captured variables**       | Heap         | Closure lifetime       | Lambda captures                      |
| **Boxed value types**        | Heap         | Until GC collects      | `object x = 42;`                     |

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Stack vs Heap before implementation work begins.
- Keep boundaries explicit so Stack vs Heap decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Stack vs Heap in production-facing code.
- When performance, correctness, or maintainability depends on consistent Stack vs Heap decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Stack vs Heap as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Stack vs Heap is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Stack vs Heap are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

