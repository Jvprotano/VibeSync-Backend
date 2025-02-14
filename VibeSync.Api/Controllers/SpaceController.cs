using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Requests;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceController(CreateSpaceUseCase createSpaceUseCase, GetSpaceByIdUseCase getSpaceByIdUseCase, SuggestMusicToSpaceUseCase suggestMusicToSpaceUseCase, GetSuggestionsUseCase getSuggestionsUseCase) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpaceById(Guid id)
    {
        try
        {
            var space = await getSpaceByIdUseCase.Execute(id);
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
        return CreatedAtAction(nameof(GetSpaceById), new { id = space.Id }, space);
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestMusicToSpace([FromBody] SuggestMusicRequest request)
    {
        var suggestion = await suggestMusicToSpaceUseCase.Execute(request);
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