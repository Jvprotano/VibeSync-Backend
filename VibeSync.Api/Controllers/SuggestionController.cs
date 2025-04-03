using Microsoft.AspNetCore.Mvc;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Infrastructure.Helpers;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuggestionController(
    ILogger<SuggestionController> logger,
    SuggestSongToSpaceUseCase suggestSongToSpaceUseCase,
    GetSuggestionsUseCase getSuggestionsUseCase) : BaseController(logger)
{
    [HttpPost("suggest")]
    [ProducesResponseType(typeof(SuggestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Suggest([FromBody] SuggestSongRequest payload)
    {
        string userIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString() ?? Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown";

        if (!SuggestionRateLimiter.CanSuggest(userIdentifier, payload.SongId, payload.spaceToken))
            return StatusCode(429, new ErrorResponse("Too many suggestions of the same music. Allowed only once every 5 minutes.", StatusCodes.Status429TooManyRequests));

        return await Handle(() => suggestSongToSpaceUseCase.Execute(payload));
    }

    [HttpGet("suggestions")]
    [ProducesResponseType(typeof(IEnumerable<GetSuggestionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions([FromQuery] GetSuggestionsRequest payload)
        => await Handle(() => getSuggestionsUseCase.Execute(payload));
}