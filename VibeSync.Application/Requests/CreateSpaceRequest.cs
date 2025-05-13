namespace VibeSync.Application.Requests;

public sealed record CreateSpaceRequest(string Name, DateTime EventDate, Guid? UserId = null);