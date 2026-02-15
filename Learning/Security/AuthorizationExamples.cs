// ==============================================================================
// AUTHORIZATION EXAMPLES - ASP.NET Core Access Control
// ==============================================================================
// WHAT IS THIS?
// -------------
// Access control patterns (roles, claims, policies, resource checks).
//
// WHY IT MATTERS
// --------------
// ✅ Enforces least privilege for sensitive actions
// ✅ Protects data in multi-tenant scenarios
//
// WHEN TO USE
// -----------
// ✅ Any app with protected actions or data
// ✅ Multi-tenant or role-based systems
//
// WHEN NOT TO USE
// ---------------
// ❌ Public endpoints that require no access control
// ❌ Static content with no user context
//
// REAL-WORLD EXAMPLE
// ------------------
// Policy-based access to approve invoices.
// ==============================================================================

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RevisionNotesDemo.Security;

/// <summary>
/// EXAMPLE 1: ROLE-BASED AUTHORIZATION (RBAC)
/// 
/// THE PROBLEM:
/// Need simple access control based on user roles (Admin, User, Manager).
/// Different roles have different permissions.
/// 
/// THE SOLUTION:
/// Assign users to roles, check role membership before allowing actions.
/// Use [Authorize(Roles = "Admin")] attribute on controllers/actions.
/// 
/// WHY IT MATTERS:
/// - Simple and intuitive
/// - Easy to implement
/// - Widely understood
/// - Works for 80% of scenarios
/// 
/// BEST FOR: Simple permission models, small to medium apps
/// NOT IDEAL FOR: Complex permission matrices, fine-grained access control
/// 
/// GOTCHA: Roles are coarse-grained - user can access ALL Admin features
/// </summary>
public static class RoleBasedAuthorizationExamples
{
    // ✅ GOOD: Attribute-based role authorization
    [Authorize(Roles = "Admin")]
    public static IResult AdminOnlyEndpoint()
    {
        return Results.Ok("Admin access granted");
    }

    // ✅ GOOD: Multiple roles (OR logic)
    [Authorize(Roles = "Admin,Manager")]
    public static IResult AdminOrManagerEndpoint()
    {
        return Results.Ok("Admin or Manager access granted");
    }

    // ✅ GOOD: Manual role check in code
    public static IResult ManualRoleCheck(HttpContext context)
    {
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return Results.Unauthorized();
        }

        if (user.IsInRole("Admin"))
        {
            return Results.Ok("Admin access");
        }

        if (user.IsInRole("Manager"))
        {
            return Results.Ok("Manager access");
        }

        return Results.Forbid();
    }

    // ❌ BAD: Hardcoded role checks throughout codebase
    public static IResult BadRoleCheck(HttpContext context)
    {
        if (context.User.FindFirst(ClaimTypes.Role)?.Value == "Admin")
        {
            // Scattered role checks = maintenance nightmare
            // What if role name changes? Have to update everywhere!
        }
        return Results.Ok();
    }
}

/// <summary>
/// EXAMPLE 2: CLAIMS-BASED AUTHORIZATION
/// 
/// THE PROBLEM:
/// Roles are too coarse. Need fine-grained permissions.
/// User might have "CanApproveInvoices" but not "CanDeleteInvoices".
/// 
/// THE SOLUTION:
/// Use claims - statements about the user (permissions, attributes, metadata).
/// Check specific claims before allowing actions.
/// 
/// WHY IT MATTERS:
/// - Fine-grained access control
/// - Flexible permission model
/// - Claims can come from multiple sources
/// - Industry standard (SAML, OAuth, OpenID Connect)
/// 
/// CLAIM EXAMPLES:
/// - Permission: CanApproveInvoices, CanDeleteUsers
/// - Attribute: Department=Sales, Level=Senior
/// - Metadata: SubscriptionTier=Premium, Region=US-West
/// 
/// BEST FOR: Complex permission models, enterprise apps, external identity
/// </summary>
public static class ClaimsBasedAuthorizationExamples
{
    public static void ConfigureClaimsAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            // ✅ GOOD: Claim-based policy
            options.AddPolicy("CanApproveInvoices", policy =>
                policy.RequireClaim("Permission", "ApproveInvoices"));

            options.AddPolicy("CanDeleteUsers", policy =>
                policy.RequireClaim("Permission", "DeleteUsers"));

            // ✅ GOOD: Multiple claims required (AND logic)
            options.AddPolicy("SeniorSalesManager", policy =>
            {
                policy.RequireClaim("Department", "Sales");
                policy.RequireClaim("Level", "Senior", "Principal");
                policy.RequireRole("Manager");
            });

            // ✅ GOOD: Custom validation logic
            options.AddPolicy("PremiumSubscriber", policy =>
                policy.RequireAssertion(context =>
                {
                    var tier = context.User.FindFirst("SubscriptionTier")?.Value;
                    return tier == "Premium" || tier == "Enterprise";
                }));
        });
    }

    // ✅ GOOD: Use policy on endpoint
    [Authorize(Policy = "CanApproveInvoices")]
    public static IResult ApproveInvoice(int invoiceId)
    {
        return Results.Ok($"Invoice {invoiceId} approved");
    }

    // ✅ GOOD: Manual claim check
    public static IResult ManualClaimCheck(HttpContext context)
    {
        var user = context.User;
        var hasPermission = user.HasClaim("Permission", "ApproveInvoices");

        if (!hasPermission)
        {
            return Results.Forbid();
        }

        return Results.Ok("Permission granted");
    }
}

/// <summary>
/// EXAMPLE 3: POLICY-BASED AUTHORIZATION
/// 
/// THE PROBLEM:
/// Complex authorization logic scattered across controllers.
/// Can't reuse authorization rules.
/// Hard to test authorization logic.
/// 
/// THE SOLUTION:
/// Define authorization policies with custom requirements and handlers.
/// Centralized, testable, reusable authorization logic.
/// 
/// WHY IT MATTERS:
/// - Separation of concerns (authorization logic separate from controllers)
/// - Reusable across multiple endpoints
/// - Testable (unit test handlers)
/// - Maintainable (change in one place)
/// - Flexible (combine multiple requirements)
/// 
/// BEST PRACTICE: Use policies for all non-trivial authorization
/// </summary>

// Custom requirement
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
}

// Custom handler
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == "DateOfBirth");

        if (dateOfBirthClaim == null)
        {
            return Task.CompletedTask; // Requirement not met
        }

        if (DateTime.TryParse(dateOfBirthClaim.Value, out var dateOfBirth))
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age >= requirement.MinimumAge)
            {
                context.Succeed(requirement); // Requirement met
            }
        }

        return Task.CompletedTask;
    }
}

public static class PolicyBasedAuthorizationSetup
{
    public static void ConfigurePolicyAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

        builder.Services.AddAuthorization(options =>
        {
            // ✅ GOOD: Policy with custom requirement
            options.AddPolicy("Over18", policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(18)));

            options.AddPolicy("Over21", policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(21)));
        });
    }

    [Authorize(Policy = "Over18")]
    public static IResult AdultContentEndpoint()
    {
        return Results.Ok("Adult content access granted");
    }
}

/// <summary>
/// EXAMPLE 4: RESOURCE-BASED AUTHORIZATION
/// 
/// THE PROBLEM:
/// Need to authorize based on the SPECIFIC resource being accessed.
/// User can edit THEIR OWN posts but not others' posts.
/// Manager can approve invoices in THEIR department only.
/// 
/// THE SOLUTION:
/// Pass the resource to authorization service at runtime.
/// Authorization handler checks user's relationship to resource.
/// 
/// WHY IT MATTERS:
/// - Fine-grained, instance-level authorization
/// - Protects individual resources
/// - Implements "ownership" patterns
/// - Required for multi-tenant apps
/// 
/// EXAMPLE SCENARIOS:
/// - Edit own profile (not others')
/// - Delete own comments (not others')
/// - Approve documents in own department
/// - Access data for own organization (multi-tenant)
/// 
/// GOTCHA: Can't use [Authorize] attribute - must call imperatively!
/// </summary>

public record Document(int Id, string Title, string OwnerId, string Department);

// Custom requirement for document operations
public class DocumentAuthorizationRequirement : IAuthorizationRequirement
{
    public string Operation { get; }
    public DocumentAuthorizationRequirement(string operation) => Operation = operation;
}

// Handler that checks user's relationship to document
public class DocumentAuthorizationHandler :
    AuthorizationHandler<DocumentAuthorizationRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DocumentAuthorizationRequirement requirement,
        Document resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userDepartment = context.User.FindFirst("Department")?.Value;

        switch (requirement.Operation)
        {
            case "Read":
                // Anyone in same department can read
                if (resource.Department == userDepartment)
                {
                    context.Succeed(requirement);
                }
                break;

            case "Edit":
                // Only owner or admin can edit
                if (resource.OwnerId == userId || context.User.IsInRole("Admin"))
                {
                    context.Succeed(requirement);
                }
                break;

            case "Delete":
                // Only owner can delete (not even admin)
                if (resource.OwnerId == userId)
                {
                    context.Succeed(requirement);
                }
                break;
        }

        return Task.CompletedTask;
    }
}

public class DocumentService
{
    private readonly IAuthorizationService _authorizationService;

    public DocumentService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<IResult> EditDocument(HttpContext context, int documentId, Document updatedDoc)
    {
        // Load document from database
        var document = GetDocument(documentId);  // Simulated

        // ✅ GOOD: Check authorization for THIS SPECIFIC document
        var authResult = await _authorizationService.AuthorizeAsync(
            context.User,
            document,
            new DocumentAuthorizationRequirement("Edit"));

        if (!authResult.Succeeded)
        {
            return Results.Forbid();
        }

        // User is authorized - proceed with edit
        return Results.Ok("Document updated");
    }

    private Document GetDocument(int id) =>
        new Document(id, "Sample", "user123", "Sales");
}

/// <summary>
/// EXAMPLE 5: COMBINING REQUIREMENTS - AND/OR Logic
/// 
/// THE PROBLEM:
/// Need complex authorization rules with multiple conditions.
/// Must satisfy ALL requirements (AND) or ANY requirement (OR).
/// 
/// THE SOLUTION:
/// Combine requirements using RequireAssertion or custom handlers.
/// 
/// WHY IT MATTERS:
/// - Express complex business rules
/// - Flexible authorization logic
/// - Clear, declarative policies
/// </summary>
public static class CombinedRequirementsExamples
{
    public static void ConfigureCombinedAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            // ✅ GOOD: Multiple requirements - ALL must pass (AND logic)
            options.AddPolicy("SeniorAdminOnly", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("Level", "Senior", "Principal");
                policy.RequireAssertion(context =>
                    int.Parse(context.User.FindFirst("YearsOfService")?.Value ?? "0") >= 5);
            });

            // ✅ GOOD: Any of multiple roles (OR logic)
            options.AddPolicy("Elevated", policy =>
            {
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("SuperUser") ||
                    context.User.HasClaim("Permission", "ElevatedAccess"));
            });

            // ✅ GOOD: Complex business rule
            options.AddPolicy("CanApproveLargeInvoices", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    // Must be manager in Finance department OR be admin
                    var isFinanceManager =
                        context.User.IsInRole("Manager") &&
                        context.User.HasClaim("Department", "Finance");

                    var isAdmin = context.User.IsInRole("Admin");

                    return isFinanceManager || isAdmin;
                });
            });
        });
    }
}

/// <summary>
/// EXAMPLE 6: AUTHORIZATION BEST PRACTICES
/// 
/// Summary of authorization best practices and patterns
/// </summary>
public static class AuthorizationBestPractices
{
    // ✅ DO: Use policies for reusable authorization logic
    // ✅ DO: Keep authorization handlers simple and focused
    // ✅ DO: Use claims for fine-grained permissions
    // ✅ DO: Implement resource-based auth for instance-level security
    // ✅ DO: Test authorization handlers in unit tests
    // ✅ DO: Use meaningful policy names
    // ✅ DO: Document authorization requirements

    // ❌ DON'T: Put authorization logic in controllers
    // ❌ DON'T: Use magic strings for roles/claims
    // ❌ DON'T: Trust client-side authorization (always verify server-side)
    // ❌ DON'T: Mix authentication and authorization concerns
    // ❌ DON'T: Forget to test authorization edge cases
    // ❌ DON'T: Use roles for everything (claims are more flexible)

    public const string AdminRole = "Admin";
    public const string ManagerRole = "Manager";
    public const string UserRole = "User";

    // Policy name constants (avoid magic strings)
    public const string Over18Policy = "Over18";
    public const string CanApproveInvoicesPolicy = "CanApproveInvoices";
    public const string SeniorAdminPolicy = "SeniorAdminOnly";
}
