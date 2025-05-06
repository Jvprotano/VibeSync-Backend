using VibeSync.Domain.Enums;

namespace VibeSync.Application.Extensions;

public static class StripeExtension
{
    public static SubscriptionStatusEnum ConvertStripeSubscriptionStatus(string? stripeStatus)
    {
        return stripeStatus?.ToLower() switch
        {
            "active" => SubscriptionStatusEnum.Active,
            "trialing" => SubscriptionStatusEnum.Active,
            "pastdue" => SubscriptionStatusEnum.PastDue,
            "unpaid" => SubscriptionStatusEnum.PastDue,
            "canceled" => SubscriptionStatusEnum.Canceled,
            "incomplete" => SubscriptionStatusEnum.Pending,
            "incompleteexpired" => SubscriptionStatusEnum.Expired,
            _ => SubscriptionStatusEnum.Unknown
        };
    }
}