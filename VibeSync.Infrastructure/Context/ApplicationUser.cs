using Microsoft.AspNetCore.Identity;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure.Context;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<Space> Spaces { get; set; } = new List<Space>();
    public virtual ICollection<UserPlan> UserPlans { get; set; } = new List<UserPlan>();
}