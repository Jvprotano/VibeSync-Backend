using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.Extensions;

public static class SuggestionExtension
{
    public static Suggestion AsDomain(this SuggestSongRequest suggestion, Guid spaceId)
    {
        return new Suggestion(
            spaceId,
            suggestion.SongId,
            suggestion.SuggestedBy
        );
    }

    public static SuggestionResponse AsResponseModel(this Suggestion suggestion)
    {
        return new SuggestionResponse(
            suggestion.SpaceId,
            suggestion.SongId
        );
    }
}