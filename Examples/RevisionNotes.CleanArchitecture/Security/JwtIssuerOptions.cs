namespace RevisionNotes.CleanArchitecture.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.CleanArchitecture";
    public string Audience { get; init; } = "RevisionNotes.CleanArchitecture.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
