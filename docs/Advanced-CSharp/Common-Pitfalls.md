# Common Pitfalls

> Subject: [Advanced-CSharp](../README.md)

## Common Pitfalls

### ❌ Not Caching Reflection Results

`csharp
// ❌ BAD: Repeated reflection calls
public void UpdateProperty(object obj, string propertyName, object value)
{
    var prop = obj.GetType().GetProperty(propertyName);  // ❌ Every time!
    prop?.SetValue(obj, value);
}

// ✅ GOOD: Cache results
private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _propertyCache = new();

public void UpdateProperty(object obj, string propertyName, object value)
{
    var key = (obj.GetType(), propertyName);
    var prop = _propertyCache.GetOrAdd(key, k => k.Item1.GetProperty(k.Item2));
    prop?.SetValue(obj, value);
}
`

### ❌ Ignoring Null Checks

`csharp
// ❌ BAD: Can throw NullReferenceException
PropertyInfo prop = type.GetProperty("NonExistent");  // null!
prop.SetValue(obj, value);  // ❌ Boom!

// ✅ GOOD: Check for null
PropertyInfo? prop = type.GetProperty("Name");
if (prop != null && prop.CanWrite)
{
    prop.SetValue(obj, value);
}
`

---


