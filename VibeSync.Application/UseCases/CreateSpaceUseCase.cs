using FluentValidation;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.Validators;
using VibeSync.Domain.Domains;

namespace VibeSync.Application.UseCases;

public class CreateSpaceUseCase(
    ISpaceRepository spaceRepository,
    IUserRepository userRepository,
    IUserPlanRepository userPlanRepository,
    IPlanRepository planRepository) : IUseCase<CreateSpaceRequest, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(CreateSpaceRequest request)
    {
        await Validate(request);

        var userId = await GetOrCreateUser(request.UserEmail!);

        await CheckUserSpaceLimit(userId);

        var response = await spaceRepository.CreateAsync(request.AsDomain(userId));

        return response.AsResponseModel();
    }

    private async Task Validate(CreateSpaceRequest request)
    {
        SpaceValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private async Task CheckUserSpaceLimit(Guid userId)
    {
        var userPlan = await userPlanRepository.GetByUserIdAsync(userId);

        if (userPlan is null)
            throw new CreateUserException("User plan not found.");

        var userSpaces = await spaceRepository.GetSpacesByUserIdAsync(userId);

        if (userPlan.ReachedMaxSpaces(userSpaces))
            throw new SpacesPerUserLimitException("User has reached the maximum number of spaces allowed.");
    }

    private async Task<Guid> GetOrCreateUser(string userEmail)
    {
        var user = await userRepository.GetByEmailAsync(userEmail)
            ?? await CreatePartialUser(userEmail);

        if (user is null)
            throw new CreateUserException("Error creating partial user.");

        return user.Id;
    }

    private async Task<User> CreatePartialUser(string userEmail)
    {
        var user = await userRepository.AddPartialUser(userEmail);

        if (user is null)
            throw new CreateUserException("Error creating partial user.");

        await LinkToFreePlan(user.Id);

        return user;
    }

    private async Task LinkToFreePlan(Guid userId)
    {
        var freePlanId = await planRepository.GetFreePlanIdAsync();
        var userPlan = new UserPlan(userId, freePlanId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        await userPlanRepository.AddAsync(userPlan);
    }
}