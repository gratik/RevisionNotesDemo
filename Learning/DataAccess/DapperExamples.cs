// ==============================================================================
// DAPPER EXAMPLES - Micro-ORM Mastery
// ==============================================================================
// PURPOSE:
//   Master Dapper - the lightweight ORM for performance-focused data access.
//   "Micro-ORM" = SQL control + object mapping without EF overhead.
//
// WHY DAPPER:
//   - 10-100x faster than EF for reads
//   - Raw SQL control
//   - Minimal abstraction
//   - Used by Stack Overflow
//
// WHAT YOU'LL LEARN:
//   1. Basic queries - Query<T>, QueryFirst, Execute
//   2. Parameters and SQL injection prevention
//   3. Multi-mapping (joins)
//   4. Async operations
//   5. Multiple result sets
//   6. When to use Dapper vs EF
//
// KEY PRINCIPLE:
//   Dapper = ADO.NET + automatic mapping. You write SQL, Dapper maps results.
// ==============================================================================

using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RevisionNotesDemo.DataAccess;

/// <summary>
/// EXAMPLE 1: BASIC QUERYING - Query<T> for Lists
/// 
/// THE PROBLEM:
/// ADO.NET requires manual mapping from DataReader to objects.
/// EF Core generates complex SQL and has overhead.
/// 
/// THE SOLUTION:
/// Dapper.Query<T>() executes SQL and maps to objects automatically.
/// 
/// WHY IT MATTERS:
/// - You control SQL (optimize as needed)
/// - Fast mapping (compiled IL)
/// - Simple API
/// </summary>
public class BasicQueryingExamples
{
    private readonly string _connectionString = "Server=.;Database=MyDb;Trusted_Connection=true";
    
    // ✅ GOOD: Query list of objects
    public async Task<List<User>> GetAllUsers()
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = "SELECT Id, FirstName, LastName, Email, CreatedDate FROM Users";
        
        // ✅ Dapper extension method on IDbConnection
        var users = await connection.QueryAsync<User>(sql);
        
        return users.ToList();
    }
    
    // ✅ GOOD: Query single object
    public async Task<User?> GetUserById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = "SELECT Id, FirstName, LastName, Email, CreatedDate FROM Users WHERE Id = @Id";
        
        // ✅ Use QueryFirstOrDefaultAsync for single result
        var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        
        return user;
    }
    
    // ✅ GOOD: Execute commands (INSERT/UPDATE/DELETE)
    public async Task<int> CreateUser(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            INSERT INTO Users (FirstName, LastName, Email, CreatedDate)
            VALUES (@FirstName, @LastName, @Email, @CreatedDate);
            
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        
        // ✅ Returns the new user ID
        var id = await connection.QuerySingleAsync<int>(sql, user);
        
        return id;
    }
    
    // ✅ GOOD: Execute without return value
    public async Task<int> UpdateUser(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            UPDATE Users 
            SET FirstName = @FirstName, 
                LastName = @LastName, 
                Email = @Email 
            WHERE Id = @Id";
        
        // ✅ ExecuteAsync returns rows affected
        var rowsAffected = await connection.ExecuteAsync(sql, user);
        
        return rowsAffected;
    }
    
    // Supporting class
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}

/// <summary>
/// EXAMPLE 2: PARAMETERS - SQL Injection Prevention
/// 
/// THE PROBLEM:
/// String concatenation creates SQL injection vulnerabilities.
/// 
/// THE SOLUTION:
/// Dapper uses parameterized queries automatically.
/// 
/// WHY IT MATTERS:
/// - Security: Prevents SQL injection
/// - Performance: Query plan reuse
/// - Correctness: Handles escaping
/// </summary>
public class ParameterExamples
{
    private readonly string _connectionString = "ConnectionString";
    
    // ❌ BAD: String interpolation - SQL INJECTION!
    public async Task<List<Product>> SearchProducts_Unsafe(string searchTerm)
    {
        using var connection = new SqlConnection(_connectionString);
        
        // ❌ NEVER DO THIS!
        var sql = $"SELECT * FROM Products WHERE Name LIKE '%{searchTerm}%'";
        // If searchTerm = "'; DROP TABLE Products; --" → DISASTER
        
        return (await connection.QueryAsync<Product>(sql)).ToList();
    }
    
    // ✅ GOOD: Parameterized query
    public async Task<List<Product>> SearchProducts_Safe(string searchTerm)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = "SELECT * FROM Products WHERE Name LIKE @SearchTerm";
        
        // ✅ Dapper handles escaping, parameterization
        var products = await connection.QueryAsync<Product>(
            sql,
            new { SearchTerm = $"%{searchTerm}%" });  // ✅ Wrap with % here
        
        return products.ToList();
    }
    
    // ✅ GOOD: Anonymous object for parameters
    public async Task<Product?> GetProductByIdAndCategory(int id, string category)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT * FROM Products 
            WHERE Id = @Id AND Category = @Category";
        
        // ✅ Property names match parameter names
        return await connection.QueryFirstOrDefaultAsync<Product>(
            sql,
            new { Id = id, Category = category });
    }
    
    // ✅ GOOD: IN clause with array
    public async Task<List<Product>> GetProductsByIds(int[] ids)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = "SELECT * FROM Products WHERE Id IN @Ids";
        
        // ✅ Dapper expands array to (1, 2, 3, ...)
        var products = await connection.QueryAsync<Product>(
            sql,
            new { Ids = ids });
        
        return products.ToList();
    }
    
    // ✅ GOOD: DynamicParameters for complex scenarios
    public async Task<List<Product>> SearchProductsAdvanced(ProductSearchOptions options)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var parameters = new DynamicParameters();
        var sql = "SELECT * FROM Products WHERE 1=1";
        
        if (!string.IsNullOrEmpty(options.Name))
        {
            sql += " AND Name LIKE @Name";
            parameters.Add("Name", $"%{options.Name}%");
        }
        
        if (options.MinPrice.HasValue)
        {
            sql += " AND Price >= @MinPrice";
            parameters.Add("MinPrice", options.MinPrice.Value);
        }
        
        if (options.MaxPrice.HasValue)
        {
            sql += " AND Price <= @MaxPrice";
            parameters.Add("MaxPrice", options.MaxPrice.Value);
        }
        
        return (await connection.QueryAsync<Product>(sql, parameters)).ToList();
    }
    
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
    
    public class ProductSearchOptions
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

/// <summary>
/// EXAMPLE 3: MULTI-MAPPING - Joins with Relationships
/// 
/// THE PROBLEM:
/// Joins return flat rows, need to map to object graph.
/// 
/// THE SOLUTION:
/// Dapper's multi-mapping splits rows into related objects.
/// 
/// WHY IT MATTERS:
/// - Single query for related data (efficient)
/// - Automatic object graph construction
/// - Avoids N+1 queries
/// </summary>
public class MultiMappingExamples
{
    private readonly string _connectionString = "ConnectionString";
    
    // ✅ GOOD: One-to-one mapping (Order -> Customer)
    public async Task<List<Order>> GetOrdersWithCustomers()
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                o.Id, o.OrderDate, o.TotalAmount,
                c.Id, c.FirstName, c.LastName, c.Email
            FROM Orders o
            INNER JOIN Customers c ON o.CustomerId = c.Id";
        
        // ✅ Multi-mapping: tell Dapper how to split row
        var orders = await connection.QueryAsync<Order, Customer, Order>(
            sql,
            (order, customer) =>
            {
                order.Customer = customer;  // ✅ Assign relationship
                return order;
            },
            splitOn: "Id");  // ✅ Split at second "Id" column
        
        return orders.ToList();
    }
    
    // ✅ GOOD: One-to-many mapping (Order -> OrderItems)
    public async Task<List<Order>> GetOrdersWithItems()
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                o.Id, o.OrderDate, o.TotalAmount,
                oi.Id, oi.ProductName, oi.Quantity, oi.Price
            FROM Orders o
            LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
            ORDER BY o.Id";
        
        var orderDict = new Dictionary<int, Order>();
        
        await connection.QueryAsync<Order, OrderItem, Order>(
            sql,
            (order, item) =>
            {
                if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                {
                    existingOrder = order;
                    existingOrder.Items = new List<OrderItem>();
                    orderDict.Add(order.Id, existingOrder);
                }
                
                if (item != null)  // LEFT JOIN may have nulls
                    existingOrder.Items.Add(item);
                
                return existingOrder;
            },
            splitOn: "Id");
        
        return orderDict.Values.ToList();
    }
    
    // ✅ GOOD: Complex mapping (Order -> Customer, OrderItems)
    public async Task<List<Order>> GetOrdersComplete()
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                o.Id, o.OrderDate, o.TotalAmount,
                c.Id, c.FirstName, c.LastName, c.Email,
                oi.Id, oi.ProductName, oi.Quantity, oi.Price
            FROM Orders o
            INNER JOIN Customers c ON o.CustomerId = c.Id
            LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
            ORDER BY o.Id";
        
        var orderDict = new Dictionary<int, Order>();
        
        await connection.QueryAsync<Order, Customer, OrderItem, Order>(
            sql,
            (order, customer, item) =>
            {
                if (!orderDict.TryGetValue(order.Id, out var existingOrder))
                {
                    existingOrder = order;
                    existingOrder.Customer = customer;
                    existingOrder.Items = new List<OrderItem>();
                    orderDict.Add(order.Id, existingOrder);
                }
                
                if (item != null)
                    existingOrder.Items.Add(item);
                
                return existingOrder;
            },
            splitOn: "Id,Id");  // ✅ Split at customer Id, then item Id
        
        return orderDict.Values.ToList();
    }
    
    // Supporting classes
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
    }
    
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

/// <summary>
/// EXAMPLE 4: MULTIPLE RESULT SETS - Multiple Queries in One Round Trip
/// 
/// THE PROBLEM:
/// Multiple queries = multiple round trips to database.
/// 
/// THE SOLUTION:
/// QueryMultiple returns multiple result sets from one command.
/// </summary>
public class MultipleResultSetsExamples
{
    private readonly string _connectionString = "ConnectionString";
    
    // ✅ GOOD: Fetch multiple entities in one call
    public async Task<DashboardData> GetDashboard(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT * FROM Users WHERE Id = @UserId;
            SELECT * FROM Orders WHERE CustomerId = @UserId ORDER BY OrderDate DESC;
            SELECT * FROM Notifications WHERE UserId = @UserId AND IsRead = 0;";
        
        using var multi = await connection.QueryMultipleAsync(sql, new { UserId = userId });
        
        // ✅ Read each result set in order
        var user = await multi.ReadFirstOrDefaultAsync<User>();
        var orders = (await multi.ReadAsync<Order>()).ToList();
        var notifications = (await multi.ReadAsync<Notification>()).ToList();
        
        return new DashboardData
        {
            User = user,
            RecentOrders = orders,
            UnreadNotifications = notifications
        };
    }
    
    // ✅ GOOD: Stored procedure with multiple result sets
    public async Task<ReportData> ExecuteReport(int year)
    {
        using var connection = new SqlConnection(_connectionString);
        
        using var multi = await connection.QueryMultipleAsync(
            "sp_GetAnnualReport",
            new { Year = year },
            commandType: CommandType.StoredProcedure);
        
        var summary = await multi.ReadFirstAsync<ReportSummary>();
        var monthlyBreakdown = (await multi.ReadAsync<MonthlyData>()).ToList();
        var topProducts = (await multi.ReadAsync<ProductSales>()).ToList();
        
        return new ReportData
        {
            Summary = summary,
            MonthlyBreakdown = monthlyBreakdown,
            TopProducts = topProducts
        };
    }
    
    // Supporting classes
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    
    public class DashboardData
    {
        public User? User { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public List<Notification> UnreadNotifications { get; set; } = new();
    }
    
    public class ReportData
    {
        public ReportSummary? Summary { get; set; }
        public List<MonthlyData> MonthlyBreakdown { get; set; } = new();
        public List<ProductSales> TopProducts { get; set; } = new();
    }
    
    public class ReportSummary { public decimal TotalSales { get; set; } }
    public class MonthlyData { public int Month { get; set; } public decimal Sales { get; set; } }
    public class ProductSales { public string ProductName { get; set; } = ""; public decimal Sales { get; set; } }
}

// SUMMARY - Dapper vs EF Core:
//
// +-----------------------+------------------------+-------------------------+
// | Feature               | Dapper                 | EF Core                 |
// +-----------------------+------------------------+-------------------------+
// | Performance           | ✅ Fast (close to ADO) | ❌ Slower (overhead)    |
// | SQL Control           | ✅ Full control        | ❌ Generated SQL        |
// | Change Tracking       | ❌ Manual              | ✅ Automatic            |
// | Migrations            | ❌ Manual              | ✅ Code-first           |
// | Relationships         | ❌ Manual mapping      | ✅ Automatic            |
// | Learning Curve        | ✅ Easy (just SQL)     | ❌ Complex              |
// | Complex Queries       | ✅ Write any SQL       | ❌ LINQ limitations     |
// +-----------------------+------------------------+-------------------------+
//
// WHEN TO USE:
// ✅ Use Dapper when:
//   - Performance critical (reads)
//   - Complex queries
//   - Existing database (database-first)
//   - Team knows SQL well
//   - Microservices (lightweight)
//
// ✅ Use EF Core when:
//   - CRUD-heavy application
//   - Code-first approach
//   - Need change tracking
//   - Team prefers LINQ
//   - Rapid development
//
// ✅ Use BOTH:
//   - EF for writes (change tracking)
//   - Dapper for reads (performance)
//   - Common pattern in high-traffic apps
