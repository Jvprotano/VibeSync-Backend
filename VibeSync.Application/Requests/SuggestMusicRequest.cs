namespace VibeSync.Application.Requests;

public sealed record SuggestSongRequest(Guid SpaceId, string SongId, string? SuggestedBy);