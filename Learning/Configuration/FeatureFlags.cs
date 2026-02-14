// ==============================================================================
// FEATURE FLAGS - Safe Feature Deployment
// ==============================================================================
// PURPOSE:
//   Master feature flags for safe, gradual feature rollout.
//   Deploy code disabled, enable for testing, gradually roll out.
//
// WHY FEATURE FLAGS:
//   - Deploy without releasing
//   - Test in production safely
//   - Gradual rollout (1% → 10% → 100%)
//   - Instant killswitch
//   - A/B testing
//
// WHAT YOU'LL LEARN:
//   1. Basic feature toggles
//   2. Microsoft.FeatureManagement library
//   3. Per-user/per-tenant flags
//   4. Time-based activation
//   5. Percentage rollout
//   6. Integration with config/database
//
// KEY PRINCIPLE:
//   Separate deployment from release. Deploy dark, light up gradually.
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.FeatureManagement.Mvc;

namespace RevisionNotesDemo.Configuration;

/// <summary>
/// EXAMPLE 1: BASIC FEATURE FLAGS - Simple On/Off Toggles
/// 
/// THE PROBLEM:
/// New feature ready but not tested in production.
/// Don't want to maintain separate branch.
/// 
/// THE SOLUTION:
/// Feature flag - code deployed but disabled.
/// 
/// WHY IT MATTERS:
/// - Deploy code continuously
/// - Enable for internal testing
/// - Enable for everyone when ready
/// </summary>
public class BasicFeatureFlagsExamples
{
    // appsettings.json:
    // {
    //   "FeatureManagement": {
    //     "NewCheckoutFlow": false,
    //     "AdvancedSearch": true,
    //     "BetaFeatures": false
    //   }
    // }
    
    // ✅ Configure Microsoft.FeatureManagement
    public static void Configure(WebApplicationBuilder builder)
    {
        // Install: dotnet add package Microsoft.FeatureManagement.AspNetCore
        
        builder.Services.AddFeatureManagement();  // ✅ Register service
    }
    
    // ✅ GOOD: Check feature flag in code
    public class CheckoutService
    {
        private readonly IFeatureManager _featureManager;
        
        public CheckoutService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }
        
        public async Task<CheckoutResult> ProcessCheckout(Order order)
        {
            // ✅ Check if new feature is enabled
            if (await _featureManager.IsEnabledAsync("NewCheckoutFlow"))
            {
                return await ProcessCheckoutV2(order);  // ✅ New implementation
            }
            
            return await ProcessCheckoutV1(order);  // ✅ Old implementation (fallback)
        }
        
        private Task<CheckoutResult> ProcessCheckoutV1(Order order) =>
            Task.FromResult(new CheckoutResult { Success = true });
        
        private Task<CheckoutResult> ProcessCheckoutV2(Order order) =>
            Task.FromResult(new CheckoutResult { Success = true, Enhanced = true });
    }
    
    // ✅ GOOD: Feature flag in controller with attribute
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        [HttpGet("simple")]
        public IActionResult SimpleSearch(string query)
        {
            // Always available
            return Ok(new[] { "Result 1", "Result 2" });
        }
        
        [HttpGet("advanced")]
        [FeatureGate("AdvancedSearch")]  // ✅ Only available if feature enabled
        public IActionResult AdvancedSearch(string query, string? category, decimal? minPrice)
        {
            // Returns 404 if feature disabled
            return Ok(new[] { "Advanced Result 1", "Advanced Result 2" });
        }
    }
    
    public class Order { }
    public class CheckoutResult { public bool Success { get; set; } public bool Enhanced { get; set; } }
}

/// <summary>
/// EXAMPLE 2: PERCENTAGE ROLLOUT - Gradual Feature Deployment
/// 
/// THE PROBLEM:
/// New feature might have bugs. Don't want to impact all users.
/// 
/// THE SOLUTION:
/// Percentage-based rollout: 1% → 5% → 25% → 100%.
/// </summary>
public class PercentageRolloutExamples
{
    // appsettings.json:
    // {
    //   "FeatureManagement": {
    //     "NewDashboard": {
    //       "EnabledFor": [
    //         {
    //           "Name": "Percentage",
    //           "Parameters": {
    //             "Value": 25  // 25% of users
    //           }
    //         }
    //       ]
    //     }
    //   }
    // }
    
    public class DashboardService
    {
        private readonly IFeatureManager _featureManager;
        
        public DashboardService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }
        
        public async Task<Dashboard> GetDashboard(string userId)
        {
            // ✅ Percentage-based: Same user always gets same result (sticky)
            var context = new TargetingContext { UserId = userId };
            
            if (await _featureManager.IsEnabledAsync("NewDashboard", context))
            {
                return GetDashboardV2();  // ✅ 25% see new version
            }
            
            return GetDashboardV1();  // ✅ 75% see old version
        }
        
        private Dashboard GetDashboardV1() => new() { Version = 1 };
        private Dashboard GetDashboardV2() => new() { Version = 2 };
    }
    
    public class Dashboard { public int Version { get; set; } }
}

/// <summary>
/// EXAMPLE 3: USER/GROUP TARGETING - Per-User Feature Access
/// 
/// THE PROBLEM:
/// Enable feature for specific users, groups, or beta testers.
/// 
/// THE SOLUTION:
/// Targeting filter - conditional on user ID, group, email domain.
/// </summary>
public class UserTargetingExamples
{
    // appsettings.json:
    // {
    //   "FeatureManagement": {
    //     "BetaFeatures": {
    //       "EnabledFor": [
    //         {
    //           "Name": "Targeting",
    //           "Parameters": {
    //             "Audience": {
    //               "Users": [
    //                 "user1@example.com",
    //                 "user2@example.com"
    //               ],
    //               "Groups": [
    //                 "BetaTesters",
    //                 "InternalTeam"
    //               ],
    //               "DefaultRolloutPercentage": 0
    //             }
    //           }
    //         }
    //       ]
    //     }
    //   }
    // }
    
    // ✅ Configure targeting
    public static void ConfigureTargeting(WebApplicationBuilder builder)
    {
        builder.Services.AddFeatureManagement()
            .AddFeatureFilter<TargetingFilter>();  // ✅ Enable targeting filter
    }
    
    // ✅ GOOD: Check feature with user context
    public class FeatureService
    {
        private readonly IFeatureManager _featureManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public FeatureService(
            IFeatureManager featureManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _featureManager = featureManager;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<bool> CanAccessBetaFeatures()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;
            
            // ✅ Build targeting context from current user
            var context = new TargetingContext
            {
                UserId = httpContext.User.Identity?.Name ?? "",
                Groups = new[] { GetUserGroup(httpContext) }
            };
            
            return await _featureManager.IsEnabledAsync("BetaFeatures", context);
        }
        
        private string GetUserGroup(Microsoft.AspNetCore.Http.HttpContext context)
        {
            // Check if user is in beta group (from claims, database, etc.)
            if (context.User.HasClaim("Group", "BetaTesters"))
                return "BetaTesters";
            
            if (context.User.IsInRole("Internal"))
                return "InternalTeam";
            
            return "Standard";
        }
    }
}

/// <summary>
/// EXAMPLE 4: TIME-WINDOW ACTIVATION - Scheduled Features
/// 
/// THE PROBLEM:
/// Enable feature at specific time (e.g., Black Friday sale).
/// 
/// THE SOLUTION:
/// Time window filter - enable between start and end dates.
/// </summary>
public class TimeWindowExamples
{
    // appsettings.json:
    // {
    //   "FeatureManagement": {
    //     "BlackFridaySale": {
    //       "EnabledFor": [
    //         {
    //           "Name": "TimeWindow",
    //           "Parameters": {
    //             "Start": "2024-11-29T00:00:00Z",
    //             "End": "2024-12-02T23:59:59Z"
    //           }
    //         }
    //       ]
    //     },
    //     "NewYearPromo": {
    //       "EnabledFor": [
    //         {
    //           "Name": "TimeWindow",
    //           "Parameters": {
    //             "Start": "2025-01-01T00:00:00Z",
    //             "End": "2025-01-07T23:59:59Z"
    //           }
    //         }
    //       ]
    //     }
    //   }
    // }
    
    // ✅ Configure time window filter
    public static void ConfigureTimeWindow(WebApplicationBuilder builder)
    {
        builder.Services.AddFeatureManagement()
            .AddFeatureFilter<TimeWindowFilter>();  // ✅ Enable time filter
    }
    
    public class PricingService
    {
        private readonly IFeatureManager _featureManager;
        
        public PricingService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }
        
        public async Task<decimal> GetPrice(Product product)
        {
            var basePrice = product.Price;
            
            // ✅ Automatically enabled during Black Friday
            if (await _featureManager.IsEnabledAsync("BlackFridaySale"))
            {
                return basePrice * 0.7m;  // 30% off
            }
            
            // ✅ Automatically enabled during New Year
            if (await _featureManager.IsEnabledAsync("NewYearPromo"))
            {
                return basePrice * 0.85m;  // 15% off
            }
            
            return basePrice;
        }
    }
    
    public class Product { public decimal Price { get; set; } }
}

/// <summary>
/// EXAMPLE 5: CUSTOM FEATURE FILTERS - Business Logic Conditions
/// 
/// THE PROBLEM:
/// Need complex conditions (tenant, subscription tier, region).
/// 
/// THE SOLUTION:
/// Implement IFeatureFilter for custom logic.
/// </summary>
public class CustomFeatureFilterExamples
{
    // ✅ GOOD: Custom filter for subscription tier
    public class SubscriptionTierFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public SubscriptionTierFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var parameters = context.Parameters.Get<SubscriptionTierFilterSettings>();
            
            if (parameters == null)
                return Task.FromResult(false);
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return Task.FromResult(false);
            
            // ✅ Get user's subscription tier
            var userTier = httpContext.User.FindFirst("SubscriptionTier")?.Value;
            
            // ✅ Check if tier matches required tiers
            var isEnabled = parameters.RequiredTiers.Contains(userTier ?? "Free");
            
            return Task.FromResult(isEnabled);
        }
    }
    
    public class SubscriptionTierFilterSettings
    {
        public List<string> RequiredTiers { get; set; } = new();
    }
    
    // appsettings.json:
    // {
    //   "FeatureManagement": {
    //     "AdvancedAnalytics": {
    //       "EnabledFor": [
    //         {
    //           "Name": "SubscriptionTier",
    //           "Parameters": {
    //             "RequiredTiers": [ "Premium", "Enterprise" ]
    //           }
    //         }
    //       ]
    //     }
    //   }
    // }
    
    // ✅ Register custom filter
    public static void RegisterCustomFilter(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        builder.Services.AddFeatureManagement()
            .AddFeatureFilter<SubscriptionTierFilter>();  // ✅ Register custom filter
    }
    
    // ✅ Use in service
    public class AnalyticsService
    {
        private readonly IFeatureManager _featureManager;
        
        public AnalyticsService(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }
        
        public async Task<AnalyticsReport> GetReport()
        {
            // ✅ Only Premium/Enterprise users see advanced analytics
            if (await _featureManager.IsEnabledAsync("AdvancedAnalytics"))
            {
                return GetAdvancedReport();
            }
            
            return GetBasicReport();
        }
        
        private AnalyticsReport GetBasicReport() => new() { Type = "Basic" };
        private AnalyticsReport GetAdvancedReport() => new() { Type = "Advanced" };
    }
    
    public class AnalyticsReport { public string Type { get; set; } = ""; }
}

/// <summary>
/// EXAMPLE 6: KILLSWITCH PATTERN - Instant Feature Disable
/// 
/// THE PROBLEM:
/// New feature causing issues in production. Need immediate disable.
/// 
/// THE SOLUTION:
/// Feature flag + Azure App Configuration for instant updates.
/// </summary>
public class KillswitchExamples
{
    /* Requires: dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
    //           dotnet add package Azure.Identity
    // ✅ GOOD: Use Azure App Configuration for live updates
    public static void ConfigureAppConfiguration(WebApplicationBuilder builder)
    {
        // Connect to Azure App Configuration
        var connectionString = builder.Configuration["AppConfig:ConnectionString"];
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(connectionString)
                    // ✅ Watch for feature flag changes
                    .UseFeatureFlags(featureFlagOptions =>
                    {
                        featureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(30);
                    });
            });
        }
        
        builder.Services.AddFeatureManagement();
    }
    */
    
    // In Azure Portal:
    // 1. Navigate to App Configuration
    // 2. Feature Manager
    // 3. Toggle "NewPaymentFlow" : ON → OFF
    // 4. All instances pick up change in 30 seconds
    
    // ✅ GOOD: Fallback when feature disabled
    public class PaymentService
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<PaymentService> _logger;
        
        public PaymentService(
            IFeatureManager featureManager,
            ILogger<PaymentService> logger)
        {
            _featureManager = featureManager;
            _logger = logger;
        }
        
        public async Task<PaymentResult> ProcessPayment(decimal amount)
        {
            if (await _featureManager.IsEnabledAsync("NewPaymentFlow"))
            {
                try
                {
                    return await ProcessPaymentV2(amount);  // ✅ New system
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "New payment flow failed, falling back to v1");
                    // Fall through to V1
                }
            }
            
            return await ProcessPaymentV1(amount);  // ✅ Old system (safe fallback)
        }
        
        private Task<PaymentResult> ProcessPaymentV1(decimal amount) =>
            Task.FromResult(new PaymentResult { Success = true, Version = 1 });
        
        private Task<PaymentResult> ProcessPaymentV2(decimal amount) =>
            Task.FromResult(new PaymentResult { Success = true, Version = 2 });
    }
    
    public class PaymentResult { public bool Success { get; set; } public int Version { get; set; } }
}

// SUMMARY - Feature Flag Best Practices:
//
// ✅ DO:
// - Deploy code disabled, enable gradually
// - Use percentage rollout (1% → 10% → 100%)
// - Always provide fallback code path
// - Remove old code after 100% rollout
// - Use Azure App Configuration for live updates
// - Monitor metrics during rollout
// - Document feature flags (what they control, owner)
//
// ❌ DON'T:
// - Leave feature flags forever (tech debt)
// - Forget to handle "disabled" case
// - Deploy without testing both paths
// - Use feature flags for A/B testing experiments (use separate tool)
// - Nest too many feature flags (complexity)
//
// FLAG LIFECYCLE:
// 1. Create flag (disabled)
// 2. Deploy code
// 3. Enable for internal testing
// 4. Enable for 1% of users
// 5. Monitor metrics
// 6. Gradually increase: 5% → 25% → 50% → 100%
// 7. Remove flag and old code
//
// FEATURE FILTERS:
// - Percentage: Gradual rollout
// - Targeting: Specific users/groups
// - TimeWindow: Scheduled features
// - Custom: Business logic (subscription, region, tenant)
//
// STORAGE OPTIONS:
// - appsettings.json: Simple, requires redeploy
// - Azure App Configuration: Live updates, no redeploy
// - Database: Custom storage, full control
//
// MONITORING:
// - Track which users see which version
// - Compare metrics (error rate, performance)
// - Alerts when feature disabled (killswitch triggered)
