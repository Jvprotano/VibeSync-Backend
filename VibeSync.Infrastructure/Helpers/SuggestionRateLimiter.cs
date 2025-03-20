namespace VibeSync.Infrastructure.Helpers;

public class SuggestionRateLimiter
{
    private static readonly Dictionary<string, DateTime> _suggestionHistory = new();
    private static readonly TimeSpan _cooldown = TimeSpan.FromMinutes(5);

    public static bool CanSuggest(string userIdentifier, string songId, Guid spacePublicToken)
    {
        string key = $"{userIdentifier}:{songId}:{spacePublicToken}";

        if (_suggestionHistory.TryGetValue(key, out var lastSuggested))
        {
            if (DateTime.UtcNow - lastSuggested < _cooldown)
                return false;
        }

        _suggestionHistory[key] = DateTime.UtcNow;
        return true;
    }
}
