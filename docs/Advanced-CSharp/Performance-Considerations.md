# Performance Considerations

> Subject: [Advanced-CSharp](../README.md)

## Performance Considerations

### Reflection is Slow

`csharp
// ❌ BAD: Reflection in hot path
for (int i = 0; i < 1000000; i++)
{
    PropertyInfo? prop = typeof(User).GetProperty("Name");
    prop?.SetValue(user, "Alice");  // ❌ Very slow!
}

// ✅ GOOD: Cache PropertyInfo
PropertyInfo? prop = typeof(User).GetProperty("Name");
for (int i = 0; i < 1000000; i++)
{
    prop?.SetValue(user, "Alice");  // ✅ Better
}

// ✅ BETTER: Use compiled expressions
var setter = CreateSetter<User, string>("Name");
for (int i = 0; i < 1000000; i++)
{
    setter(user, "Alice");  // ✅ Fast (near-native)
}
`

### Compiled Expressions for Performance

`csharp
// ✅ Create fast property setter using expressions
public static Action<T, TValue> CreateSetter<T, TValue>(string propertyName)
{
    var param1 = Expression.Parameter(typeof(T));
    var param2 = Expression.Parameter(typeof(TValue));
    var property = Expression.Property(param1, propertyName);
    var assign = Expression.Assign(property, param2);
    
    return Expression.Lambda<Action<T, TValue>>(assign, param1, param2).Compile();
}

// ✅ Create fast property getter
public static Func<T, TValue> CreateGetter<T, TValue>(string propertyName)
{
    var param = Expression.Parameter(typeof(T));
    var property = Expression.Property(param, propertyName);
    
    return Expression.Lambda<Func<T, TValue>>(property, param).Compile();
}
`

---


