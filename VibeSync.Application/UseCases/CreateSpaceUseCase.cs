using FluentValidation;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.Validators;

namespace VibeSync.Application.UseCases;

public class CreateSpaceUseCase(ISpaceRepository spaceRepository, IUserRepository userRepository) : IUseCase<CreateSpaceRequest, SpaceResponse>
{
    public async Task<SpaceResponse> Execute(CreateSpaceRequest request)
    {
        await Validate(request);

        var userId = await GetOrCreateUser(request.UserEmail);

        await CheckUserSpaceLimit(userId);

        var response = await spaceRepository.CreateSpaceAsync(request.AsDomain(userId));

        return response.AsResponseModel();
    }

    private async Task Validate(CreateSpaceRequest request)
    {
        SpaceValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private async Task<string> GetOrCreateUser(string userEmail)
    {
        var user = await userRepository.GetUserByEmailAsync(userEmail)
            ?? await userRepository.AddPartialUser(userEmail);

        if (string.IsNullOrEmpty(user?.Id))
            throw new CreateUserException("Error creating partial user.");

        return user.Id;
    }

    private async Task CheckUserSpaceLimit(string userId)
    {
        if ((await spaceRepository.GetSpacesByUserIdAsync(userId)).Any())
            throw new SpacesPerUserLimitException("User already has a space.");
    }
}