# Best Practices

> Subject: [Advanced-CSharp](../README.md)

## Best Practices

### ✅ Reflection
- Cache Type and MemberInfo objects (expensive to retrieve)
- Use compiled expressions for hot paths
- Prefer generic constraints over reflection when possible
- Use reflection for frameworks/tools, not business logic
- Consider source generators (compile-time alternative)

### ✅ Attributes
- Keep attributes simple and data-focused
- Use AttributeUsage to control where applied
- Make attribute properties immutable when possible
- Document attribute behavior clearly
- Don't put complex logic in attributes

### ✅ Performance
- Measure impact with BenchmarkDotNet
- Cache reflection results
- Use expression trees for repeated calls
- Consider source generators for compile-time alternatives

---


