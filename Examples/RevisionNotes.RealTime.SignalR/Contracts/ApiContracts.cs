namespace RevisionNotes.RealTime.SignalR.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record SendGroupMessageRequest(string Group, string Message);
