using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class CreateSpaceUseCase(ISpaceRepository spaceRepository) : IUseCase<CreateSpaceRequest, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(CreateSpaceRequest request)
    {
        var response = await spaceRepository.CreateSpace(request.AsModel());

        return response.AsDomain();
    }
}