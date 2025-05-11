using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>(options)
{
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Suggestion> Suggestions { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<Plan> Plans { get; set; }

    static AppDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserPlan>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.UserPlans)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Space>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Spaces)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Plan>().HasData(
            new Plan(new Guid("0b4f8c3d-a685-4d45-9c0e-3f85bc56ec15"), "Free", 1, 0),
            new Plan(new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"), "Basic", 5, 29.99m, "price_1RBlW7QTScSq3LFhzyNd64Ky"),
            new Plan(new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"), "Professional", 20, 49.99m, "price_1RBnZrQTScSq3LFhSrkv57nG"),
            new Plan(new Guid("692fa6dd-6ff1-4227-a49d-cff32643dcae"), "Premium", int.MaxValue, 99.99m, "price_1RBnZrQTScSq3LFh5imaf4Jk")
            );
    }
}