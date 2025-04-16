using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class CreateCheckoutSessionUseCase(IStripeService stripeService, IPlanRepository planRepository) : IUseCase<CheckoutRequest, CheckoutResponse>
{
    public async Task<CheckoutResponse> Execute(CheckoutRequest request)
    {
        var plan = await planRepository.GetPlanByIdAsync(request.PlanId);

        if (plan is null || plan is { StripePriceId: null })
            throw new PlanNotFoundException("Invalid plan.");

        var checkoutUrl = await stripeService.CreateCheckoutSessionAsync(plan.StripePriceId!, request.PlanId, request.UserId);

        return new(checkoutUrl);
    }
}