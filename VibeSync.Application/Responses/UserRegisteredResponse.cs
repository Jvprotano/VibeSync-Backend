namespace VibeSync.Application.Responses;

public sealed record UserRegisteredResponse(Guid UserId, string Email, string Message);