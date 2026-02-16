// ==============================================================================
// FRONT-END UI - REACT + .NET API INTEGRATION
// ==============================================================================
// WHAT IS THIS?
// -------------
// Common patterns for connecting a React SPA to an ASP.NET Core API.
//
// WHY IT MATTERS
// --------------
// ✅ Keeps API calls consistent and testable
// ✅ Reduces CORS/auth/error handling defects
//
// WHEN TO USE
// -----------
// ✅ SPA front end (React) with separate .NET API backend
// ✅ Teams with independent frontend/backend release cadence
//
// WHEN NOT TO USE
// ---------------
// ❌ Server-rendered app where API and UI are tightly coupled in one MVC app
// ❌ Tiny prototypes where API contract discipline is unnecessary
//
// REAL-WORLD EXAMPLE
// ------------------
// Shared API client with cancellation, auth header injection, and ProblemDetails parsing.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative React + .NET API snippets showing production-oriented practices.
/// </summary>
public static class ReactApiIntegrationExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("React + .NET API integration examples are illustrative only.");
        Console.WriteLine("See docs/DotNet-API-React.md for the full guide.");
    }

    /// <summary>
    /// BAD: Repeats base URL, skips cancellation, and ignores API errors.
    /// </summary>
    private const string BadReactFetch = @"export async function loadOrders() {
  const res = await fetch('https://localhost:5001/api/orders');
  return await res.json(); // Throws opaque errors on non-2xx
}";

    /// <summary>
    /// GOOD: Centralized API client with auth header, cancellation, and ProblemDetails handling.
    /// </summary>
    private const string GoodReactApiClient = @"const API_BASE = import.meta.env.VITE_API_BASE_URL;

export async function apiGet(path, token, signal) {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    signal,
  });

  if (!res.ok) {
    let details = { title: 'Request failed', status: res.status };
    try { details = await res.json(); } catch {}
    throw new Error(`${details.title} (${details.status})`);
  }

  return await res.json();
}";

    /// <summary>
    /// GOOD: React query hook pattern with cancellation and controlled loading states.
    /// </summary>
    private const string GoodReactHook = @"import { useEffect, useState } from 'react';
import { apiGet } from './apiClient';

export function useOrders(token) {
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const controller = new AbortController();
    setIsLoading(true);

    apiGet('/api/orders', token, controller.signal)
      .then(setOrders)
      .catch((err) => {
        if (err.name !== 'AbortError') setError(err.message);
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, [token]);

  return { orders, isLoading, error };
}";

    /// <summary>
    /// BAD: Overly permissive CORS policy and no explicit SPA origin control.
    /// </summary>
    private const string BadApiCors = @"builder.Services.AddCors(o =>
{
    o.AddPolicy(""SpaPolicy"", p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});";

    /// <summary>
    /// GOOD: Explicit CORS policy and middleware order for React SPA.
    /// </summary>
    private const string GoodApiCors = @"builder.Services.AddCors(o =>
{
    o.AddPolicy(""SpaPolicy"", p => p
        .WithOrigins(""https://app.example.com"", ""http://localhost:5173"")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

app.UseRouting();
app.UseCors(""SpaPolicy"");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();";

    /// <summary>
    /// GOOD: Stable DTO contract for SPA-facing endpoints.
    /// </summary>
    private const string GoodApiContract = @"public sealed record OrderDto(
    Guid Id,
    string Number,
    decimal Total,
    string Status,
    DateTimeOffset CreatedAtUtc);

[ApiController]
[Route(""api/orders"")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IReadOnlyList<OrderDto>> GetOrders(CancellationToken ct)
        => await _service.GetOrdersAsync(ct);
}";

    /// <summary>
    /// GOOD: Enterprise security baseline for SPA-facing APIs.
    /// </summary>
    private const string GoodApiSecurityBaseline = @"builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration[""Auth:Authority""];
        options.Audience = ""orders-api"";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(""orders.read"", p => p.RequireClaim(""scope"", ""orders.read""));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(""api"", limiter =>
    {
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.PermitLimit = 120;
        limiter.QueueLimit = 0;
    });
});

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(""SpaPolicy"");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();";

    /// <summary>
    /// GOOD: Structured logging + trace correlation + sensitive-data protection.
    /// </summary>
    private const string GoodApiLoggingBaseline = @"builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation());

app.Use(async (context, next) =>
{
    using (logger.BeginScope(new Dictionary<string, object?>
    {
        [""TraceId""] = Activity.Current?.TraceId.ToString(),
        [""Path""] = context.Request.Path.Value
    }))
    {
        await next();
    }
});

logger.LogInformation(""Order list requested for tenant {TenantId}"", tenantId); // Never log raw tokens/PII";

    /// <summary>
    /// GOOD: Frontend telemetry with request correlation and bounded error detail.
    /// </summary>
    private const string GoodReactTelemetry = @"const traceId = crypto.randomUUID();

await fetch(`${API_BASE}/api/orders`, {
  headers: {
    'X-Trace-Id': traceId,
    'Accept': 'application/json',
  },
});

captureEvent('orders_loaded', { traceId, count: orders.length });
captureError('orders_load_failed', { traceId, status, code: 'ORDERS_LOAD' });";
}
