namespace RevisionNotes.Resilience.ChaosDemo.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.Resilience.ChaosDemo";
    public string Audience { get; init; } = "RevisionNotes.Resilience.ChaosDemo.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
