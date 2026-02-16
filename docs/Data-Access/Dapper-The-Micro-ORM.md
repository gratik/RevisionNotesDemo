# Dapper: The Micro-ORM

> Subject: [Data-Access](../README.md)

## Dapper: The Micro-ORM

**The Sweet Spot:** SQL control + automatic object mapping without EF overhead.

### Basic Queries

```csharp
using Dapper;

// Query list of objects
var users = await connection.QueryAsync<User>(
    "SELECT Id, FirstName, LastName, Email FROM Users");

// Query single object
var user = await connection.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Id = @Id",
    new { Id = userId });

// Execute command (INSERT/UPDATE/DELETE)
var rowsAffected = await connection.ExecuteAsync(
    "UPDATE Users SET LastLogin = @Now WHERE Id = @Id",
    new { Now = DateTime.UtcNow, Id = userId });

// Return scalar value (new ID)
var newId = await connection.QuerySingleAsync<int>(
    @"INSERT INTO Users (Name, Email) VALUES (@Name, @Email);
      SELECT CAST(SCOPE_IDENTITY() AS INT);",
    new { Name = "John", Email = "john@example.com" });
```

### Multi-Mapping (JOIN Queries)

**One of Dapper's most powerful features:**

```csharp
// Map to two objects from a JOIN
var orders = await connection.QueryAsync<Order, Customer, Order>(
    sql: @"SELECT o.*, c.*
           FROM Orders o
           INNER JOIN Customers c ON o.CustomerId = c.Id",
    map: (order, customer) =>
    {
        order.Customer = customer;
        return order;
    },
    splitOn: "Id"  // Where second object starts (Customer.Id)
);

// Map to three objects
var orders = await connection.QueryAsync<Order, Customer, Product, Order>(
    sql: @"SELECT o.*, c.*, p.*
           FROM Orders o
           INNER JOIN Customers c ON o.CustomerId = c.Id
           INNER JOIN Products p ON o.ProductId = p.Id",
    map: (order, customer, product) =>
    {
        order.Customer = customer;
        order.Product = product;
        return order;
    },
    splitOn: "Id,Id"  // Split points for Customer and Product
);
```

### Multiple Result Sets (QueryMultiple)

**Execute multiple queries in one round-trip:**

```csharp
using var multi = await connection.QueryMultipleAsync(@"
    SELECT * FROM Customers WHERE Id = @CustomerId;
    SELECT * FROM Orders WHERE CustomerId = @CustomerId;
    SELECT * FROM Addresses WHERE CustomerId = @CustomerId;",
    new { CustomerId = customerId }
);

var customer = await multi.ReadFirstAsync<Customer>();
var orders = await multi.ReadAsync<Order>();
var addresses = await multi.ReadAsync<Address>();
```

### Dynamic Parameters

```csharp
var parameters = new DynamicParameters();
parameters.Add("@Name", "John");
parameters.Add("@OutputId", dbType: DbType.Int32, direction: ParameterDirection.Output);

await connection.ExecuteAsync(
    "INSERT INTO Users (Name) VALUES (@Name); SELECT @OutputId = SCOPE_IDENTITY();",
    parameters);

var newId = parameters.Get<int>("@OutputId");
```

---


