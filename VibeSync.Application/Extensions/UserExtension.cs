using VibeSync.Application.Responses;
using VibeSync.Domain.Domains;

namespace VibeSync.Application.Extensions;

public static class UserExtension
{
    public static UserResponse AsResponseModel(this User user, string? planName = null)
        => new UserResponse(
            user.Name,
            user.Email,
            user.Id,
            planName);
}