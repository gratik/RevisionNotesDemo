using Microsoft.Extensions.Options;

namespace RevisionNotes.MultiTenant.SaaS.Tenants;

public sealed class TenantCatalogOptions
{
    public string[] AllowedTenantIds { get; init; } = ["tenant-a", "tenant-b"];
}

public sealed class TenantResolutionMiddleware(
    IOptions<TenantCatalogOptions> options) : IMiddleware
{
    public const string TenantHeaderName = "X-Tenant-Id";
    public const string TenantItemKey = "tenant.id";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(TenantHeaderName, out var requestedTenant) ||
            string.IsNullOrWhiteSpace(requestedTenant))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "tenant_required",
                message = $"Provide header {TenantHeaderName}."
            });
            return;
        }

        var tenantId = requestedTenant.ToString().Trim().ToLowerInvariant();
        if (!options.Value.AllowedTenantIds.Contains(tenantId, StringComparer.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "tenant_not_allowed",
                message = $"Tenant '{tenantId}' is not configured."
            });
            return;
        }

        context.Items[TenantItemKey] = tenantId;
        await next(context);
    }
}

public static class TenantHttpContextExtensions
{
    public static string GetTenantId(this HttpContext context)
    {
        if (context.Items.TryGetValue(TenantResolutionMiddleware.TenantItemKey, out var value) &&
            value is string tenantId &&
            !string.IsNullOrWhiteSpace(tenantId))
        {
            return tenantId;
        }

        throw new InvalidOperationException("Tenant is missing from request context.");
    }
}
