using Microsoft.AspNetCore.Mvc;
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
        catch (BadRequestException ex)
        {
            logger.LogError(ex, ex.Message);
            return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest, ex.InnerException?.Message, ex.GetType().Name));
        }
    }
}