namespace RevisionNotes.DataAccess.AdvancedEfCore.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.DataAccess.AdvancedEfCore";
    public string Audience { get; init; } = "RevisionNotes.DataAccess.AdvancedEfCore.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
