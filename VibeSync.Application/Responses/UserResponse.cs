namespace VibeSync.Application.Responses;

public sealed record UserResponse(string Name, string Email, Guid Id, PlanResponse? Plan = null);