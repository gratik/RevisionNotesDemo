# Mocking with Moq

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


