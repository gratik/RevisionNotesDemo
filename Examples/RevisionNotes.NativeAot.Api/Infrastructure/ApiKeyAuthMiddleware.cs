using Microsoft.Extensions.Options;

namespace RevisionNotes.NativeAot.Api.Infrastructure;

public sealed class ApiSecurityOptions
{
    public string HeaderName { get; init; } = "X-Api-Key";
    public string ApiKey { get; init; } = "ChangeMe-Aot-ApiKey";
}

public sealed class ApiKeyAuthMiddleware(
    IOptions<ApiSecurityOptions> options) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path == "/")
        {
            await next(context);
            return;
        }

        var headerName = options.Value.HeaderName;
        if (!context.Request.Headers.TryGetValue(headerName, out var provided) ||
            !string.Equals(provided.ToString(), options.Value.ApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"{{\"error\":\"unauthorized\",\"message\":\"Provide a valid {headerName} header.\"}}");
            return;
        }

        await next(context);
    }
}
