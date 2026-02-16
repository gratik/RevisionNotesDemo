# Migrations

> Subject: [Entity-Framework](../README.md)

## Migrations

### Creating Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Migration Best Practices

```csharp
// ✅ GOOD: Descriptive migration names
dotnet ef migrations add AddUserEmailIndex
dotnet ef migrations add AddOrderStatusColumn

// ✅ Always review generated migration
public partial class AddUserEmailIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");
    }
}
```

---


