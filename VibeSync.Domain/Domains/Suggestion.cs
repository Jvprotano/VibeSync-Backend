namespace VibeSync.Domain.Models;

public class Suggestion : BaseEntity
{
    public Suggestion(Guid spaceId, string songId, string? suggestedBy = null)
    {
        SpaceId = spaceId;
        SongId = songId;
        SuggestedBy = suggestedBy;
    }

    public string SongId { get; private set; }
    public Space? Space { get; private set; }
    public Guid SpaceId { get; private set; }
    public string? SuggestedBy { get; private set; }
}