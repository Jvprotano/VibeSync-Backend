using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(
    ILogger<UserController> logger,
    RegisterUserUseCase registerUserUseCase,
    GetUserUseCase getUserUseCase) : BaseController(logger)
{
    [HttpPost("complete-registration")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteRegistration([FromBody] UserRequest payload)
    {
        try
        {
            return await Handle(() => registerUserUseCase.Execute(payload));
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Validation error occurred while registering user.");
            return BadRequest(new ErrorResponse("Validation error", StatusCodes.Status400BadRequest, ex.ToString()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("An unexpected error occurred."));
        }
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();

        if (userId is null)
            return Unauthorized(new ErrorResponse("User ID is missing.", StatusCodes.Status401Unauthorized));

        return await Handle(() => getUserUseCase.Execute(userId));
    }
}