using VibeSync.Application.Responses;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISongIntegrationRepository
{
    Task<YouTubeSearchResponse> SearchByTerm(string query);
    Task<YouTubeVideoResponse> SearchByVideoIds(string[] videoIds);
}