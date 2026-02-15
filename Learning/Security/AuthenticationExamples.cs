// ==============================================================================
// AUTHENTICATION EXAMPLES - ASP.NET Core Security
// ==============================================================================
// WHAT IS THIS?
// -------------
// Identity verification patterns (JWT, cookies, OAuth, refresh tokens).
//
// WHY IT MATTERS
// --------------
// ✅ Protects resources and user data
// ✅ Enables personalization and auditing
//
// WHEN TO USE
// -----------
// ✅ APIs and apps that require user identity
// ✅ Multi-client apps (web, mobile, SPA)
//
// WHEN NOT TO USE
// ---------------
// ❌ Public endpoints with no identity needs
// ❌ Internal tools with separate network-based controls only
//
// REAL-WORLD EXAMPLE
// ------------------
// SPA uses JWT access and refresh tokens for API access.
// ==============================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
// Note: OAuth providers require additional packages:
// - Microsoft.AspNetCore.Authentication.Google
// - Microsoft.AspNetCore.Authentication.MicrosoftAccount

namespace RevisionNotesDemo.Security;

/// <summary>
/// EXAMPLE 1: JWT TOKEN AUTHENTICATION
/// 
/// THE PROBLEM:
/// Need stateless authentication for APIs and SPAs where sessions don't work.
/// Cookies don't work well for mobile apps or cross-domain scenarios.
/// 
/// THE SOLUTION:
/// Use JWT tokens - self-contained, signed tokens that carry user claims.
/// Server generates token, client includes it in Authorization header.
/// Server validates signature without database lookup (stateless).
/// 
/// WHY IT MATTERS:
/// - Stateless (no server-side session storage)
/// - Scalable (no session affinity required)
/// - Cross-domain compatible
/// - Works with mobile apps
/// - Industry standard (RFC 7519)
/// 
/// JWT STRUCTURE:
/// Header.Payload.Signature
/// - Header: Algorithm and token type
/// - Payload: Claims (user data, expiration, issuer)
/// - Signature: Ensures token hasn't been tampered with
/// 
/// GOTCHA: Store JWT secret securely! Never commit to source control!
/// SECURITY: Use HTTPS only! JWT in HTTP = credentials exposed!
/// </summary>
public class JwtAuthenticationService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtAuthenticationService(string secretKey, string issuer, string audience)
    {
        // ❌ BAD: Hardcoded secret in code
        // _secretKey = "my-super-secret-key";

        // ✅ GOOD: Secret from configuration/environment variable
        _secretKey = secretKey; // From IConfiguration or Key Vault
        _issuer = issuer;
        _audience = audience;
    }

    /// <summary>
    /// Generate JWT token for authenticated user
    /// </summary>
    public string GenerateToken(string userId, string username, string email, string[] roles)
    {
        // ✅ GOOD: Build claims (user information)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64), // Issued at
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // ✅ GOOD: Create signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // ✅ GOOD: Create token with appropriate expiration
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            // ✅ GOOD: Validate token parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true, // Check expiration
                ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 min clock skew
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // ✅ GOOD: Verify algorithm to prevent "none" algorithm attack
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                return principal;
            }

            return null;
        }
        catch (SecurityTokenException)
        {
            // Token invalid, expired, or tampered
            return null;
        }
    }
}

/// <summary>
/// EXAMPLE 2: REFRESH TOKEN PATTERN
/// 
/// THE PROBLEM:
/// Short-lived access tokens expire quickly (security best practice).
/// Forcing users to re-login frequently is poor UX.
/// Storing long-lived tokens is a security risk.
/// 
/// THE SOLUTION:
/// Use refresh tokens - long-lived tokens stored securely that can get new access tokens.
/// Access token: Short-lived (15 min - 1 hour), sent with each API request
/// Refresh token: Long-lived (days/weeks), used only to get new access tokens
/// 
/// WHY IT MATTERS:
/// - Security: Limit damage if access token is stolen (expires quickly)
/// - UX: Users stay logged in without re-entering credentials
/// - Control: Can revoke refresh tokens from database
/// - Compliance: Meets security standards (OWASP, PCI-DSS)
/// 
/// BEST PRACTICE: Store refresh tokens hashed in database, not plain text!
/// </summary>
public class RefreshTokenService
{
    private readonly Dictionary<string, RefreshToken> _refreshTokens = new(); // In production: Use database

    public record RefreshToken(
        string Token,
        string UserId,
        DateTime ExpiresAt,
        DateTime CreatedAt,
        string? ReplacedByToken = null, // For token rotation
        bool IsRevoked = false
    );

    /// <summary>
    /// Generate secure refresh token
    /// </summary>
    public RefreshToken GenerateRefreshToken(string userId)
    {
        // ❌ BAD: Sequential or predictable tokens
        // var token = $"refresh_{userId}_{DateTime.Now.Ticks}";

        // ✅ GOOD: Cryptographically secure random token
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var token = Convert.ToBase64String(randomBytes);

        var refreshToken = new RefreshToken(
            Token: token,
            UserId: userId,
            ExpiresAt: DateTime.UtcNow.AddDays(30), // 30 days
            CreatedAt: DateTime.UtcNow
        );

        // ✅ GOOD: Store hashed token (in production)
        // var hashedToken = HashToken(token);
        // _database.RefreshTokens.Add(new { Hash = hashedToken, UserId, ExpiresAt });

        _refreshTokens[token] = refreshToken;
        return refreshToken;
    }

    /// <summary>
    /// Validate refresh token and rotate it (security best practice)
    /// </summary>
    public (bool IsValid, RefreshToken? NewToken) ValidateAndRotateRefreshToken(string token)
    {
        if (!_refreshTokens.TryGetValue(token, out var refreshToken))
        {
            return (false, null);
        }

        // ✅ GOOD: Check if token is valid
        if (refreshToken.IsRevoked ||
            refreshToken.ExpiresAt < DateTime.UtcNow ||
            refreshToken.ReplacedByToken != null)
        {
            return (false, null);
        }

        // ✅ GOOD: Rotate token (generate new one, invalidate old)
        var newRefreshToken = GenerateRefreshToken(refreshToken.UserId);

        // Mark old token as replaced
        _refreshTokens[token] = refreshToken with { ReplacedByToken = newRefreshToken.Token };

        return (true, newRefreshToken);
    }

    /// <summary>
    /// Revoke refresh token (logout, security breach, etc.)
    /// </summary>
    public void RevokeToken(string token)
    {
        if (_refreshTokens.TryGetValue(token, out var refreshToken))
        {
            _refreshTokens[token] = refreshToken with { IsRevoked = true };
        }
    }
}

/// <summary>
/// EXAMPLE 3: COOKIE-BASED AUTHENTICATION
/// 
/// THE PROBLEM:
/// Traditional web applications need server-side session management.
/// JWT not ideal for browser-only apps (XSS vulnerability if stored in localStorage).
/// 
/// THE SOLUTION:
/// Use HTTP-only, secure cookies with ASP.NET Core Cookie Authentication.
/// Browser automatically sends cookie with each request.
/// Server validates cookie and loads user claims.
/// 
/// WHY IT MATTERS:
/// - XSS protection (HttpOnly cookies can't be accessed by JavaScript)
/// - CSRF protection (with anti-forgery tokens)
/// - Server controls session lifetime
/// - Works with MVC/Razor Pages
/// 
/// BEST FOR: Traditional web apps, admin panels, internal tools
/// NOT FOR: APIs consumed by mobile apps or SPAs on different domains
/// </summary>
public static class CookieAuthenticationSetup
{
    public static void ConfigureCookieAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>
            {
                // ✅ GOOD: Secure cookie configuration
                options.Cookie.Name = "MyApp.Auth";
                options.Cookie.HttpOnly = true; // Prevent XSS attacks
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
                options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
                options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cookie lifetime
                options.SlidingExpiration = true; // Renew cookie on activity

                // Redirect paths
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";

                // ❌ BAD: Storing sensitive data in cookie
                // Don't store password, credit cards, SSN in cookie!

                // ✅ GOOD: Store minimal claims (userId, name, roles)
            });
    }

    /// <summary>
    /// Login user with cookie authentication
    /// </summary>
    public static async Task<IResult> LoginUser(
        HttpContext context,
        string username,
        string password)
    {
        // Validate credentials (check database)
        // var user = await _userService.ValidateCredentials(username, password);
        // if (user == null) return Results.Unauthorized();

        // ✅ GOOD: Create claims for authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // ✅ GOOD: Sign in user (creates encrypted cookie)
        await context.SignInAsync("CookieAuth", claimsPrincipal, new AuthenticationProperties
        {
            IsPersistent = true, // "Remember me"
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

        return Results.Ok(new { Message = "Logged in successfully" });
    }

    /// <summary>
    /// Logout user
    /// </summary>
    public static async Task<IResult> LogoutUser(HttpContext context)
    {
        // ✅ GOOD: Sign out (deletes cookie)
        await context.SignOutAsync("CookieAuth");
        return Results.Ok(new { Message = "Logged out successfully" });
    }
}

/// <summary>
/// EXAMPLE 4: OAUTH 2.0 / OPENID CONNECT
/// 
/// THE PROBLEM:
/// Want users to login with Google, Microsoft, GitHub, etc. (social login).
/// Don't want to manage passwords or user credentials.
/// Need to integrate with external identity providers.
/// 
/// THE SOLUTION:
/// Use OAuth 2.0 / OpenID Connect to delegate authentication to external providers.
/// User redirects to provider, authenticates, returns with authorization code.
/// Your app exchanges code for access token and user info.
/// 
/// WHY IT MATTERS:
/// - No password management burden
/// - Improved UX (users don't create yet another account)
/// - Enterprise SSO integration ability
/// - Higher security (leverage big providers' security)
/// 
/// OAUTH 2.0 FLOW (Authorization Code):
/// 1. User clicks "Login with Google"
/// 2. Redirect to Google with client_id, redirect_uri, scope
/// 3. User authenticates on Google
/// 4. Google redirects back with authorization code
/// 5. App exchanges code for access token (server-side)
/// 6. App gets user info from Google API
/// 7. App creates local session/token
/// 
/// GOTCHA: Store client_secret securely! Use PKCE for public clients!
/// </summary>
public static class OAuthSetup
{
    public static void ConfigureOAuthProviders(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
        /* Requires: dotnet add package Microsoft.AspNetCore.Authentication.Google
         *           dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount

        // ✅ GOOD: Google OAuth configuration
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
            options.SaveTokens = true; // Save access token for API calls

            // Request additional scopes
            options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
            options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");

            // Map claims
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        })

        // ✅ GOOD: Microsoft OAuth configuration
        .AddMicrosoftAccount(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
            options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
            options.SaveTokens = true;
        });
        */

        // ❌ BAD: Hardcoding client secrets
        // .AddGoogle(options => { options.ClientId = "123"; options.ClientSecret = "secret"; })
    }
}

/// <summary>
/// EXAMPLE 5: MULTI-FACTOR AUTHENTICATION (MFA) BASICS
/// 
/// THE PROBLEM:
/// Passwords alone are weak (phishing, leaks, brute force).
/// Need additional verification factor for sensitive operations.
/// 
/// THE SOLUTION:
/// Require second factor after password (TOTP, SMS, email, biometric).
/// Something you know (password) + something you have (phone/token).
/// 
/// WHY IT MATTERS:
/// - Dramatically reduces account takeover risk
/// - Compliance requirements (PCI-DSS, HIPAA, SOC 2)
/// - Protects sensitive operations
/// - Industry best practice
/// 
/// MFA FACTORS:
/// - Something you know: Password, PIN
/// - Something you have: Phone, hardware token, authenticator app
/// - Something you are: Fingerprint, face recognition
/// 
/// BEST PRACTICE: Use TOTP (Time-based One-Time Password) like Google Authenticator
/// </summary>
public class TotpService
{
    /// <summary>
    /// Generate TOTP secret for user (one-time setup)
    /// </summary>
    public string GenerateSecret()
    {
        // ✅ GOOD: Generate cryptographically secure secret
        var secretBytes = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(secretBytes);
        }
        return Convert.ToBase64String(secretBytes);
    }

    /// <summary>
    /// Generate current TOTP code (6 digits, 30-second window)
    /// </summary>
    public string GenerateCode(string secret)
    {
        var secretBytes = Convert.FromBase64String(secret);
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = unixTimestamp / 30; // 30-second window

        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        using var hmac = new HMACSHA1(secretBytes);
        var hash = hmac.ComputeHash(counterBytes);

        var offset = hash[^1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24) |
                     ((hash[offset + 1] & 0xFF) << 16) |
                     ((hash[offset + 2] & 0xFF) << 8) |
                     (hash[offset + 3] & 0xFF);

        var code = binary % 1000000;
        return code.ToString("D6", CultureInfo.InvariantCulture); // 6-digit code with leading zeros
    }

    /// <summary>
    /// Validate TOTP code (allow previous/next 30-sec window for clock skew)
    /// </summary>
    public bool ValidateCode(string secret, string userCode)
    {
        // ✅ GOOD: Check current window and adjacent windows for clock skew
        var currentCode = GenerateCode(secret);

        // Generate codes for previous and next 30-second windows
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var previousCode = GenerateCodeForCounter(secret, (unixTimestamp / 30) - 1);
        var nextCode = GenerateCodeForCounter(secret, (unixTimestamp / 30) + 1);

        return userCode == currentCode ||
               userCode == previousCode ||
               userCode == nextCode;
    }

    private static string GenerateCodeForCounter(string secret, long counter)
    {
        var secretBytes = Convert.FromBase64String(secret);
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        using var hmac = new HMACSHA1(secretBytes);
        var hash = hmac.ComputeHash(counterBytes);

        var offset = hash[^1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24) |
                     ((hash[offset + 1] & 0xFF) << 16) |
                     ((hash[offset + 2] & 0xFF) << 8) |
                     (hash[offset + 3] & 0xFF);

        var code = binary % 1000000;
        return code.ToString("D6", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Generate QR code URL for authenticator apps
    /// </summary>
    public string GenerateQrCodeUrl(string userEmail, string issuer, string secret)
    {
        // Format: otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}
        var label = $"{issuer}:{userEmail}";
        var secretBase32 = ConvertToBase32(Convert.FromBase64String(secret));
        return $"otpauth://totp/{Uri.EscapeDataString(label)}?secret={secretBase32}&issuer={Uri.EscapeDataString(issuer)}";
    }

    private static string ConvertToBase32(byte[] input)
    {
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var result = new StringBuilder();

        for (int offset = 0; offset < input.Length;)
        {
            int numCharsInThisGroup = Math.Min(5, input.Length - offset);
            byte[] group = new byte[5];
            Array.Copy(input, offset, group, 0, numCharsInThisGroup);

            int buffer = 0;
            for (int i = 0; i < 5; i++)
            {
                buffer = (buffer << 8) | group[i];
            }

            for (int i = 0; i < 8; i++)
            {
                int index = (buffer >> (35 - (i * 5))) & 0x1F;
                result.Append(base32Chars[index]);
            }

            offset += 5;
        }

        return result.ToString();
    }
}

/// <summary>
/// EXAMPLE 6: ASP.NET CORE CONFIGURATION
/// 
/// Complete authentication pipeline setup
/// </summary>
public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        // ✅ GOOD: Multiple authentication schemes
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
        .AddJwtBearer("Bearer", options =>
        {
            var secretKey = builder.Configuration["Jwt:SecretKey"]!;
            var key = Encoding.UTF8.GetBytes(secretKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            // ✅ GOOD: Handle authentication events
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine($"Token validated for: {context.Principal!.Identity!.Name}");
                    return Task.CompletedTask;
                }
            };
        });
    }

    public static void UseAuthenticationPipeline(WebApplication app)
    {
        // ✅ CORRECT ORDER: Authentication BEFORE Authorization
        app.UseAuthentication(); // WHO you are
        app.UseAuthorization();  // WHAT you can do
    }
}
