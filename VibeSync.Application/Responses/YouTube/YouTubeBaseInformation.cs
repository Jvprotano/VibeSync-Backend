namespace VibeSync.Application.Responses.YouTube;

public class YouTubeBaseInformation
{
    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public Thumbnails Thumbnails { get; set; } = new();
        public string ChannelTitle { get; set; } = string.Empty;
    }

    public class Thumbnails
    {
        public ThumbnailInfo Default { get; set; } = new();
        public ThumbnailInfo Medium { get; set; } = new();
        public ThumbnailInfo High { get; set; } = new();
    }

    public class ThumbnailInfo
    {
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
