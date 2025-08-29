namespace VibeSync.Application.Requests;

public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
