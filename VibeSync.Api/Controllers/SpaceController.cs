using Microsoft.AspNetCore.Mvc;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceController(
    CreateSpaceUseCase createSpaceUseCase,
    GetSpaceByPublicTokenUseCase getSpaceByPublicTokenUseCase,
    GetSpaceByAdminTokenUseCase getSpaceByAdminTokenUseCase) : ControllerBase
{
    [HttpGet("{token}")]
    [ProducesResponseType(typeof(GetPublicSpaceResponse), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(SpaceResponse), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(SpaceResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceRequest request)
    {
        var space = await createSpaceUseCase.Execute(request);
        return CreatedAtAction(nameof(GetSpaceByPublicToken), new { space.AdminToken }, space);
    }
}