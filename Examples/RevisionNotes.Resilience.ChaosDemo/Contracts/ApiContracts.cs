namespace RevisionNotes.Resilience.ChaosDemo.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record ChaosConfigRequest(bool Enabled, int FailureRatePercent, int MaxDelayMs);
public sealed record ResilientValueResponse(string Value, bool FromCache, int Attempts, bool CircuitOpen);
