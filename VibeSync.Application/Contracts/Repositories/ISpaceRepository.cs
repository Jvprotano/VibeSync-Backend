using VibeSync.Domain.Models;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISpaceRepository
{
    Task<Space?> GetByPublicTokenAsync(Guid token);
    Task<Space?> GetByAdminTokenAsync(Guid token);
    Task<IEnumerable<Space>> GetSpacesByUserIdAsync(Guid userId);
    Task<Space> CreateAsync(Space space);
}
