using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class PlanRepository(AppDbContext appDbContext) : IPlanRepository
{
    public async Task<Guid> GetFreePlanIdAsync() => await appDbContext.Plans
        .AsNoTracking()
        .Where(x => x.Name == "Free")
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    public async Task<Plan?> GetPlanByIdAsync(Guid planId)
    {
        return await appDbContext.Plans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == planId);
    }
}
