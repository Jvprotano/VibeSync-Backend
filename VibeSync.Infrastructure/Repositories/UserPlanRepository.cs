using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class UserPlanRepository(AppDbContext appDbContext) : IUserPlanRepository
{
    public async Task<UserPlan> AddAsync(UserPlan userPlan, CancellationToken cancellationToken = default)
    {
        await appDbContext.UserPlans.AddAsync(userPlan, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return userPlan;
    }

    public async Task<UserPlan?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await appDbContext.UserPlans.FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);
    }

    public async Task<UserPlan> UpdateAsync(UserPlan userPlan)
    {
        appDbContext.UserPlans.Update(userPlan);
        await appDbContext.SaveChangesAsync();
        return userPlan;
    }
}
