namespace VibeSync.Application.Responses;

public sealed record SpaceResponse(Guid Id, string Name, string Link, string QrCode);