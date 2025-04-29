using Moq;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Exceptions;
using VibeSync.Infrastructure.Repositories;
using VibeSync.Tests.Support;

namespace VibeSync.Tests.Application
{
    public class SpaceUseCaseTests : DatabaseTestBase
    {
        private readonly CreateSpaceUseCase _createSpaceUseCase;
        private readonly GetSpaceByAdminTokenUseCase _getSpaceByAdminTokenUseCase;


        public SpaceUseCaseTests()
        {
            var spaceRepository = new SpaceRepository(_context);
            // var userRepository = new UserRepository(_context, null!);
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User(Guid.NewGuid().ToString(), "TestUser", "", ""));

            // var userPlanRepository = new UserPlanRepository(_context);
            var userPlanRepositoryMock = new Mock<IUserPlanRepository>();

            // userPlanRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), default))
            //     .ReturnsAsync(new UserPlan(Guid.NewGuid().ToString(), Guid.NewGuid(), DateTime.Now, null, null, null, true));

            userPlanRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), default))
                .ReturnsAsync(new UserPlan(Guid.NewGuid().ToString(), Guid.NewGuid(), DateTime.Now, null, null, null, true)
                {
                    Plan = new Plan(Guid.NewGuid(), "Basic Plan", 3, 0)
                });

            var planRepository = new PlanRepository(_context);

            _createSpaceUseCase = new CreateSpaceUseCase(spaceRepository, userRepositoryMock.Object, userPlanRepositoryMock.Object, planRepository);
            _getSpaceByAdminTokenUseCase = new GetSpaceByAdminTokenUseCase(spaceRepository);
        }

        [Fact]
        public async Task ShouldReturnSpace_WhenSuccessGet()
        {
            const string DEFAULT_SPACE_NAME = "Fake Space";
            const string DEFAULT_USER_EMAIL = "Test@test.com";

            const string DEFAULT_URL = "https://www.google.com/search?q=";

            // Arrange
            var createdSpace = await _createSpaceUseCase.Execute(
                new CreateSpaceRequest(DEFAULT_SPACE_NAME, DEFAULT_USER_EMAIL, DateTime.Now));

            var expected = new SpaceResponse(
                createdSpace.AdminToken,
                createdSpace.PublicToken,
                DEFAULT_SPACE_NAME,
                $"{DEFAULT_URL}{createdSpace.PublicToken}",
                "Fake Qr Code"
            );

            expected = expected with { QrCode = createdSpace.QrCode };

            // Act    
            var actual = await _getSpaceByAdminTokenUseCase.Execute(createdSpace.AdminToken);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ShouldThrowException_WhenSpaceNotFound()
        {
            // Arrange
            var nonExistentGuid = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<SpaceNotFoundException>(async () =>
                await _getSpaceByAdminTokenUseCase.Execute(nonExistentGuid));
        }
    }
}
