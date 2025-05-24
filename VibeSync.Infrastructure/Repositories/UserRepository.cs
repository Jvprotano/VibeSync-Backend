using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Application.Exceptions;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class UserRepository(
    AppDbContext appDbContext,
    UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<bool> UserExistsAsync(string email)
        => await appDbContext.Users.AnyAsync(user => user.Email == email);

    public async Task<User?> GetByEmailAsync(string userEmail)
    {
        var appUser = await GetApplicationUserByEmailAsync(userEmail);

        if (appUser == null)
            return null;

        return appUser.AsUser();
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        var appUser = await userManager.FindByIdAsync(userId.ToString());

        if (appUser == null)
            return null;

        return appUser.AsUser();
    }

    public async Task<User?> CreateUserAsync(string email, string password, string fullName)
    {
        var appUser = new ApplicationUser(fullName, email, email);

        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
            return null;
        }

        var user = await GetApplicationUserByEmailAsync(email) ?? throw new UserNotFoundException();

        return user.AsUser();
    }

    private async Task<ApplicationUser?> GetApplicationUserByEmailAsync(string email)
        => await userManager.FindByEmailAsync(email);
}