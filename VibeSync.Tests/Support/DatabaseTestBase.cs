using Microsoft.EntityFrameworkCore;
using VibeSync.Infrastructure;

namespace VibeSync.Tests.Support;

public abstract class DatabaseTestBase : IAsyncLifetime
{
    protected readonly AppDbContext _context;

    protected DatabaseTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "VibeSyncTestDB")
            .Options;

        _context = new AppDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }
}

