using VibeSync.Domain.Models;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISuggestionRepository
{
    Task<Suggestion> CreateSuggestion(Suggestion request);
    Task<IEnumerable<Suggestion>> GetSuggestions(Guid spaceId, DateTime? startingDateTime, DateTime? endDateTime, int? Amount);
}