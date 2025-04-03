using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ILogger<UserController> logger, RegisterUserUseCase registerUserUseCase) : BaseController(logger)
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
}
