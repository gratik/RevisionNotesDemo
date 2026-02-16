namespace RevisionNotes.gRPC.Service.Security;

public sealed class JwtIssuerOptions
{
    public string Issuer { get; init; } = "revision-notes-grpc-service";
    public string Audience { get; init; } = "revision-notes-grpc-clients";
    public string SigningKey { get; init; } = "UseAtLeast32CharsAndRotateInProduction123!";
    public int TokenLifetimeMinutes { get; init; } = 30;
}

public sealed record LoginRequest(string Username, string Password);