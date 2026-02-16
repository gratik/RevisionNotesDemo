namespace RevisionNotes.StandardApi.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "revision-notes-standard-api";
    public string Audience { get; init; } = "revision-notes-clients";
    public string SigningKey { get; init; } = "ReplaceWithLongRandomSigningKey-DevelopmentOnly";
    public int TokenLifetimeMinutes { get; init; } = 30;
}