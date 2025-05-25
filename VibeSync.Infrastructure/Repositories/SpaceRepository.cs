using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Models;
using VibeSync.Infrastructure.Context;
using VibeSync.Infrastructure.Helpers;

namespace VibeSync.Infrastructure.Repositories;

public sealed class SpaceRepository(AppDbContext appDbContext) : ISpaceRepository
{
    public async Task<Space?> GetByPublicTokenAsync(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstOrDefaultAsync(c => guid.Equals(c.PublicToken));
    }
    public async Task<Space?> GetByAdminTokenAsync(Guid adminToken)
    {
        return await appDbContext.Spaces
            .FirstOrDefaultAsync(c => adminToken.Equals(c.AdminToken));
    }
    public async Task<IEnumerable<Space>> GetSpacesByUserIdAsync(Guid userId)
    {
        return await appDbContext.Spaces
            .Where(c => c.UserId.Equals(userId))
            .Include(c => c.Suggestions)
            .ToListAsync();
    }
    public async Task<Space> CreateAsync(Space space)
    {
        space.SetQrCode(QrCodeExtension.GenerateQrCode(space.PublicToken.ToString()));

        await appDbContext.Spaces.AddAsync(space);
        await appDbContext.SaveChangesAsync();
        return space;
    }
}