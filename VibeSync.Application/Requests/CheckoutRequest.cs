namespace VibeSync.Application.Requests;

public sealed record CheckoutRequest(Guid PlanId, Guid UserId);