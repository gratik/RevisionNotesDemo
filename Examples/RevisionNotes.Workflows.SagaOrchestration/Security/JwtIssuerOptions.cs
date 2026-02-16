namespace RevisionNotes.Workflows.SagaOrchestration.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "RevisionNotes.Workflows.SagaOrchestration";
    public string Audience { get; init; } = "RevisionNotes.Workflows.SagaOrchestration.Client";
    public string SigningKey { get; init; } = "Replace-With-Long-Random-Signing-Key-For-Production-Only";
    public int ExpiresMinutes { get; init; } = 20;
}
