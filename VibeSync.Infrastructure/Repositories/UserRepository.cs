using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Domains;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class UserRepository(AppDbContext appDbContext, UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<User?> AddPartialUser(string userEmail)
    {
        var result = await userManager.CreateAsync(new() { Email = userEmail, UserName = userEmail });

        if (!result.Succeeded)
            return null;

        return await GetUserByEmailAsync(userEmail);
    }

    public async Task<User?> AddPasswordToUserAsync(string userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var updateResult = await userManager.AddPasswordAsync(user, password);

        if (!updateResult.Succeeded)
            return null;

        return new User(user.Id, user.UserName!, user.Email!, password);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await appDbContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task<User?> GetUserByEmailAsync(string userEmail)
    {
        var appUser = await GetApplicationUserByEmailAsync(userEmail);

        if (appUser == null)
            return null;

        return new User(appUser.Id, appUser.UserName!, appUser.Email!, appUser.PasswordHash);
    }

    private async Task<ApplicationUser?> GetApplicationUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
}