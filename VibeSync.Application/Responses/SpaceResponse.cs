namespace VibeSync.Application.Responses;

public sealed record SpaceResponse(Guid AdminToken, Guid PublicToken, string Name, string QrCode, DateTime EventDate);