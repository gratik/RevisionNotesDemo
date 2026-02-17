# Feature Flags

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

