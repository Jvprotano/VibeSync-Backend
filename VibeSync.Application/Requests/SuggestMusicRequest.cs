namespace VibeSync.Application.Requests;

public sealed record SuggestMusicRequest(Guid SpaceId, string SongId, string? SuggestedBy);