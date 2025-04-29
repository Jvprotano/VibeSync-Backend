using VibeSync.Domain.Domains;

namespace VibeSync.Tests.Domain;

public sealed class UserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void HasPassword_ShouldReturnFalse_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        var user = new User(Guid.NewGuid().ToString(), "", "", password);

        // Act
        var result = user.HasPassword;

        // Assert
        Assert.False(result);
    }
}
