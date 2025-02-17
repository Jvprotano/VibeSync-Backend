using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Requests;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceController(
    CreateSpaceUseCase createSpaceUseCase,
    GetSpaceByPublicTokenUseCase getSpaceByPublicTokenUseCase,
    SuggestSongToSpaceUseCase suggestSongToSpaceUseCase,
    GetSuggestionsUseCase getSuggestionsUseCase,
    GetSpaceByAdminTokenUseCase getSpaceByAdminTokenUseCase) : ControllerBase
{
    [HttpGet("{token}")]
    public async Task<IActionResult> GetSpaceByPublicToken(Guid token)
    {
        try
        {
            var space = await getSpaceByPublicTokenUseCase.Execute(token);
            return Ok(space);
        }
        catch (SpaceNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("admin/{adminToken}")]
    public async Task<IActionResult> GetSpaceByAdminToken(Guid adminToken)
    {
        try
        {
            var space = await getSpaceByAdminTokenUseCase.Execute(adminToken);
            return Ok(space);
        }
        catch (SpaceNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceRequest request)
    {
        var space = await createSpaceUseCase.Execute(request);
        return CreatedAtAction(nameof(GetSpaceByPublicToken), new { AdminToken = space.AdminToken }, space);
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestSongToSpace([FromBody] SuggestSongRequest request)
    {
        var suggestion = await suggestSongToSpaceUseCase.Execute(request);
        return Ok(suggestion);
    }

    [HttpGet("suggestions")]
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