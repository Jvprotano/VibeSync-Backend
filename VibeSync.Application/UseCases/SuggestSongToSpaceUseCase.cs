using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Application.UseCases;

public class SuggestSongToSpaceUseCase(ISuggestionRepository suggestionRepository, ISpaceRepository spaceRepository) : IUseCase<SuggestSongRequest, SuggestionResponse>
{
    public async Task<SuggestionResponse> Execute(SuggestSongRequest request)
    {
        var space = await spaceRepository.GetSpaceByPublicTokenAsync(request.spaceToken) ?? throw new SpaceNotFoundException(request.spaceToken);

        var response = await suggestionRepository.CreateSuggestion(request.AsDomain(space.Id));

        return response.AsResponseModel();
    }
}
