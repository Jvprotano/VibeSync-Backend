namespace VibeSync.Application.Responses;

public sealed record GetSuggestionsResponse(int countSuggestions, SongResponse song);