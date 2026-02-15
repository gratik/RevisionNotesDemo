// ==============================================================================
// OpenAPI/Swagger Advanced Specification & Tooling
// ==============================================================================
// WHAT IS THIS?
// OpenAPI (formerly Swagger) is a specification for describing REST APIs in machine-readable YAML/JSON. Tools auto-generate docs, SDKs, tests, mock servers.
//
// WHY IT MATTERS
// ✅ DOCUMENTATION AUTO-GENERATED: Stays in sync with code | ✅ SDK GENERATION: Generate client libraries (JavaScript, Python, etc.) | ✅ VALIDATION: Enforce request/response contracts | ✅ TESTING: Mock server for frontend devs | ✅ DISCOVERY: API versioning, deprecation management
//
// WHEN TO USE
// ✅ Public APIs | ✅ API versioning | ✅ Multiple client types | ✅ Team handoffs (frontend/backend)
//
// WHEN NOT TO USE
// ❌ Internal-only APIs (optional) | ❌ Simple CRUD endpoints (nice-to-have)
//
// REAL-WORLD EXAMPLE
// Stripe API: OpenAPI spec defines all endpoints. Auto-generates SDKs (Python, Node, Ruby, Go). Docs auto-rendered. Breaking changes detected (schema version bump). Client devs use mock server before backend ready.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.WebAPI;

public class OpenAPISwaggerAdvanced
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  OpenAPI/Swagger Advanced Specification");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        SpecificationExample();
        ToolingBenefits();
        VersionManagement();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("OpenAPI = standardized API description");
        Console.WriteLine("  Write YAML/JSON spec");
        Console.WriteLine("  Tools read spec");
        Console.WriteLine("  Auto-generate docs, SDKs, tests\n");
    }

    private static void SpecificationExample()
    {
        Console.WriteLine("📋 SPECIFICATION EXAMPLE (YAML):\n");

        Console.WriteLine("openapi: 3.0.0");
        Console.WriteLine("info:");
        Console.WriteLine("  title: Product API");
        Console.WriteLine("  version: 1.0.0");
        Console.WriteLine("paths:");
        Console.WriteLine("  /products:");
        Console.WriteLine("    get:");
        Console.WriteLine("      summary: List all products");
        Console.WriteLine("      parameters:");
        Console.WriteLine("        - name: limit");
        Console.WriteLine("          in: query");
        Console.WriteLine("          schema:");
        Console.WriteLine("            type: integer");
        Console.WriteLine("      responses:");
        Console.WriteLine("        '200':");
        Console.WriteLine("          description: Success");
        Console.WriteLine("          content:");
        Console.WriteLine("            application/json:");
        Console.WriteLine("              schema:");
        Console.WriteLine("                $ref: '#/components/schemas/Product'\n");
    }

    private static void ToolingBenefits()
    {
        Console.WriteLine("🛠️  TOOLING BENEFITS:\n");

        Console.WriteLine("1. SWAGGER UI");
        Console.WriteLine("   Auto-generated interactive docs");
        Console.WriteLine("   Try endpoint in browser\n");

        Console.WriteLine("2. SDK GENERATION");
        Console.WriteLine("   openapi-generator create SDK:");
        Console.WriteLine("   $ openapi-generator-cli generate -g typescript-node\n");

        Console.WriteLine("3. API VALIDATION");
        Console.WriteLine("   Spec2.0 vs Spec3.0 comparison");
        Console.WriteLine("   Detect breaking changes\n");

        Console.WriteLine("4. MOCK SERVER");
        Console.WriteLine("   Prism generates mock API from spec");
        Console.WriteLine("   Frontend dev without backend server\n");
    }

    private static void VersionManagement()
    {
        Console.WriteLine("📦 VERSIONING:\n");

        Console.WriteLine("v1 spec: /products returns id, name, price");
        Console.WriteLine("v2 spec: /products returns id, name, price, tags, reviews\n");

        Console.WriteLine("In OpenAPI:");
        Console.WriteLine("  Path: /v1/products or /v2/products (or headers)");
        Console.WriteLine("  info.version: tracks API version");
        Console.WriteLine("  Clients use matching schema version\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");

        Console.WriteLine("1. GENERATE FROM CODE");
        Console.WriteLine("   Use attributes: [ApiDescription], [ApiVersion]");
        Console.WriteLine("   Spec auto-generated (single source of truth)\n");

        Console.WriteLine("2. VALIDATE REQUESTS");
        Console.WriteLine("   Use middleware to validate against spec\n");

        Console.WriteLine("3. DEPRECATION NOTICE");
        Console.WriteLine("   deprecated: true in spec");
        Console.WriteLine("   Tools warn clients\n");

        Console.WriteLine("4. TEST AGAINST SPEC");
        Console.WriteLine("   Auto-generate tests from spec\n");
    }
}
