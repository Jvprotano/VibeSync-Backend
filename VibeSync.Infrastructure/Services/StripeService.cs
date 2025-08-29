using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Helpers;

namespace VibeSync.Infrastructure.Services;

public class StripeService : IStripeService
{
    private readonly StripeSettings _settings;
    private readonly FrontendSettings _frontendSettings;
    private readonly ILogger<StripeService> _logger;

    public StripeService(
        IOptions<StripeSettings> settings,
        IOptions<FrontendSettings> frontendSettings,
        ILogger<StripeService> logger)
    {
        _frontendSettings = frontendSettings.Value;
        _logger = logger;
        if (settings == null || settings.Value == null)
            throw new ArgumentNullException(nameof(settings), "Stripe settings cannot be null.");

        if (_frontendSettings == null || string.IsNullOrWhiteSpace(_frontendSettings.BaseUrl))
            throw new ArgumentNullException(nameof(frontendSettings), "Frontend settings cannot be null or have an empty BaseUrl.");

        _settings = settings.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(string stripePriceId, Guid planId, Guid userId)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = $"{_frontendSettings.BaseUrl}/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_frontendSettings.BaseUrl}/pricing",
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "planId", $"{planId}"}
            }
        };

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(options);
        return session.Url;
    }

    public async Task CancelSubscriptionAsync(string stripeSubscriptionId)
    {
        var subscriptionService = new SubscriptionService();
        try
        {
            await subscriptionService.CancelAsync(stripeSubscriptionId, null);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error cancelling subscription in Stripe for Subscription ID: {SubscriptionId}", stripeSubscriptionId);
            throw new InvalidOperationException($"Stripe error: {ex.Message}");
        }
    }
}
