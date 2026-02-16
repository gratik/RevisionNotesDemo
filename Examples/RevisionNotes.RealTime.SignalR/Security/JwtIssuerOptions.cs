namespace RevisionNotes.RealTime.SignalR.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.RealTime.SignalR";
    public string Audience { get; init; } = "RevisionNotes.RealTime.SignalR.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
