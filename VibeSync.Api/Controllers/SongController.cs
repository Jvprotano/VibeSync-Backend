using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongController(SearchSongUseCase searchSongUseCase) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<SongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSong([FromQuery] SearchSongRequest request)
    {
        var songs = await searchSongUseCase.Execute(request);
        return Ok(songs);
    }
}
