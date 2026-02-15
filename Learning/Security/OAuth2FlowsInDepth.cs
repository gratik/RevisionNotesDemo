// ==============================================================================
// OAuth2 Authorization Flows In-Depth
// ==============================================================================
// WHAT IS THIS?
// OAuth2 defines multiple authorization flows for different scenarios: Authorization Code (web apps), Client Credentials (service-to-service), PKCE (mobile/SPA), Device Code (smart TVs).
//
// WHY IT MATTERS
// ✅ INDUSTRY STANDARD: OAuth2 is ubiquitous (Google, GitHub, Microsoft sign-in) | ✅ DELEGATION: Apps access resources on your behalf without knowing your password | ✅ SCOPES: Granular permissions (read calendar, not write) | ✅ REVOKABLE: Users can revoke app access anytime | ✅ STATELESS: Tokens used, no server-side session
//
// WHEN TO USE
// ✅ "Sign in with Google/GitHub" | ✅ Mobile app calling backend API | ✅ Microservices calling each other | ✅ Third-party integrations
//
// WHEN NOT TO USE
// ❌ Internal APIs only | ❌ No delegation needed (direct username/password acceptable) | ❌ Simplicity paramount
//
// REAL-WORLD EXAMPLE
// Slack desktop app: Doesn't know your Slack password. Redirects to Slack.com, you authenticate, grants "read:messages" scope. Receives token. API uses token, not password. Revoke anytime on Slack.com.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Security;

public class OAuth2FlowsInDepth
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  OAuth2 Authorization Flows In-Depth");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        AuthorizationCodeFlow();
        ClientCredentialsFlow();
        PKCEFlow();
        ScopeManagement();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("OAuth2 vs Basic Auth:");
        Console.WriteLine("  Basic: Authorization: Basic base64(user:password)");
        Console.WriteLine("    Problem: App knows password, no scopes, revocation hard\n");
        
        Console.WriteLine("  OAuth2: Use token instead");
        Console.WriteLine("    Benefit: App never sees password, scopes limit access, revoke anytime\n");
    }

    private static void AuthorizationCodeFlow()
    {
        Console.WriteLine("1️⃣  AUTHORIZATION CODE FLOW (Web Apps):\n");
        
        Console.WriteLine("Step 1: User clicks \\\"Sign in with GitHub\\\"");
        Console.WriteLine("Step 2: Redirected to: github.com/login/oauth/authorize?client_id=xxx&scope=repo,user");
        Console.WriteLine("Step 3: User authenticates, grants permission");
        Console.WriteLine("Step 4: Redirected back with authorization code");
        Console.WriteLine("Step 5: Backend exchanges code for token (secure)");
        Console.WriteLine("Step 6: Token used for API requests\n");
        
        Console.WriteLine("Security: Authorization code short-lived, token stored securely on server\n");
    }

    private static void ClientCredentialsFlow()
    {
        Console.WriteLine("2️⃣  CLIENT CREDENTIALS (Service-to-Service):\n");
        
        Console.WriteLine("No user involved. Service calls service.");
        Console.WriteLine("  Service-A: POST /oauth/token");
        Console.WriteLine("    { \\\"client_id\\\": \\\"service-a\\\", \\\"client_secret\\\": \\\"secret123\\\" }");
        Console.WriteLine("  Service-B: Returns access token");
        Console.WriteLine("  Service-A: Uses token to call Service-B APIs\n");
        
        Console.WriteLine("Use: Batch jobs, cron tasks, microservices\n");
    }

    private static void PKCEFlow()
    {
        Console.WriteLine("3️⃣  PKCE (Proof Key for Code Exchange) - Mobile/SPA:\n");
        
        Console.WriteLine("Problem: Mobile app can't securely store client_secret");
        Console.WriteLine("Solution: PKCE adds dynamic code_verifier\n");
        
        Console.WriteLine("Step 1: Generate random code_verifier");
        Console.WriteLine("Step 2: code_challenge = SHA256(code_verifier)");
        Console.WriteLine("Step 3: Redirect with code_challenge");
        Console.WriteLine("Step 4: On callback, exchange code + code_verifier");
        Console.WriteLine("Step 5: Server verifies: SHA256(received_verifier) == stored_challenge\n");
        
        Console.WriteLine("Benefit: No client_secret needed, mobile apps secure\n");
    }

    private static void ScopeManagement()
    {
        Console.WriteLine("🔐 SCOPE GRANULARITY:\n");
        
        Console.WriteLine("Scope: Requested permissions");
        Console.WriteLine("  repo: Full control of private repositories");
        Console.WriteLine("  repo:status: Access commit status");
        Console.WriteLine("  user: Access user profile");
        Console.WriteLine("  user:email: Access user email");
        Console.WriteLine("  read:org: Read organization data\n");
        
        Console.WriteLine("Principle: Request minimum necessary scopes\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. VALIDATE STATE PARAMETER");
        Console.WriteLine("   Prevents CSRF attacks");
        Console.WriteLine("   Generate random state, verify on callback\n");
        
        Console.WriteLine("2. USE HTTPS EVERYWHERE");
        Console.WriteLine("   Tokens transmitted over HTTPS only\n");
        
        Console.WriteLine("3. REFRESH TOKENS");
        Console.WriteLine("   Access token: <1 hour lifespan");
        Console.WriteLine("   Refresh token: long-lived, use to get new access token\n");
        
        Console.WriteLine("4. REVOCATION");
        Console.WriteLine("   Users can revoke token on provider");
        Console.WriteLine("   App checks token valid before using\n");
    }
}
