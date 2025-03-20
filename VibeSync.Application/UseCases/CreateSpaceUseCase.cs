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
        if (request.Name.Length < 3)
            throw new ArgumentException("O nome do Space deve ter no mínimo 3 caracteres.");

        var response = await spaceRepository.CreateSpace(request.AsModel());

        return response.AsDomain();
    }
}