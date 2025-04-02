namespace VibeSync.Application.Responses;

public sealed record ErrorResponse(string Message, int StatusCode = 500, string? Details = null, string? Type = null);