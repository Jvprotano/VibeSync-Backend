using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Application.UseCases;

public class GetSpaceByIdUseCase(ISpaceRepository spaceRepository) : IUseCase<Guid, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(Guid guid)
    {
        var response = await spaceRepository.GetSpaceById(guid) ?? throw new SpaceNotFoundException(guid); ;

        return response.AsDomain();
    }
}