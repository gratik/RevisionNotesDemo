# Testing Frameworks Comparison

> Subject: [Testing](../README.md)

## Testing Frameworks Comparison

| Feature | xUnit | NUnit | MSTest |
|---------|-------|-------|--------|
| **Philosophy** | Modern, opinionated | Feature-rich, flexible | VS integrated |
| **Test Method** | `[Fact]` | `[Test]` | `[TestMethod]` |
| **Parameterized** | `[Theory]` + `[InlineData]` | `[TestCase]` | `[DataTestMethod]` + `[DataRow]` |
| **Setup** | Constructor | `[SetUp]` | `[TestInitialize]` |
| **Teardown** | `IDisposable` | `[TearDown]` | `[TestCleanup]` |
| **Assertions** | `Assert.Equal()` | `Assert.That()` | `Assert.AreEqual()` |
| **Parallel** | ✅ Default | ⚠️ Opt-in | ⚠️ Limited |
| **Popularity** | High (modern projects) | High (legacy) | Medium (VS users) |
| **ASP.NET Core** | ✅ Recommended | ✅ Supported | ✅ Supported |

### Recommendation

**Use xUnit** for new projects:
- Modern, clean API
- Parallel by default (faster)
- No global state
- Used by ASP.NET Core team

**Use NUnit** if:
- Existing projects already use it
- Need richer assertion API
- Prefer `[SetUp]`/`[TearDown]` pattern

**Use MSTest** if:
- Tight VS integration required
- Enterprise standardization

---


