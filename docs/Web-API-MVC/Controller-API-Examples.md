# Controller API Examples

> Subject: [Web-API-MVC](../README.md)

## Controller API Examples

### RESTful Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    
    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }
    
    // GET api/users
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }
    
    // GET api/users/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        
        return Ok(user);
    }
    
    // POST api/users
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Create(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await _repository.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
    
    // DELETE api/users/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
}
```

### Action Filters

```csharp
// ✅ Reusable validation filter
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}

// Usage
[HttpPost]
[ValidateModel]
public async Task<ActionResult<User>> Create(CreateUserRequest request)
{
    // ModelState already validated
}
```

---


