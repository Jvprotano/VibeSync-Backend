using VibeSync.Domain.Models;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISpaceRepository
{
    Task<Space> GetSpaceByPublicTokenAsync(Guid token);
    Task<Space> GetSpaceByAdminTokenAsync(Guid token);
    Task<IEnumerable<Space>> GetSpacesByUserIdAsync(string userId);
    Task<Space> CreateSpaceAsync(Space space);
}
