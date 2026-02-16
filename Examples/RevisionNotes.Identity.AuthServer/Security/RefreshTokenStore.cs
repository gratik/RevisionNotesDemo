using System.Collections.Concurrent;

namespace RevisionNotes.Identity.AuthServer.Security;

public interface IRefreshTokenStore
{
    RefreshTokenRecord Create(string username);
    string? TryConsume(string token);
}

public sealed record RefreshTokenRecord(string Token, string Username, DateTimeOffset ExpiresAtUtc);

public sealed class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshTokenRecord> _tokens = new();

    public RefreshTokenRecord Create(string username)
    {
        var record = new RefreshTokenRecord(
            Token: Guid.NewGuid().ToString("N"),
            Username: username,
            ExpiresAtUtc: DateTimeOffset.UtcNow.AddHours(6));

        _tokens[record.Token] = record;
        return record;
    }

    public string? TryConsume(string token)
    {
        if (!_tokens.TryRemove(token, out var record))
        {
            return null;
        }

        return record.ExpiresAtUtc < DateTimeOffset.UtcNow ? null : record.Username;
    }
}
