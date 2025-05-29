using Microsoft.Extensions.Options;
using Moq;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Helpers;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Enums;
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
            var frontendSettings = new FrontendSettings
            {
                BaseUrl = "https://example.com"
            };
            var optionsMock = new Mock<IOptions<FrontendSettings>>();
            optionsMock.Setup(o => o.Value).Returns(frontendSettings);

            var spaceRepository = new SpaceRepository(_context, optionsMock.Object);
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User(Guid.NewGuid(), "TestUser", ""));

            userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User(Guid.NewGuid(), "TestUser", ""));

            var userPlanRepositoryMock = new Mock<IUserPlanRepository>();

            userPlanRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(new UserPlan(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, null, null, null, SubscriptionStatusEnum.Active)
                {
                    Plan = new Plan(Guid.NewGuid(), "Basic Plan", 3, 0)
                });

            var planRepository = new PlanRepository(_context);

            _createSpaceUseCase = new CreateSpaceUseCase(spaceRepository, userRepositoryMock.Object, userPlanRepositoryMock.Object);
            _getSpaceByAdminTokenUseCase = new GetSpaceByAdminTokenUseCase(spaceRepository);
        }

        [Fact]
        public async Task ShouldReturnSpace_WhenSuccessGet()
        {
            const string DEFAULT_SPACE_NAME = "Fake Space";

            // Arrange
            var eventDate = DateTime.UtcNow.AddDays(1);
            var createdSpace = await _createSpaceUseCase.Execute(
                new CreateSpaceRequest(DEFAULT_SPACE_NAME, eventDate, Guid.NewGuid()));

            var expected = new SpaceResponse(
                createdSpace.AdminToken,
                createdSpace.PublicToken,
                DEFAULT_SPACE_NAME,
                "Fake Qr Code",
                eventDate
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
