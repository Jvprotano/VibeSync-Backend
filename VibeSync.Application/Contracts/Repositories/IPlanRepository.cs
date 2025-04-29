using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetPlanByIdAsync(Guid planId);
    Task<Guid> GetFreePlanIdAsync();
}
