using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Models;
using VibeSync.Infrastructure.Context;
using VibeSync.Infrastructure.Helpers;

namespace VibeSync.Infrastructure.Repositories;

public sealed class SpaceRepository(AppDbContext appDbContext) : ISpaceRepository
{
    public async Task<Space> GetSpaceByPublicTokenAsync(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstAsync(c => guid.Equals(c.PublicToken));
    }
    public async Task<Space> GetSpaceByAdminTokenAsync(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstAsync(c => guid.Equals(c.AdminToken));
    }
    public async Task<IEnumerable<Space>> GetSpacesByUserIdAsync(string userId)
    {
        return await appDbContext.Spaces
            .Where(c => c.UserId.Equals(userId))
            .ToListAsync();
    }
    public async Task<Space> CreateSpaceAsync(Space space)
    {
        space.SetQrCode(QrCodeExtension.GenerateQrCode(space.Id.ToString()));

        await appDbContext.Spaces.AddAsync(space);
        await appDbContext.SaveChangesAsync();
        return space;
    }
}