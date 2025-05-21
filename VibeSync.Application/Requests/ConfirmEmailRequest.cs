namespace VibeSync.Application.Requests;

public sealed record ConfirmEmailRequest(string UserId, string Token);