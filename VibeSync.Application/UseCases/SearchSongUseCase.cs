using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class SearchSongUseCase(ISongIntegrationRepository songIntegrationRepository) : IUseCase<string, IEnumerable<SongResponse>>
{
    public async Task<IEnumerable<SongResponse>> Execute(string query)
    {
        var result = await songIntegrationRepository.SearchByTerm(query);

        return result.Items.Select(item => new SongResponse(
            item.Id.VideoId,
            item.Snippet.Title,
            item.Snippet.Thumbnails.Default.Url,
            item.Snippet.ChannelTitle
        )).ToList();
    }
}
