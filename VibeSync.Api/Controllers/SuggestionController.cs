using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Exceptions;
using VibeSync.Infrastructure.Helpers;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuggestionController(SuggestSongToSpaceUseCase suggestSongToSpaceUseCase, GetSuggestionsUseCase getSuggestionsUseCase) : ControllerBase
{
    [HttpPost("suggest")]
    [ProducesResponseType(typeof(SuggestionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Suggest([FromBody] SuggestSongRequest request)
    {
        try
        {
            string userIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString() ?? Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown";

            if (!SuggestionRateLimiter.CanSuggest(userIdentifier, request.SongId, request.spaceToken))
                return StatusCode(429, "Você já sugeriu essa música recentemente.");

            var suggestion = await suggestSongToSpaceUseCase.Execute(request);

            return Ok(suggestion);
        }
        catch (SpaceNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(IEnumerable<GetSuggestionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions([FromQuery] GetSuggestionsRequest request)
    {
        try
        {
            var suggestions = await getSuggestionsUseCase.Execute(request);
            return Ok(suggestions);
        }
        catch (SpaceNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }
}