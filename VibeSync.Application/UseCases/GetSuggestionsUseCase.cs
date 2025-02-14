using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Models;

namespace VibeSync.Application.UseCases;

public class GetSuggestionsUseCase(ISuggestionRepository suggestionRepository, ISongIntegrationRepository songIntegrationRepository) : IUseCase<GetSuggestionsRequest, IEnumerable<GetSuggestionsResponse>>
{
    public async Task<IEnumerable<GetSuggestionsResponse>> Execute(GetSuggestionsRequest request)
    {
        IEnumerable<Suggestion> response = suggestionRepository.GetSuggestions(request.SpaceId, request.StartDateTime, request.EndDateTime);

        var songs = await GetSongsByResponse(response);

        return songs.Items.Select(song => GetSongResponse(song, response)).ToList();
    }

    private async Task<YouTubeVideoResponse> GetSongsByResponse(IEnumerable<Suggestion> response)
    {
        var songIds = response.Select(s => s.SongId).ToArray();
        return await songIntegrationRepository.SearchByVideoIds(songIds.DistinctBy(id => id).ToArray());
    }

    private static GetSuggestionsResponse GetSongResponse(YouTubeVideoItem song, IEnumerable<Suggestion> response)
    {
        return new GetSuggestionsResponse(response.Count(s => s.SongId == song.Id), new SongResponse(
            song.Id,
            song.Snippet.Title,
            song.Snippet.Thumbnails.Default.Url,
            song.Snippet.ChannelTitle
        ));
    }
}