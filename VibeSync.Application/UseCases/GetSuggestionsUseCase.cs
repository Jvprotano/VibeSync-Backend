using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;
using VibeSync.Domain.Models;

namespace VibeSync.Application.UseCases;

public class GetSuggestionsUseCase(ISuggestionRepository suggestionRepository, ISongIntegrationRepository songIntegrationRepository, ISpaceRepository spaceRepository) : IUseCase<GetSuggestionsRequest, IEnumerable<GetSuggestionsResponse>>
{
    public async Task<IEnumerable<GetSuggestionsResponse>> Execute(GetSuggestionsRequest request)
    {
        var space = await spaceRepository.GetSpaceByAdminTokenAsync(request.SpaceAdminToken) ?? throw new SpaceNotFoundException(request.SpaceAdminToken);

        var response = await suggestionRepository.GetSuggestions(space.Id, request.StartDateTime, request.EndDateTime, request.Amount);

        var songs = await GetSongsByResponse(response);

        return songs.Items.Select(song => GetSongResponse(song, response))
        .OrderByDescending(song => song.countSuggestions)
        .Take(request.Amount)
        .ToArray();
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
            song.Snippet.ChannelTitle,
            string.Empty,
            string.Empty,
            song.Snippet.PublishedAt
        ));
    }
}