using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FluentValidation;
using VibeSync.Application.Exceptions.Base;
using VibeSync.Application.Responses;

namespace VibeSync.Api.Controllers.Base;

public abstract class BaseController(ILogger logger) : ControllerBase
{
    protected async Task<IActionResult> Handle<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await action();
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, ex.Message);
            return NotFound(new ErrorResponse(ex.Message, StatusCodes.Status404NotFound, ex.InnerException?.Message, ex.GetType().Name));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, ex.Message);
            return Unauthorized(new ErrorResponse(ex.Message, StatusCodes.Status401Unauthorized, ex.InnerException?.Message, ex.GetType().Name));
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, ex.Message);
            return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest, ex.InnerException?.Message, ex.GetType().Name));
        }
        catch (BadRequestException ex)
        {
            logger.LogError(ex, ex.Message);
            return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest, ex.InnerException?.Message, ex.GetType().Name));
        }
    }

    protected Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return null;

        return new Guid(userId);
    }

    protected string? GetUserEmail()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userEmail))
            return null;

        return userEmail;
    }
}