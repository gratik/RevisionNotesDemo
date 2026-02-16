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


