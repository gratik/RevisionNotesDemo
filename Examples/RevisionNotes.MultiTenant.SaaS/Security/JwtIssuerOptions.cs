namespace RevisionNotes.MultiTenant.SaaS.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.MultiTenant.SaaS";
    public string Audience { get; init; } = "RevisionNotes.MultiTenant.SaaS.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
