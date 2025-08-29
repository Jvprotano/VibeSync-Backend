namespace VibeSync.Application.Contracts.Services;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(string stripePriceId, Guid planId, Guid userId);
    Task CancelSubscriptionAsync(string stripeSubscriptionId);
}
