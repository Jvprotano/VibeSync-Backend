using VibeSync.Domain.Models;

namespace VibeSync.Application.Contracts.Repositories;

public interface ISpaceRepository
{
    Task<Space> GetSpaceByPublicToken(Guid token);
    Task<Space> GetSpaceByAdminToken(Guid token);
    Task<Space> CreateSpace(Space space);
}
