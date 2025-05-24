using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Tests.Domain;

public class ApplicationUserTests
{
    [Fact]
    public void AsUser_Should_Return_User()
    {
        //Arrange
        var appUser = new ApplicationUser
        (
            "John Doe",
            "email@test.com",
            "email@test.com",
            true
        );

        //Act
        var user = appUser.AsUser();

        //Assert
        Assert.IsType<User>(user);

        Assert.Equal(appUser.Id, user.Id);
        Assert.Equal(appUser.FullName, user.FullName);
        Assert.Equal(appUser.Email, user.Email);
        Assert.Equal(appUser.EmailConfirmed, user.ConfirmedEmail);
    }
}