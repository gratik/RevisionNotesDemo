namespace RevisionNotes.Identity.AuthServer.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.Identity.AuthServer";
    public string Audience { get; init; } = "RevisionNotes.Identity.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
