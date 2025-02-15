using static VibeSync.Application.Responses.YouTube.YouTubeBaseInformation;

namespace VibeSync.Application.Responses;

public class YouTubeSearchResponse
{
    public string? NextPageToken { get; set; }
    public string? PrevPageToken { get; set; }
    public List<YouTubeSearchItem> Items { get; set; } = new();
}

public class YouTubeSearchItem
{
    public Id Id { get; set; } = new();
    public Snippet Snippet { get; set; } = new();
}

public class Id
{
    public string Kind { get; set; } = string.Empty;
    public string VideoId { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string? PlaylistId { get; set; }
}
