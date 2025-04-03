using VibeSync.Application.Responses;
using VibeSync.Domain.Domains;

namespace VibeSync.Application.Extensions;

public static class UserExtension
{
    public static UserResponse AsResponseModel(this User user)
        => new UserResponse(user.Name, user.Email, Guid.Parse(user.Id));
}