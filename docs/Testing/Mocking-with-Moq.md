# Mocking with Moq

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: xUnit basics, mocking concepts, and API behavior expectations.
- Related examples: docs/Testing/README.md
> Subject: [Testing](../README.md)

## Mocking with Moq

### Why Mock?

**Isolation**: Test one class without its dependencies
**Speed**: No database, network, or file I/O
**Control**: Simulate success, failure, edge cases

### Basic Mocking

```csharp
[Fact]
public void GetUser_UserExists_ReturnsUser()
{
    // Arrange: Create mock
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetById(1))
        .Returns(new User { Id = 1, Name = "Alice" });
    
    var service = new UserService(mockRepo.Object);
    
    // Act
    var user = service.GetUser(1);
    
    // Assert
    Assert.NotNull(user);
    Assert.Equal("Alice", user.Name);
}
```

### Verifying Interactions

```csharp
[Fact]
public void CreateUser_ValidUser_SavesCalled()
{
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);
    
    service.CreateUser(new User { Name = "Bob" });
    
    // ✅ Verify method was called once
    mockRepo.Verify(r => r.Save(It.IsAny<User>()), Times.Once);
}

[Fact]
public void DeleteUser_InvalidId_DoesNotCallDatabase()
{
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);
    
    service.DeleteUser(-1);  // Invalid ID
    
    // ✅ Verify method was never called
    mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
}
```

### Async Mocking

```csharp
[Fact]
public async Task GetUserAsync_UserExists_ReturnsUser()
{
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new User { Id = 1, Name = "Alice" });
    
    var service = new UserService(mockRepo.Object);
    
    var user = await service.GetUserAsync(1);
    
    Assert.NotNull(user);
}
```

### Exception Mocking

```csharp
[Fact]
public async Task GetUserAsync_DatabaseError_ThrowsException()
{
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ThrowsAsync(new DatabaseException("Connection failed"));
    
    var service = new UserService(mockRepo.Object);
    
    await Assert.ThrowsAsync<DatabaseException>(() => 
        service.GetUserAsync(1));
}
```

---


## Interview Answer Block
30-second answer:
- Mocking with Moq is about verification strategies across unit, integration, and system levels. It matters because testing quality determines confidence in safe refactoring and releases.
- Use it when building fast feedback loops and meaningful regression safety nets.

2-minute answer:
- Start with the problem Mocking with Moq solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: broader coverage vs build time and maintenance overhead.
- Close with one failure mode and mitigation: brittle tests that validate implementation details instead of behavior.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Mocking with Moq but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Mocking with Moq, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Mocking with Moq and map it to one concrete implementation in this module.
- 3 minutes: compare Mocking with Moq with an alternative, then walk through one failure mode and mitigation.