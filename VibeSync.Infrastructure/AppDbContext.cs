using Microsoft.EntityFrameworkCore;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Space> Spaces { get; set; }
}
