// ==============================================================================
// ADO.NET PATTERNS - Low-Level Data Access Mastery
// ==============================================================================
//
// WHAT IS ADO.NET?
// ----------------
// The low-level data access API for .NET. It provides direct control over
// connections, commands, transactions, and data readers.
//
// WHY IT MATTERS
// --------------
// - Maximum performance and control
// - Foundation for Dapper and EF Core
// - Essential for understanding database behavior
// - Useful for bulk operations and fine-tuned SQL
//
// WHEN TO USE
// -----------
// - YES: Performance-critical or specialized SQL
// - YES: Bulk imports or stored procedure-heavy systems
// - YES: When you need full control over connections and transactions
//
// WHEN NOT TO USE
// ---------------
// - NO: For rapid CRUD where EF Core fits
// - NO: If you want automatic change tracking and graph updates
//
// REAL-WORLD EXAMPLE
// ------------------
// ETL pipeline:
// - Stream rows via SqlDataReader
// - Batch insert into target tables
// - Use explicit transactions for consistency
// ==============================================================================

using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace RevisionNotesDemo.DataAccess;

/// <summary>
/// EXAMPLE 1: CONNECTION MANAGEMENT - Pooling and Disposal
/// 
/// THE PROBLEM:
/// Database connections are expensive to create/destroy.
/// Not closing connections exhausts connection pool.
/// 
/// THE SOLUTION:
/// Use `using` statements for automatic disposal.
/// Connection pooling reuses connections automatically.
/// 
/// WHY IT MATTERS:
/// - Connection pool exhaustion = app hangs
/// - Proper disposal = fast, reliable
/// - Pooling = connections reused transparently
/// </summary>
public class ConnectionManagementExamples
{
    private readonly string _connectionString = "Server=.;Database=MyDb;Trusted_Connection=true";

    // ❌ BAD: Not disposing connection
    public async Task<List<string>> GetNames_Bad()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();  // ❌ Connection never closed!

        var command = new SqlCommand("SELECT Name FROM Users", connection);
        var reader = await command.ExecuteReaderAsync();

        var names = new List<string>();
        while (await reader.ReadAsync())
        {
            names.Add(reader.GetString(0));
        }

        return names;  // ❌ Connection leaked
    }

    // ✅ GOOD: Using statement ensures disposal
    public async Task<List<string>> GetNames_Good()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Name FROM Users", connection);
        using var reader = await command.ExecuteReaderAsync();

        var names = new List<string>();
        while (await reader.ReadAsync())
        {
            names.Add(reader.GetString(0));
        }

        return names;  // ✅ Connection returned to pool
    }

    // ✅ GOOD: Nested using pattern
    public async Task<int> ExecuteCommand_Nested()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Users SET LastLogin = @Now WHERE Id = @Id";
                command.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Id", 1);

                return await command.ExecuteNonQueryAsync();
            }
        }  // ✅ Both disposed
    }

    // CONNECTION POOLING:
    // - ADO.NET pools connections automatically
    // - Connection string = pool key
    // - "using" returns connection to pool (not destroyed)
    // - Default: Min=0, Max=100 connections

    // ✅ Custom pool settings in connection string:
    private readonly string _connectionStringCustomPool =
        "Server=.;Database=MyDb;Trusted_Connection=true;" +
        "Min Pool Size=5;Max Pool Size=200;Pooling=true";

    // GOTCHA: Connection string differences = separate pools
    public void PoolingGotcha()
    {
        var conn1 = "Server=.;Database=MyDb";
        var conn2 = "Server=.;Database=MyDb;";  // ❌ Trailing semicolon = different pool!

        // These use SEPARATE pools (wasteful)
    }
}

/// <summary>
/// EXAMPLE 2: SQLCOMMAND PATTERNS - Executing Queries
/// 
/// THE PROBLEM:
/// Different operations need different execution methods.
/// 
/// THE SOLUTION:
/// - ExecuteReader: SELECT (returns rows)
/// - ExecuteNonQuery: INSERT/UPDATE/DELETE (returns row count)
/// - ExecuteScalar: Single value (COUNT, SUM, new ID)
/// </summary>
public class SqlCommandPatternsExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ✅ ExecuteReader: For SELECT queries
    public async Task<List<User>> GetUsers_ExecuteReader()
    {
        var users = new List<User>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Id, Name, Email FROM Users", connection);
        using var reader = await command.ExecuteReaderAsync();

        // ✅ Read each row
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),         // By index
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            });
        }

        return users;
    }

    // ✅ ExecuteNonQuery: For INSERT/UPDATE/DELETE
    public async Task<int> UpdateUser_ExecuteNonQuery(int id, string name)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(
            "UPDATE Users SET Name = @Name WHERE Id = @Id",
            connection);

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Id", id);

        // ✅ Returns number of rows affected
        return await command.ExecuteNonQueryAsync();
    }

    // ✅ ExecuteScalar: For single value
    public async Task<int> GetUserCount_ExecuteScalar()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT COUNT(*) FROM Users", connection);

        // ✅ Returns first column of first row
        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result, CultureInfo.InvariantCulture) : 0;
    }

    // ✅ ExecuteScalar: Get new ID after INSERT
    public async Task<int> CreateUser_ReturnId(string name, string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
            INSERT INTO Users (Name, Email) VALUES (@Name, @Email);
            SELECT CAST(SCOPE_IDENTITY() AS INT);",  // ✅ Return new ID
            connection);

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Email", email);

        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result, CultureInfo.InvariantCulture) : 0;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

/// <summary>
/// EXAMPLE 3: SQLDATAREADER - Efficient Reading Patterns
/// 
/// THE PROBLEM:
/// DataReader is forward-only, need to read efficiently.
/// 
/// THE SOLUTION:
/// Read by ordinal (faster), check for nulls, use async.
/// </summary>
public class SqlDataReaderExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ❌ BAD: Reading by column name (slow)
    public async Task<List<Product>> GetProducts_ByName()
    {
        var products = new List<Product>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Id, Name, Price, Description FROM Products", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                // ❌ Slower: name lookup every row
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            });
        }

        return products;
    }

    // ✅ GOOD: Reading by ordinal (fast)
    public async Task<List<Product>> GetProducts_ByOrdinal()
    {
        var products = new List<Product>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Id, Name, Price, Description FROM Products", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),      // ✅ Direct ordinal access
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3)  // ✅ Null handling
            });
        }

        return products;
    }

    // ✅ BEST: Get ordinals once, reuse
    public async Task<List<Product>> GetProducts_Optimized()
    {
        var products = new List<Product>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand("SELECT Id, Name, Price, Description FROM Products", connection);
        using var reader = await command.ExecuteReaderAsync();

        // ✅ Get ordinals once before loop
        int idOrdinal = reader.GetOrdinal("Id");
        int nameOrdinal = reader.GetOrdinal("Name");
        int priceOrdinal = reader.GetOrdinal("Price");
        int descOrdinal = reader.GetOrdinal("Description");

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(idOrdinal),
                Name = reader.GetString(nameOrdinal),
                Price = reader.GetDecimal(priceOrdinal),
                Description = reader.IsDBNull(descOrdinal) ? null : reader.GetString(descOrdinal)
            });
        }

        return products;
    }

    // ✅ GOOD: Helper method for safe reading
    public static T? GetValueOrNull<T>(SqlDataReader reader, int ordinal) where T : struct
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetFieldValue<T>(ordinal);
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}

/// <summary>
/// EXAMPLE 4: PARAMETERIZED QUERIES - SQL Injection Prevention
/// 
/// THE PROBLEM:
/// String concatenation creates SQL injection risks.
/// 
/// THE SOLUTION:
/// Always use SqlParameter for user input.
/// </summary>
public class ParameterizedQueriesExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ❌ DANGER: SQL Injection vulnerability!
    public async Task<List<User>> SearchUsers_Unsafe(string searchTerm)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // ❌ NEVER DO THIS!
        var sql = $"SELECT * FROM Users WHERE Name LIKE '%{searchTerm}%'";
        // If searchTerm = "'; DROP TABLE Users; --" → Database destroyed!

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var users = new List<User>();
        // ... read results
        return users;
    }

    // ✅ GOOD: Parameterized query (safe)
    public async Task<List<User>> SearchUsers_Safe(string searchTerm)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT * FROM Users WHERE Name LIKE @SearchTerm";
        using var command = new SqlCommand(sql, connection);

        // ✅ AddWithValue (simple)
        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

        using var reader = await command.ExecuteReaderAsync();

        var users = new List<User>();
        // ... read results
        return users;
    }

    // ✅ BETTER: Explicit parameter types (performance)
    public async Task<User?> GetUser_TypedParameters(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT Id, Name, Email FROM Users WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);

        // ✅ Explicit type avoids implicit conversion
        var param = command.Parameters.Add("@Id", SqlDbType.Int);
        param.Value = id;

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2)
            };
        }

        return null;
    }

    // ✅ GOOD: Multiple parameters
    public async Task<int> UpdateUser_MultipleParams(int id, string name, string email, DateTime? lastLogin)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE Users 
            SET Name = @Name, 
                Email = @Email, 
                LastLogin = @LastLogin 
            WHERE Id = @Id";

        using var command = new SqlCommand(sql, connection);

        // ✅ Add all parameters
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
        command.Parameters.Add("@LastLogin", SqlDbType.DateTime).Value =
            lastLogin.HasValue ? lastLogin.Value : DBNull.Value;  // ✅ Handle null

        return await command.ExecuteNonQueryAsync();
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

/// <summary>
/// EXAMPLE 5: ASYNC ADO.NET - Non-Blocking Database Operations
/// 
/// THE PROBLEM:
/// Sync database calls block threads (thread pool exhaustion).
/// 
/// THE SOLUTION:
/// Use async methods: OpenAsync, ExecuteReaderAsync, ReadAsync.
/// </summary>
public class AsyncAdoNetExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ❌ BAD: Synchronous methods (blocks thread)
    public List<Order> GetOrders_Sync(int customerId)
    {
        var orders = new List<Order>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();  // ❌ Blocks thread

        using var command = new SqlCommand("SELECT * FROM Orders WHERE CustomerId = @Id", connection);
        command.Parameters.AddWithValue("@Id", customerId);

        using var reader = command.ExecuteReader();  // ❌ Blocks thread

        while (reader.Read())  // ❌ Blocks thread
        {
            orders.Add(new Order { Id = reader.GetInt32(0) });
        }

        return orders;
    }

    // ✅ GOOD: Async methods (non-blocking)
    public async Task<List<Order>> GetOrders_Async(int customerId)
    {
        var orders = new List<Order>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();  // ✅ Non-blocking

        using var command = new SqlCommand("SELECT * FROM Orders WHERE CustomerId = @Id", connection);
        command.Parameters.AddWithValue("@Id", customerId);

        using var reader = await command.ExecuteReaderAsync();  // ✅ Non-blocking

        while (await reader.ReadAsync())  // ✅ Non-blocking
        {
            orders.Add(new Order { Id = reader.GetInt32(0) });
        }

        return orders;
    }

    // ✅ GOOD: CancellationToken support
    public async Task<List<Order>> GetOrders_WithCancellation(
        int customerId,
        CancellationToken cancellationToken)
    {
        var orders = new List<Order>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);  // ✅ Cancellable

        using var command = new SqlCommand("SELECT * FROM Orders WHERE CustomerId = @Id", connection);
        command.Parameters.AddWithValue("@Id", customerId);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);  // ✅ Cancellable

        while (await reader.ReadAsync(cancellationToken))  // ✅ Cancellable
        {
            orders.Add(new Order { Id = reader.GetInt32(0) });
        }

        return orders;
    }

    public class Order { public int Id { get; set; } }
}

// SUMMARY - ADO.NET Best Practices:
//
// ✅ ALWAYS:
// - Use `using` statements for disposal
// - Use async methods (OpenAsync, ExecuteReaderAsync, ReadAsync)
// - Use parameterized queries (never string concatenation)
// - Read by ordinal (faster than by name)
// - Check IsDBNull before reading nullable columns
// - Use explicit parameter types for better performance
//
// ❌ NEVER:
// - Concatenate user input into SQL
// - Forget to dispose connections/commands/readers
// - Use synchronous methods in ASP.NET Core
// - Leave connections open longer than needed
// - Create custom connection pooling (ADO.NET handles it)
//
// CONNECTION POOLING SUMMARY:
// - Enabled by default in connection string
// - Pool identified by exact connection string match
// - using() returns connection to pool (not destroyed)
// - Default: min=0, max=100 connections
// - Monitor: Performance Counter "NumberOfPooledConnections"
//
// WHEN TO USE:
// ✅ Use ADO.NET directly when:
//   - Maximum performance needed
//   - Complex queries/stored procedures
//   - Bulk operations
//   - Learning foundation
//
// ✅ Use Dapper when:
//   - Want object mapping but keep SQL control
//   - Moderate performance needs
//
// ✅ Use EF Core when:
//   - Productivity over performance
//   - Change tracking needed
//   - Code-first approach
