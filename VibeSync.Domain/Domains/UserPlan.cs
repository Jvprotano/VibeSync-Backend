using VibeSync.Domain.Models;

namespace VibeSync.Domain.Domains;

public class UserPlan : BaseEntity
{
    public UserPlan(string userId, string stripeCustomerId, string stripeSubscriptionId, Guid planId, DateTime startDate, DateTime? currentPeriodEnd, bool isActive)
    {
        UserId = userId;
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        PlanId = planId;
        StartDate = startDate;
        CurrentPeriodEnd = currentPeriodEnd;
        IsActive = isActive;
    }

    public string UserId { get; private set; }
    public string StripeCustomerId { get; private set; }
    public string StripeSubscriptionId { get; private set; }
    public Plan? Plan { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? CurrentPeriodEnd { get; private set; }
    public DateTime? CancelAt { get; private set; }
    public bool IsActive { get; private set; }

    public void Renew(string stripeSubscriptionId, DateTime currentPeriodEnd)
    {
        StripeSubscriptionId = stripeSubscriptionId;
        CurrentPeriodEnd = currentPeriodEnd;
        IsActive = true;
    }
    public void Cancel()
    {
        IsActive = false;
        CancelAt = DateTime.UtcNow;
    }
}
