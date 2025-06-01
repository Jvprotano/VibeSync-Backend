using FluentValidation;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.Validators;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Enums;

namespace VibeSync.Application.UseCases;

public class RegisterUserUseCase(
    IUserRepository userRepository,
    IPlanRepository planRepository,
    IUserPlanRepository userPlanRepository) : IUseCase<RegisterRequest, UserResponse>
{
    public async Task<UserResponse> Execute(RegisterRequest userRequest)
    {
        await ValidateAsync(userRequest);

        var user = await userRepository.GetByEmailAsync(userRequest.Email);

        if (user is { ConfirmedEmail: false })
            return user.AsResponseModel();

        if (user is not null)
            throw new UserAlreadyExistsException(userRequest.Email);

        var userCreated = await userRepository.CreateUserAsync(userRequest.Email, userRequest.Password, userRequest.FullName)
            ?? throw new CreateUserException();

        if (await userPlanRepository.GetByUserIdAsync(userCreated.Id) is null)
            await LinkToFreePlan(userCreated.Id);

        return userCreated.AsResponseModel();
    }

    private async Task ValidateAsync(RegisterRequest userRequest)
    {
        RegisterValidator validator = new();
        var validationResult = await validator.ValidateAsync(userRequest);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
    private async Task LinkToFreePlan(Guid userId)
    {
        var freePlanId = await planRepository.GetFreePlanIdAsync();
        var userPlan = new UserPlan(userId, freePlanId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7), status: SubscriptionStatusEnum.Active);
        await userPlanRepository.AddAsync(userPlan);
    }
}