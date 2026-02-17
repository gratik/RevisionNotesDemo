# Mocking with Moq

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

