using Microsoft.EntityFrameworkCore;
using VibeSync.Application.Contracts.Repositories;
using VibeSync.Domain.Models;
using VibeSync.Infrastructure.Extensions;

namespace VibeSync.Infrastructure.Repositories;

public sealed class SpaceRepository(AppDbContext appDbContext) : ISpaceRepository
{
    public async Task<Space> GetSpaceById(Guid guid)
    {
        return await appDbContext.Spaces
            .FirstAsync(c => c.Id == guid);
    }
    public async Task<Space> CreateSpace(Space space)
    {
        space.SetQrCode(QrCodeExtension.GenerateQrCode(space.Id.ToString()));
        
        await appDbContext.Spaces.AddAsync(space);
        await appDbContext.SaveChangesAsync();
        return space;
    }
}
