using static VibeSync.Application.Responses.YouTube.YouTubeBaseInformation;

namespace VibeSync.Application.Responses;

public class YouTubeVideoResponse
{
    public List<YouTubeVideoItem> Items { get; set; } = new();
}

public class YouTubeVideoItem
{
    public string Id { get; set; } = string.Empty;
    public Snippet Snippet { get; set; } = new();
}