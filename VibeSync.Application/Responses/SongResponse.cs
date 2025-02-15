namespace VibeSync.Application.Responses;

public sealed record SongResponse
(
    string VideoId,
    string Title,
    string ThumbnailUrl,
    string ArtistName,
    string? NextPageToken,
    string? PrevPageToken
);