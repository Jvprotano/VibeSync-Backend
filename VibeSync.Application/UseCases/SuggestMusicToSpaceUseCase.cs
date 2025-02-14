using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class SuggestMusicToSpaceUseCase(ISuggestionRepository suggestionRepository) : IUseCase<SuggestMusicRequest, SuggestionResponse>
{
    public async Task<SuggestionResponse> Execute(SuggestMusicRequest request)
    {
        var response = await suggestionRepository.CreateSuggestion(request.AsModel());

        return response.AsDomain();
    }
}
