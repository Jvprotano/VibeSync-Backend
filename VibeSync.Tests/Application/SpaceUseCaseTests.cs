using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
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
            var userRepository = new UserRepository(_context, null!);

            _createSpaceUseCase = new CreateSpaceUseCase(spaceRepository, userRepository);
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

            var publicToken = createdSpace.PublicToken;
            var adminToken = createdSpace.AdminToken;

            var expected = new SpaceResponse(
                publicToken,
                adminToken,
                DEFAULT_SPACE_NAME,
                $"{DEFAULT_URL}{publicToken}",
                "Fake Qr Code"
            );

            expected = expected with { QrCode = createdSpace.QrCode };

            // Act    
            var actual = await _getSpaceByAdminTokenUseCase.Execute(publicToken);

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
