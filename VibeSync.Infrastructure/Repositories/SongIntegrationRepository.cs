using System.Text.Json;
using Microsoft.Extensions.Options;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Helpers;
using VibeSync.Application.Responses;

namespace VibeSync.Infrastructure.Repositories;

public class SongIntegrationRepository(IOptions<YouTubeSettings> settings, HttpClient httpClient) : ISongIntegrationRepository
{
    private readonly string _youtubeApiKey = settings.Value.ApiKey;
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

public async Task<YouTubeSearchResponse> SearchByTerm(string query, int pageSize = 10, string? pageToken = null)
{
    var url = GetRequestUriByTerm(query, _youtubeApiKey, pageSize, pageToken);

    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<YouTubeSearchResponse>(content, jsonOptions);

    return result ?? new();
}

private static string GetRequestUriByTerm(string query, string apiKey, int pageSize, string? pageToken)
{
    var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&type=video&q={query}&maxResults={pageSize}&videoCategoryId=10&key={apiKey}";
    
    if (!string.IsNullOrEmpty(pageToken))
        url += $"&pageToken={pageToken}";

    return url;
}
    public async Task<YouTubeVideoResponse> SearchByVideoIds(string[] videoIds)
    {
        var url = GetRequestUriByIds(string.Join(",", videoIds), _youtubeApiKey);

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<YouTubeVideoResponse>(content, jsonOptions);

        return result ?? new();
    }

    private static string GetRequestUriByIds(string videoIds, string apiKey)
    => $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={videoIds}&key={apiKey}";
}
