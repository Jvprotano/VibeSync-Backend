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

        var userId = await userRepository.GetUserIdByEmail(request.UserEmail) ?? await userRepository.AddPartialUser(request.UserEmail);

        if (string.IsNullOrEmpty(userId))
            throw new CreateUserException();

        var response = await spaceRepository.CreateSpace(request.AsModel(userId));

        return response.AsDomain();
    }

    private async Task Validate(CreateSpaceRequest request)
    {
        SpaceValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
}