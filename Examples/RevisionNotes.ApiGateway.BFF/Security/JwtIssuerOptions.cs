namespace RevisionNotes.ApiGateway.BFF.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.ApiGateway.BFF";
    public string Audience { get; init; } = "RevisionNotes.ApiGateway.BFF.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
