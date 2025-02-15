using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class SuggestSongToSpaceUseCase(ISuggestionRepository suggestionRepository) : IUseCase<SuggestSongRequest, SuggestionResponse>
{
    public async Task<SuggestionResponse> Execute(SuggestSongRequest request)
    {
        var response = await suggestionRepository.CreateSuggestion(request.AsModel());

        return response.AsDomain();
    }
}
