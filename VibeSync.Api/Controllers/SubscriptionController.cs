using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.Services;
using VibeSync.Domain.Enums;

namespace VibeSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : BaseController
{
    private readonly IUserPlanRepository _userPlanRepository;
    private readonly IStripeService _stripeService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        IUserPlanRepository userPlanRepository,
        IStripeService stripeService)
        : base(logger)
    {
        _logger = logger;
        _userPlanRepository = userPlanRepository;
        _stripeService = stripeService;
    }

    [HttpPost("cancel")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSubscription()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new { message = "User ID not found in token." });

        var userPlan = await _userPlanRepository.GetByUserIdAsync(userId.Value);
        if (userPlan == null || string.IsNullOrEmpty(userPlan.StripeSubscriptionId))
            return BadRequest(new { message = "Active subscription not found." });

        try
        {
            await _stripeService.CancelSubscriptionAsync(userPlan.StripeSubscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription in Stripe. Falling back to database value.");
        }

        userPlan.UpdateStatus(SubscriptionStatusEnum.Canceled);
        await _userPlanRepository.UpdateAsync(userPlan);

        return Ok(new { userPlan.CurrentPeriodEnd });
    }
}
