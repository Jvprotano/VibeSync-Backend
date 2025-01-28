namespace VibeSync.Application.Requests;

public sealed record CreateSpaceRequest(string Name, DateTime? Expiration);