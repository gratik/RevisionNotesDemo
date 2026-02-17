# Modern Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Object-oriented design fundamentals and refactoring familiarity.
- Related examples: docs/Design-Patterns/README.md
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

## Detailed Guidance

Modern Patterns should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

### Design Notes
- Use language features to enforce intent at compile time (constraints, nullability, variance).
- Keep APIs narrow and intention-revealing; avoid generic over-engineering.
- Prefer composition and small interfaces over deep inheritance chains.
- Document where performance optimizations justify additional complexity.

### When To Use
- When building reusable libraries or framework-facing APIs.
- When replacing runtime casts/dynamic code with typed contracts.
- When teaching or reviewing core language design tradeoffs.

### Anti-Patterns To Avoid
- Public APIs with too many type parameters and unclear semantics.
- Constraints that do not correspond to required operations.
- Using reflection/dynamic where static typing is sufficient.

## Practical Example

- Start with a concrete implementation and extract generic behavior only when duplication appears.
- Add minimal constraints needed for compile-time guarantees.
- Validate with tests across reference and value type scenarios.

## Validation Checklist

- API signatures are understandable without deep internal context.
- Nullability and constraints match true invariants.
- Type misuse fails at compile time where possible.
- Benchmarks exist for any non-trivial performance optimizations.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Modern Patterns is about reusable design solutions for recurring software problems. It matters because pattern choice shapes long-term extensibility and readability.
- Use it when selecting pattern structure to simplify complex behavior.

2-minute answer:
- Start with the problem Modern Patterns solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: architectural consistency vs accidental overengineering.
- Close with one failure mode and mitigation: forcing patterns where straightforward code is enough.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Modern Patterns but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Modern Patterns, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Modern Patterns and map it to one concrete implementation in this module.
- 3 minutes: compare Modern Patterns with an alternative, then walk through one failure mode and mitigation.