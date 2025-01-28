using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.Contracts.UseCases;

public interface ISpaceUseCase
{
    Task<SpaceResponse> CreateSpace(CreateSpaceRequest request);
    Task<SpaceResponse> GetSpaceById(Guid guid);
}