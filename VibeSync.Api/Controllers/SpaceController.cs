using Microsoft.AspNetCore.Mvc;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Exceptions;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpaceController(
    ILogger<SpaceController> logger,
    CreateSpaceUseCase createSpaceUseCase,
    GetSpaceByPublicTokenUseCase getSpaceByPublicTokenUseCase,
    GetSpaceByAdminTokenUseCase getSpaceByAdminTokenUseCase) : BaseController(logger)
{
    [HttpGet("{token}")]
    public async Task<IActionResult> GetSpaceByPublicToken(Guid token)
        => await Handle(() => getSpaceByPublicTokenUseCase.Execute(token));

    [HttpGet("admin/{adminToken}")]
    [ProducesResponseType(typeof(SpaceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpaceByAdminToken(Guid adminToken)
        => await Handle(() => getSpaceByAdminTokenUseCase.Execute(adminToken));

    [HttpPost]
    [ProducesResponseType(typeof(SpaceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceRequest request)
    {
        try
        {
            return await Handle(() => createSpaceUseCase.Execute(request));
        }
        catch (SpacesPerUserLimitException exception)
        {
            return StatusCode(429, new ErrorResponse(exception.Message, StatusCodes.Status429TooManyRequests));
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new ErrorResponse(exception.Message, StatusCodes.Status401Unauthorized));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ErrorResponse(exception.Message, StatusCodes.Status400BadRequest));
        }
    }
}