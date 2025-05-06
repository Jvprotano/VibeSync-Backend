using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Repositories;

public interface IUserPlanRepository
{
    Task<UserPlan?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserPlan?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId);
    Task<UserPlan> AddAsync(UserPlan userPlan, CancellationToken cancellationToken = default);
    Task<UserPlan> UpdateAsync(UserPlan userPlan);
}