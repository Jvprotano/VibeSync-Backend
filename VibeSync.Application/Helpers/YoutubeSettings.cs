namespace VibeSync.Application.Helpers;

public class YouTubeSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://www.googleapis.com/youtube/v3";
}

public static class YoutubeHelper
{
    public static string GetYoutubeUrl(string videoId)
    {
        return $"https://www.youtube.com/watch?v={videoId}";
    }
}