using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class UserPlanRepository(AppDbContext appDbContext) : IUserPlanRepository
{
    private DbSet<UserPlan> DbSet => appDbContext.UserPlans;

    public async Task<UserPlan?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(userPlan => userPlan.UserId == userId)
            .Include(userPlan => userPlan.Plan)
            .OrderByDescending(userPlan => userPlan.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserPlan?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId)
    {
        return await DbSet
            .Include(userPlan => userPlan.Plan)
            .FirstOrDefaultAsync(userPlan => userPlan.StripeSubscriptionId == stripeSubscriptionId);
    }

    public async Task<UserPlan> AddAsync(UserPlan userPlan, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(userPlan, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return userPlan;
    }

    public async Task<UserPlan> UpdateAsync(UserPlan userPlan)
    {
        DbSet.Update(userPlan);
        await appDbContext.SaveChangesAsync();
        return userPlan;
    }
}
