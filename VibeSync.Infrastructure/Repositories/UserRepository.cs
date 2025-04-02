using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Infrastructure.Repositories;

public class UserRepository(AppDbContext appDbContext, UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<string?> AddPartialUser(string userEmail)
    {
        var result = await userManager.CreateAsync(new() { Email = userEmail, UserName = userEmail });

        if (!result.Succeeded)
            return null;

        return await GetUserIdByEmail(userEmail);
    }

    public async Task AddPasswordToUser(string userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return;

        await userManager.AddPasswordAsync(user, password);
    }

    public async Task<bool> UserExists(string email)
    {
        return await appDbContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task<string?> GetUserIdByEmail(string userEmail)
    {
        return (await GetUserByEmail(userEmail))?.Id;
    }

    private async Task<ApplicationUser?> GetUserByEmail(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
}