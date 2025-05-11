using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Responses;

namespace VibeSync.Application.UseCases;

public class GetUserUseCase(IUserRepository userRepository, IUserPlanRepository userPlanRepository) : IUseCase<Guid, UserResponse>
{
    public async Task<UserResponse> Execute(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new UserNotFoundException();

        var userPlan = await userPlanRepository.GetByUserIdAsync(user.Id);

        return user.AsResponseModel(userPlan?.Plan?.Name);
    }
}