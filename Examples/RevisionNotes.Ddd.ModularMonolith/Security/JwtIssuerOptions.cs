namespace RevisionNotes.Ddd.ModularMonolith.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.Ddd.ModularMonolith";
    public string Audience { get; init; } = "RevisionNotes.Ddd.ModularMonolith.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
