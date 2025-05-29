using VibeSync.Domain.Enums;

namespace VibeSync.Application.Requests;

public sealed record GetSuggestionsRequest(Guid SpaceAdminToken, SuggestionFilterTimeEnum TimeFilter, int Amount = 10);