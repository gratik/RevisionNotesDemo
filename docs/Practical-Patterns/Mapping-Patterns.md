# Mapping Patterns

> Subject: [Practical-Patterns](../README.md)

## Mapping Patterns

### Manual Mapping

```csharp
// ✅ Explicit and clear
public class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
    
    public static User ToEntity(CreateUserRequest request)
    {
        return new User
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

### AutoMapper

```csharp
// ✅ Configuration
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Simple mapping
        CreateMap<User, UserDto>();
        
        // Custom mapping
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.ItemCount,
                opt => opt.MapFrom(src => src.Items.Count));
        
        // Reverse mapping
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}

// Usage
public class UserService
{
    private readonly IMapper _mapper;
    
    public UserDto GetUser(User user)
    {
        return _mapper.Map<UserDto>(user);
    }
}
```

---

## Detailed Guidance

Mapping Patterns guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Mapping Patterns before implementation work begins.
- Keep boundaries explicit so Mapping Patterns decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Mapping Patterns in production-facing code.
- When performance, correctness, or maintainability depends on consistent Mapping Patterns decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Mapping Patterns as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Mapping Patterns is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Mapping Patterns are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

