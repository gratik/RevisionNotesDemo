namespace RevisionNotes.EventSourcing.Cqrs.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.EventSourcing.Cqrs";
    public string Audience { get; init; } = "RevisionNotes.EventSourcing.Cqrs.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
