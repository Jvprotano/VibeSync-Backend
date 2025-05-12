namespace VibeSync.Application.Requests;

public sealed record RegisterRequest(string Email, string Password, string FullName);