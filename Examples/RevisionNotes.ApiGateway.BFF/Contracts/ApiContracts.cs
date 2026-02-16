namespace RevisionNotes.ApiGateway.BFF.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record DashboardResponse(ProfileSummary Profile, IReadOnlyList<OrderSummary> Orders, bool UsedFallback);
public sealed record ProfileSummary(string UserId, string DisplayName, string Tier);
public sealed record OrderSummary(string OrderId, decimal Amount, string Status);
