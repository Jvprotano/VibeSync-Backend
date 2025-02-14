namespace VibeSync.Application.Requests;

public sealed record GetSuggestionsRequest(Guid SpaceId, DateTime? StartDateTime, DateTime? EndDateTime);