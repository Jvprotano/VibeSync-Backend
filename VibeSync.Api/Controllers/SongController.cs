using Microsoft.AspNetCore.Mvc;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongController(SearchSongUseCase searchSongUseCase, ILogger<SongController> logger) : BaseController(logger)
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<SongResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSong([FromQuery] SearchSongRequest request)
        => await Handle(() => searchSongUseCase.Execute(request));
}
