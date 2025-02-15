namespace VibeSync.Application.Requests;

public class PageFilter
{
    public int PageNumber { get; set; }
    public string? PageToken { get; set; }
}