using VibeSync.Application.Responses;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISongIntegrationRepository
{
    Task<YouTubeSearchResponse> SearchByTerm(string query, int pageSize = 10, string? pageToken = null);
    Task<YouTubeVideoResponse> SearchByVideoIds(string[] videoIds);
}