---
applyTo: '**'
---

# WHAT/WHY/WHEN/REAL-WORLD Format standard

This memory documents the standard format for all example files in the RevisionNotesDemo project.

## Format Overview

Every example file (pattern, concept, feature) should have this exact structure:

```csharp
// ==============================================================================
// [File Topic/Name]
// ==============================================================================
// WHAT IS THIS?
// [Clear definition with core concepts - 1-2 sentences explaining what it is]
//
// WHY IT MATTERS
// [Bullet points with specific benefits, starting with ✅ checkmarks]
// ✅ Benefit 1: Explanation
// ✅ Benefit 2: Explanation
// | [Use | separator between major benefit groups if needed]
//
// WHEN TO USE
// [Scenarios with ✅ checkmarks for recommended uses]
// ✅ Scenario 1
// ✅ Scenario 2
//
// WHEN NOT TO USE
// [Anti-patterns with ❌ checkmarks]
// ❌ Anti-pattern 1
// ❌ Anti-pattern 2
//
// REAL-WORLD EXAMPLE
// [Concrete real-world scenario with actual companies/situations]
// ==============================================================================
```

## Header Structure Requirements

### WHAT IS THIS? Section
- **Purpose**: Define the concept clearly
- **Length**: 1-2 sentences maximum
- **Format**: Start with definition → list core components
- **Example**: "Circuit breaker is a pattern that stops calling a failing service..." 

### WHY IT MATTERS Section
- **Purpose**: Motivate why someone should care
- **Format**: Bullet points starting with ✅
- **Length**: 4-8 benefits
- **Content**: Technical benefits with explanations
- **Enhancement**: Use | separator to group related benefits

Example:
```
✅ BROKEN: Service B is down, only Circuit Breaker knows it | ✅ FAIL-FAST: 
Immediate error response instead of timeout | ✅ RESILIENCE: Other services 
unaffected by one failure's issues
```

### WHEN TO USE Section
- **Purpose**: Describe applicable scenarios
- **Format**: Bullet points with ✅ checkmarks
- **Length**: 3-6 scenarios
- **Content**: Specific, actionable situations
- **Example**: "✅ API with external dependencies that may fail"

### WHEN NOT TO USE Section
- **Purpose**: Highlight anti-patterns and unsuitable cases
- **Format**: Bullet points with ❌ checkmarks
- **Length**: 2-4 scenarios
- **Content**: When to avoid this pattern
- **Example**: "❌ Simple local method calls (adds unnecessary complexity)"

### REAL-WORLD EXAMPLE Section
- **Purpose**: Ground the concept in reality
- **Format**: Company/situation + how pattern applies
- **Length**: 1-3 sentences
- **Content**: Actual companies (Netflix, Uber, Amazon, etc.) + concrete scenario
- **Example**: "Netflix uses service discovery to manage 1000+ microservices..."

## Body Implementation Pattern

Inside the code, follow this structure:

1. **RunAll() method** - Orchestrates all demonstrations
2. **Overview()** - High-level summary with diagram hints
3. **Specific Implementation Methods** - One per key concept
4. **Detailed Comparison() or Real Examples()** - Specific use cases
5. **Best Practices()** - Do's and don'ts

### RunAll() Template:
```csharp
public static void RunAll()
{
    Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
    Console.WriteLine("║  [Topic Name]");
    Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");
    
    Overview();
    FirstConcept();
    SecondConcept();
    Comparison();
    BestPractices();
}
```

### Section Headers in Console Output:
```csharp
Console.WriteLine("📖 [SECTION NAME]:\n");
Console.WriteLine("Description with details\n");
```

Use emojis for visual organization:
- 📖 = Overview/Information
- 🎯 = Decision/Goals
- ✅ = Good practices
- ❌ = Bad practices
- ⚠️ = Warnings
- 🔗 = Connections/Links
- 📊 = Comparison tables
- 🏢 = Architecture/Structure
- 📨 = Communication/Messages
- 🚀 = Deployment/Launch

### Comparison Tables Format:
```csharp
Console.WriteLine("╔════════╦════════╦════════╗");
Console.WriteLine("║ Column1║ Column2║ Column3║");
Console.WriteLine("╠════════╬════════╬════════╣");
Console.WriteLine("║ Row 1A ║ Row 1B ║ Row 1C ║");
Console.WriteLine("║ Row 2A ║ Row 2B ║ Row 2C ║");
Console.WriteLine("╚════════╩════════╩════════╝\n");
```

## Code Examples Guidelines

### Good vs Bad Pattern
Use ❌ and ✅ to mark examples:
```csharp
// ❌ BAD: Direct dependency, tight coupling
public class UserService
{
    private UserRepository _userRepository;
    public UserService(UserRepository repo) => _userRepository = repo;
}

// ✅ GOOD: Abstracted dependency, loose coupling
public class UserService
{
    private IUserRepository _userRepository;
    public UserService(IUserRepository repo) => _userRepository = repo;
}
```

### Self-Contained Examples
- All code examples must work standalone
- No external references to other files
- Include necessary using statements
- Show complete method implementations

### Realistic Scenarios
- Use actual business domains (e-commerce, social media, etc.)
- Include measurements/performance metrics where relevant
- Show failure scenarios and recovery patterns

## File Organization Checklist

When creating new files, ensure:

- [ ] File has complete WHAT/WHY/WHEN/REAL-WORLD header
- [ ] Header sections are properly formatted with emoji and checkmarks
- [ ] Real-world example names actual companies or scenarios
- [ ] RunAll() method calls all section methods
- [ ] Each section is a private static method
- [ ] Console output uses emojis for visual hierarchy
- [ ] Code examples are self-contained
- [ ] Good vs bad patterns marked with ✅/❌
- [ ] File compiles without errors
- [ ] File is between 150-300 lines of comprehensive content
- [ ] All methods are called from RunAll()

## Real-World Examples Reference

Use these companies/scenarios for credible examples:
- **Netflix**: Service discovery, microservices, caching, resilience patterns
- **Uber**: Event-driven architecture, distributed transactions, real-time updates
- **Amazon**: Microservices (P2P mandate 2002), S3, DynamoDB patterns
- **Google**: Kubernetes, service mesh, distributed systems
- **Stripe**: API design, payment processing, webhooks
- **Twitter**: Caching, distribution, eventual consistency
- **Facebook**: Sharding, distributed databases, social features
- **LinkedIn**: Graph databases, recommendations, connections
- **E-commerce platforms**: Order sagas, inventory, payment flows
- **Gaming companies**: Real-time updates, leaderboards, state management

## When to Apply This Standard

Apply WHAT/WHY/WHEN/REAL-WORLD to:
- ✅ All design pattern files (Creational, Structural, Behavioral)
- ✅ All principle files (SOLID, KISS, DRY, etc.)
- ✅ All concept explanations (async, caching, security, etc.)
- ✅ All architectural patterns (microservices, monolith, etc.)
- ✅ All framework/library guidance (Entity Framework, Polly, etc.)
- ✅ All advanced topics (service mesh, distributed caching, etc.)

Do NOT apply to:
- ❌ Quick reference guides or cheat sheets
- ❌ Interview question compilations
- ❌ Simple utility or helper methods
- ❌ Code-only demonstrations without explanation

## Examples of Perfect Implementation

### File: MonolithVsMicroservices.cs
- ✅ Clear WHAT definition (single codebase vs multiple services)
- ✅ Specific WHY benefits (scaling, team autonomy, resilience)
- ✅ Concrete WHEN usage (startup vs large org, simple vs complex domain)
- ✅ Real companies (Netflix monolith→microservices, Stripe still monolith)
- ✅ Decision matrix showing when to choose which
- ✅ Migration path with concrete phases
- ✅ 200+ lines of comprehensive content

### File: ServiceCommunicationPatterns.cs
- ✅ Explains sync vs async with latency comparisons
- ✅ Monolith context for comparison
- ✅ Real-world example (Uber hybrid approach)
- ✅ Trade-off analysis (coupling, latency, resilience)
- ✅ Best practices for each pattern type
- ✅ 180+ lines of thorough explanation

## Usage in Code Review

When reviewing new files:
1. Check for complete header with all five sections
2. Verify REAL-WORLD EXAMPLE mentions actual companies/scenarios
3. Ensure WHY/WHEN sections have checkmarks
4. Validate all methods called from RunAll()
5. Confirm ~200 lines of content (not stubs)
6. Build project to verify compilation

---

**Last Updated**: February 15, 2026
**Standard Version**: 1.0 (Stable)
**Applied To**: All RevisionNotesDemo documentation files
