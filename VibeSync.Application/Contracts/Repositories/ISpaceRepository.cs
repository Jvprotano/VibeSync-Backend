using VibeSync.Domain.Models;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISpaceRepository
{
    Task<Space?> GetSpaceById(Guid guid);
    Task<Space> CreateSpace(Space space);
}
