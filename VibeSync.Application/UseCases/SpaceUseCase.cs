using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Helpers;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Application.UseCases;

public class SpaceUseCase(ISpaceRepository spaceRepository) : ISpaceUseCase
{
    public async Task<SpaceResponse> CreateSpace(CreateSpaceRequest request)
    {
        var response = await spaceRepository.CreateSpace(request.AsModel());

        return response.AsDomain();
    }

    public async Task<SpaceResponse> GetSpaceById(Guid guid)
    {
        try
        {
            var response = await spaceRepository.GetSpaceById(guid);

            return response.AsDomain();
        }
        catch (InvalidOperationException)
        {
            throw new SpaceNotFoundException(guid);
        }
        catch (Exception)
        {
            throw;
        }
    }
}