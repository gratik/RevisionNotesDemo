namespace RevisionNotes.Identity.AuthServer.Contracts;

public sealed record PasswordGrantRequest(string GrantType, string Username, string Password);
public sealed record RefreshGrantRequest(string GrantType, string RefreshToken);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInSeconds, string RefreshToken);
