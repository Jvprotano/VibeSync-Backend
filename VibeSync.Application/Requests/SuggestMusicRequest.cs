namespace VibeSync.Application.Requests;

public sealed record SuggestSongRequest(Guid spaceToken, string SongId, string? SuggestedBy);