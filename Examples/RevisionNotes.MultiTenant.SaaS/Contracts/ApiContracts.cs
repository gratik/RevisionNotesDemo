namespace RevisionNotes.MultiTenant.SaaS.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record CreateTenantProjectRequest(string Name);
