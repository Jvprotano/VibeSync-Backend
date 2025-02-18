namespace VibeSync.Application.Requests;

public sealed record GetSuggestionsRequest(Guid SpaceAdminToken, DateTime? StartDateTime, DateTime? EndDateTime, int Amount = 10);