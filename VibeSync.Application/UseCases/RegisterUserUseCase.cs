using FluentValidation;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Contracts.UseCases;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Extensions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.Validators;

namespace VibeSync.Application.UseCases;

public class RegisterUserUseCase(IUserRepository userRepository) : IUseCase<RegisterRequest, UserResponse>
{
    public async Task<UserResponse> Execute(RegisterRequest userRequest)
    {
        await ValidateAsync(userRequest);

        if (await userRepository.UserExistsAsync(userRequest.Email))
            throw new UserAlreadyRegisteredException(userRequest.Email);

        var userCreated = await userRepository.CreateUserAsync(userRequest.Email, userRequest.Password, userRequest.FullName)
            ?? throw new CreateUserException("Error creating user password.");

        return userCreated.AsResponseModel();
    }

    private async Task ValidateAsync(RegisterRequest userRequest)
    {
        RegisterValidator validator = new();
        var validationResult = await validator.ValidateAsync(userRequest);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
}