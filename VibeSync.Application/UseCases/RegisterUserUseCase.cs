using FluentValidation;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.Validators;

namespace VibeSync.Application.UseCases;

public class RegisterUserUseCase(IUserRepository userRepository) : IUseCase<UserRequest, UserResponse>
{
    public async Task<UserResponse> Execute(UserRequest userRequest)
    {
        var user = await userRepository.GetUserByEmailAsync(userRequest.Email) ?? throw new UserNotFoundException();

        if (user.HasPassword)
            throw new UserAlreadyRegisteredException(userRequest.Email);

        await ValidateAsync(userRequest);

        var userUpdated = await userRepository.AddPasswordToUserAsync(user.Id, userRequest.Password) ?? throw new CreateUserException("Error creating user password.");

        return userUpdated.AsResponseModel();
    }

    private async Task ValidateAsync(UserRequest userRequest)
    {
        UserValidator validator = new();
        var validationResult = await validator.ValidateAsync(userRequest);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
}