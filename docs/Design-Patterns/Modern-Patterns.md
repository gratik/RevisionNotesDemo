# Modern Patterns

> Subject: [Design-Patterns](../README.md)

## Modern Patterns

### CQRS (Command Query Responsibility Segregation)

**Problem**: Separate read and write concerns

```csharp
// ✅ CQRS pattern
// Commands (write)
public record CreateUserCommand(string Name, string Email);

public class CreateUserCommandHandler
{
    private readonly IUserRepository _repository;
    
    public async Task<int> HandleAsync(CreateUserCommand command)
    {
        var user = new User { Name = command.Name, Email = command.Email };
        await _repository.AddAsync(user);
        return user.Id;
    }
}

// Queries (read)
public record GetUserQuery(int Id);

public class GetUserQueryHandler
{
    private readonly IUserRepository _repository;
    
    public async Task<UserDto?> HandleAsync(GetUserQuery query)
    {
        var user = await _repository.GetByIdAsync(query.Id);
        return user == null ? null : new UserDto { /* ... */ };
    }
}
```

---


