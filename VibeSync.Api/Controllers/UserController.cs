using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    ILogger<UserController> logger,
    GetUserUseCase getUserUseCase) : BaseController(logger)
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized(new ErrorResponse("User ID is missing.", StatusCodes.Status401Unauthorized));

        return await Handle(() => getUserUseCase.Execute(userId.Value));
    }
}