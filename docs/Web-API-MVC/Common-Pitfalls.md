# Common Pitfalls

> Subject: [Web-API-MVC](../README.md)

## Common Pitfalls

### ❌ Not Using Async/Await

```csharp
// ❌ BAD: Blocking call
public ActionResult<User> Get(int id)
{
    var user = _repository.GetByIdAsync(id).Result;  // ❌ Blocks thread
    return Ok(user);
}

// ✅ GOOD: Async all the way
public async Task<ActionResult<User>> Get(int id)
{
    var user = await _repository.GetByIdAsync(id);
    return Ok(user);
}
```

### ❌ Exposing Domain Models

```csharp
// ❌ BAD: Exposes internal structure
[HttpGet]
public ActionResult<Customer> Get()
{
    return Ok(_dbContext.Customers.First());  // ❌ Domain model
}

// ✅ GOOD: Use DTOs
[HttpGet]
public ActionResult<CustomerDto> Get()
{
    var customer = _dbContext.Customers.First();
    var dto = _mapper.Map<CustomerDto>(customer);
    return Ok(dto);
}
```

### ❌ Wrong Status Codes

```csharp
// ❌ BAD: Always returns 200
[HttpPost]
public ActionResult Create(User user)
{
    _repository.Add(user);
    return Ok(user);  // ❌ Should be 201 Created
}

// ✅ GOOD: Correct status code
[HttpPost]
public ActionResult<User> Create(User user)
{
    _repository.Add(user);
    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
}
```

---


