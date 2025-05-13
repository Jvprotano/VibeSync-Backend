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
    IUserPlanRepository userPlanRepository)
    : IUseCase<CreateSpaceRequest, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(CreateSpaceRequest request)
    {
        await Validate(request);

        var user = await userRepository.GetByIdAsync(request.UserId!.Value) ?? throw new UserNotFoundException();

        await CheckUserSpaceLimit(user.Id);

        var response = await spaceRepository.CreateAsync(request.AsDomain(user.Id));

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
}