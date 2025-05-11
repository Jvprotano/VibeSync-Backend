using System.Data;
using VibeSync.Domain.Enums;
using VibeSync.Domain.Models;

namespace VibeSync.Domain.Domains;

public class UserPlan : BaseEntity
{
    public UserPlan(
        Guid userId,
        Guid planId,
        DateTime startDate,
        DateTime? currentPeriodEnd,
        string? stripeCustomerId = null,
        string? stripeSubscriptionId = null,
        SubscriptionStatusEnum status = SubscriptionStatusEnum.Unknown)
    {
        UserId = userId;
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        PlanId = planId;
        StartDate = startDate;
        CurrentPeriodEnd = currentPeriodEnd;
        Status = status;
    }

    public Guid UserId { get; private set; }
    public string? StripeCustomerId { get; private set; }
    public string? StripeSubscriptionId { get; private set; }
    public Plan? Plan { get; set; }
    public Guid PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? CurrentPeriodEnd { get; private set; }
    public DateTime? CancellationDate { get; private set; }
    public SubscriptionStatusEnum Status { get; private set; } = SubscriptionStatusEnum.Unknown;

    public void Renew(DateTime currentPeriodEnd, SubscriptionStatusEnum status)
    {
        CurrentPeriodEnd = currentPeriodEnd;
        Status = status;
    }

    public void Cancel()
    {
        Status = SubscriptionStatusEnum.Canceled;
        CancellationDate = DateTime.UtcNow;
    }

    public void UpdateStatus(SubscriptionStatusEnum status)
    {
        if (status == SubscriptionStatusEnum.Canceled)
            Cancel();

        if (Status == SubscriptionStatusEnum.Canceled)
            return;

        Status = status;
    }

    public bool ReachedMaxSpaces(IEnumerable<Space> userSpaces)
    {
        if (Plan == null)
            return false;

        return userSpaces.Count() >= Plan?.MaxSpaces;
    }
}