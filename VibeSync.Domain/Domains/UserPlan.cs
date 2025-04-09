using VibeSync.Domain.Models;

namespace VibeSync.Domain.Domains;

public class UserPlan : BaseEntity
{
    public UserPlan(Guid userId, string stripeCustomerId, string stripeSubscriptionId, Guid planId, DateTime startDate, DateTime? currentPeriodEnd, bool isActive)
    {
        UserId = userId;
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        PlanId = planId;
        StartDate = startDate;
        CurrentPeriodEnd = currentPeriodEnd;
        IsActive = isActive;
    }

    public Guid UserId { get; private set; }
    public string StripeCustomerId { get; private set; } // criado ao fazer checkout
    public string StripeSubscriptionId { get; private set; } // salvo ao criar a subscription
    public Plan? Plan { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? CurrentPeriodEnd { get; private set; } // usado para verificar validade da assinatura
    public bool IsActive { get; private set; }

    public void Renew(string stripeSubscriptionId, DateTime currentPeriodEnd)
    {
        StripeSubscriptionId = stripeSubscriptionId;
        CurrentPeriodEnd = currentPeriodEnd;
    }
    public void Cancel()
    {
        IsActive = false;
    }
    public void Reactivate()
    {
        IsActive = true;
    }
}
