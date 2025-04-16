using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Application.UseCases;

public class GetSpaceByAdminTokenUseCase(ISpaceRepository spaceRepository) : IUseCase<Guid, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(Guid publicToken)
    {
        var response = await spaceRepository.GetByAdminTokenAsync(publicToken) ?? throw new SpaceNotFoundException(publicToken); ;

        return response.AsResponseModel();
    }
}