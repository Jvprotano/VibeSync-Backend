using Microsoft.AspNetCore.Identity;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure.Context;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Space> Spaces { get; set; } = new List<Space>();
}