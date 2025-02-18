using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class SearchSongUseCase(ISongIntegrationRepository songIntegrationRepository) : IUseCase<SearchSongRequest, IEnumerable<SongResponse>>
{
    public async Task<IEnumerable<SongResponse>> Execute(SearchSongRequest request)
    {
        var result = await songIntegrationRepository.SearchByTerm(request.Term, request.PageSize, request.PageToken);

        return result.Items.Select(item => new SongResponse(
            item.Id.VideoId,
            item.Snippet.Title,
            item.Snippet.Thumbnails.Default.Url,
            item.Snippet.ChannelTitle,
            result.NextPageToken,
            result.PrevPageToken,
            item.Snippet.PublishedAt
        )).ToList();
    }
}
