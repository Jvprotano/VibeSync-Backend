using VibeSync.Application.Contracts.UseCases;
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
        private readonly ISpaceUseCase _useCase;

        public SpaceUseCaseTests()
        {
            var spaceRepository = new SpaceRepository(_context);
            _useCase = new SpaceUseCase(spaceRepository);
        }

        [Fact]
        public async Task ShouldReturnSpace_WhenSuccessGet()
        {
            const string DEFAULT_SPACE_NAME = "Fake Space";
            const string DEFAULT_URL = "https://www.google.com/search?q=";

            // Arrange
            var createdSpace = await _useCase.CreateSpace(
                new CreateSpaceRequest(DEFAULT_SPACE_NAME, DateTime.Now));

            var guid = createdSpace.Id;

            var expected = new SpaceResponse(
                guid,
                DEFAULT_SPACE_NAME,
                $"{DEFAULT_URL}{guid}",
                "Fake Qr Code"
            );

            expected = expected with { QrCode = createdSpace.QrCode };

            // Act    
            var actual = await _useCase.GetSpaceById(guid);

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
                await _useCase.GetSpaceById(nonExistentGuid));
        }
    }
}
