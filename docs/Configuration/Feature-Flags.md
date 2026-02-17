# Feature Flags

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET configuration providers and environment layering basics.
- Related examples: docs/Configuration/README.md
> Subject: [Configuration](../README.md)

## Feature Flags

### Configuration-Based Feature Flags

`csharp
// appsettings.json
{
  "FeatureFlags": {
    "EnableNewDashboard": true,
    "EnableBetaFeatures": false,
    "EnableCache": true
  }
}

// ✅ Feature flag settings class
public class FeatureFlags
{
    public bool EnableNewDashboard { get; set; }
    public bool EnableBetaFeatures { get; set; }
    public bool EnableCache { get; set; }
}

// Register
builder.Services.Configure<FeatureFlags>(
    builder.Configuration.GetSection("FeatureFlags"));

// ✅ Use in service
public class DashboardService
{
    private readonly FeatureFlags _features;
    
    public DashboardService(IOptions<FeatureFlags> options)
    {
        _features = options.Value;
    }
    
    public Dashboard GetDashboard()
    {
        if (_features.EnableNewDashboard)
        {
            return new NewDashboard();
        }
        return new LegacyDashboard();
    }
}
`

### Microsoft.FeatureManagement

`csharp
// ✅ Install package: Microsoft.FeatureManagement.AspNetCore

// appsettings.json
{
  "FeatureManagement": {
    "BetaFeatures": true,
    "PremiumFeatures": {
      "EnabledFor": [
        {
          "Name": "Percentage",
          "Parameters": {
            "Value": 50  // 50% rollout
          }
        }
      ]
    }
  }
}

// Register
builder.Services.AddFeatureManagement();

// ✅ Check feature in controller
public class FeaturesController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    
    public FeaturesController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }
    
    [HttpGet("beta")]
    public async Task<IActionResult> GetBetaFeature()
    {
        if (await _featureManager.IsEnabledAsync("BetaFeatures"))
        {
            return Ok(new { feature = "Beta content" });
        }
        return Ok(new { feature = "Stable content" });
    }
}

// ✅ Use attribute
[FeatureGate("BetaFeatures")]
[HttpGet("beta-only")]
public IActionResult BetaOnlyEndpoint()
{
    return Ok("This is only available when BetaFeatures is enabled");
}
`

---


## Interview Answer Block
30-second answer:
- Feature Flags is about environment-aware application configuration strategy. It matters because configuration errors cause major runtime failures.
- Use it when safely managing settings across local, CI, and production.

2-minute answer:
- Start with the problem Feature Flags solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized config controls vs deployment flexibility.
- Close with one failure mode and mitigation: missing validation and secret handling discipline.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Feature Flags but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Feature Flags, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Feature Flags and map it to one concrete implementation in this module.
- 3 minutes: compare Feature Flags with an alternative, then walk through one failure mode and mitigation.