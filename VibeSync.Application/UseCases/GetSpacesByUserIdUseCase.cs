using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class GetSpacesByUserIdUseCase(ISpaceRepository spaceRepository) : IUseCase<string, IEnumerable<SpaceResponse>>
{
    public async Task<IEnumerable<SpaceResponse>> Execute(string request)
    {
        var spaces = await spaceRepository.GetSpacesByUserIdAsync(request.ToString());

        return spaces.Select(space => space.AsResponseModel());
    }
}
