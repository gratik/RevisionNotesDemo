namespace RevisionNotes.Testing.Pyramid.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.Testing.Pyramid";
    public string Audience { get; init; } = "RevisionNotes.Testing.Pyramid.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
