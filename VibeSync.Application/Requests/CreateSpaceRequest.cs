namespace VibeSync.Application.Requests;

public sealed record CreateSpaceRequest(string Name, string UserEmail, DateTime? Expiration);