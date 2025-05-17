using VibeSync.Application.Responses;
using VibeSync.Domain.Domains;

namespace VibeSync.Application.Extensions;

public static class UserExtension
{
    public static UserResponse AsResponseModel(this User user, Plan? plan = null)
        => new UserResponse(
            user.Name,
            user.Email,
            user.Id,
            plan != null ? new PlanResponse(plan.Name, plan.MaxSpaces) : null
            );
}