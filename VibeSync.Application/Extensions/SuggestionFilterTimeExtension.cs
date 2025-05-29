using VibeSync.Domain.Enums;

namespace VibeSync.Application.Extensions;

public static class SuggestionFilterTimeExtension
{
    public static DateTime GetStartDateTime(this SuggestionFilterTimeEnum filter)
    {
        return filter switch
        {
            SuggestionFilterTimeEnum.LAST_5_MINUTES => DateTime.UtcNow.AddMinutes(-5),
            SuggestionFilterTimeEnum.LAST_10_MINUTES => DateTime.UtcNow.AddMinutes(-10),
            SuggestionFilterTimeEnum.LAST_30_MINUTES => DateTime.UtcNow.AddMinutes(-30),
            SuggestionFilterTimeEnum.LAST_HOUR => DateTime.UtcNow.AddHours(-1),
            SuggestionFilterTimeEnum.ALL_TIME => DateTime.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
        };
    }
}
