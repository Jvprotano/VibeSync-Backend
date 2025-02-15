using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Extensions;

public static class SuggestionExtension
{
    public static Suggestion AsModel(this SuggestSongRequest suggestion)
    {
        return new Suggestion(
            suggestion.SpaceId,
            suggestion.SongId,
            suggestion.SuggestedBy
        );
    }

    public static SuggestionResponse AsDomain(this Suggestion suggestion)
    {
        return new SuggestionResponse(
            suggestion.SpaceId,
            suggestion.SongId
        );
    }
}