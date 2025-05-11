using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class GetSpacesByUserIdUseCase(ISpaceRepository spaceRepository) : IUseCase<Guid, IEnumerable<SpaceResponse>>
{
    public async Task<IEnumerable<SpaceResponse>> Execute(Guid userId)
    {
        var spaces = await spaceRepository.GetSpacesByUserIdAsync(userId);

        return spaces.Select(space => space.AsResponseModel());
    }
}
