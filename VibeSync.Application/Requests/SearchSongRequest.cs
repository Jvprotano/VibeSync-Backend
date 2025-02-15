namespace VibeSync.Application.Requests;

public sealed record SearchSongRequest(string Term, int PageSize = 10, string? PageToken = null);