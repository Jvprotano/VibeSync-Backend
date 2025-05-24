using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Domains;
using VibeSync.Tests.Support;
using VibeSync.Tests.Support.Factories;

namespace VibeSync.Tests.Application;

public class RegisterUseCaseTests : DatabaseTestBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    public RegisterUseCaseTests()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var userPlanRepositoryMock = new Mock<IUserPlanRepository>();
        _registerUserUseCase = new(userRepositoryMock.Object, planRepositoryMock.Object, userPlanRepositoryMock.Object);
    }
    [Fact]
    public async Task CreateUser_ValidInput_ReturnsUser()
    {
        // Arrange
        var request = UserFactory.CreateValidRegisterRequest();

        var userRepositoryMock = new Mock<IUserRepository>();
        var planRepositoryMock = new Mock<IPlanRepository>();
        var userPlanRepositoryMock = new Mock<IUserPlanRepository>();

        var useCase = new RegisterUserUseCase(userRepositoryMock.Object, planRepositoryMock.Object, userPlanRepositoryMock.Object);

        userRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new User(Guid.NewGuid(), request.FullName, request.Email));

        userRepositoryMock.Setup(x => x.UserExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var expected = new UserResponse(request.FullName, request.Email, Guid.NewGuid());

        // Act
        var actual = await useCase.Execute(request);

        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task If_FullName_Is_Empty_Throw_ValidationException()
    {
        // Arrange
        var request = UserFactory.CreateValidRegisterRequest() with { FullName = string.Empty };
        var expected = new ValidationFailure("FullName", "Full name is required.");

        // Act 
        var action = async () => await _registerUserUseCase.Execute(request);
        var actual = (await action.Should().ThrowAsync<ValidationException>()).Which.Message;

        // Assert
        actual.Should().Contain(expected.ToString());
    }

    [Fact]
    public async Task If_Email_Is_Empty_Throw_ValidationException()
    {
        // Arrange
        var request = UserFactory.CreateValidRegisterRequest() with { Email = string.Empty };

        // Act 
        var action = async () => await _registerUserUseCase.Execute(request);
        var expected = new ValidationFailure("Email", "Email is required.");

        // Assert
        var actual = (await action.Should().ThrowAsync<ValidationException>()).Which.Message;

        actual.Should().Contain(expected.ToString());
    }

    [Fact]
    public async Task If_Password_Is_Empty_Throw_ValidationException()
    {
        // Arrange
        var request = UserFactory.CreateValidRegisterRequest() with { Password = string.Empty };
        var expected = new ValidationFailure("Password", "Password is required.");

        // Act 
        var action = async () => await _registerUserUseCase.Execute(request);
        var actual = (await action.Should().ThrowAsync<ValidationException>()).Which.Message;

        // Assert
        actual.Should().Contain(expected.ToString());
    }
}