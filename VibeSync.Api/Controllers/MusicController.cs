using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(SearchSongUseCase searchSongUseCase) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchMusic([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query cannot be empty.");

        var songs = await searchSongUseCase.Execute(query);
        return Ok(songs);
    }
}
