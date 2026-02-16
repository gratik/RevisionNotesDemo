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


