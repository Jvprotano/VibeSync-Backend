using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure.Repositories;

public class SuggestionRepository(AppDbContext appDbContext) : ISuggestionRepository
{
    public async Task<Suggestion> CreateSuggestion(Suggestion request)
    {
        await appDbContext.Suggestions.AddAsync(request);
        await appDbContext.SaveChangesAsync();
        return request;
    }

    public async Task<IEnumerable<Suggestion>> GetSuggestions(Guid spaceId, DateTime? startDateTime, DateTime? endDateTime, int? amount)
    {
        var query = appDbContext.Suggestions.Where(s => s.SpaceId == spaceId);

        if (startDateTime.HasValue)
            query = query.Where(s => s.CreatedAt >= startDateTime);

        if (endDateTime.HasValue)
            query = query.Where(s => s.CreatedAt <= endDateTime);

        return await query.ToArrayAsync();
    }
}