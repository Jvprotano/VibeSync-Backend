using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Application.UseCases;

public class GetSpaceByPublicTokenUseCase(ISpaceRepository spaceRepository) : IUseCase<Guid, GetPublicSpaceResponse>
{
    public async Task<GetPublicSpaceResponse> Execute(Guid publicToken)
    {
        var response = await spaceRepository.GetSpaceByPublicTokenAsync(publicToken) ?? throw new SpaceNotFoundException(publicToken); ;

        return response.AsPublicResponseModel();
    }
}