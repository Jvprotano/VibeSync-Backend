using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Models;
using VibeSync.Infrastructure.Helpers;

namespace VibeSync.Infrastructure.Repositories;

public sealed class SpaceRepository(AppDbContext appDbContext) : ISpaceRepository
{
    public async Task<Space> GetSpaceByPublicToken(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstAsync(c => guid.Equals(c.PublicToken));
    }
    public async Task<Space> GetSpaceByAdminToken(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstAsync(c => guid.Equals(c.AdminToken));
    }
    public async Task<Space> CreateSpace(Space space)
    {
        space.SetQrCode(QrCodeExtension.GenerateQrCode(space.Id.ToString()));

        await appDbContext.Spaces.AddAsync(space);
        await appDbContext.SaveChangesAsync();
        return space;
    }
}