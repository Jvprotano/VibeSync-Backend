using Microsoft.AspNetCore.Identity;
using VibeSync.Domain.Domains;
using VibeSync.Domain.Models;

namespace VibeSync.Infrastructure.Context;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser(string fullName, string email, string userName, bool emailConfirmed = false)
        : base(userName)
    {
        FullName = fullName;
        Email = email;
        UserName = userName;
        EmailConfirmed = emailConfirmed;
    }

    public string FullName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<Space> Spaces { get; set; } = [];
    public virtual ICollection<UserPlan> UserPlans { get; set; } = [];

    public User AsUser()
        => new(Id, FullName, Email ?? UserName!, EmailConfirmed);
}

public class ApplicationRole : IdentityRole<Guid> { }

public class ApplicationUserRole : IdentityUserRole<Guid> { }

public class ApplicationUserClaim : IdentityUserClaim<Guid> { }

public class ApplicationUserLogin : IdentityUserLogin<Guid> { }

public class ApplicationRoleClaim : IdentityRoleClaim<Guid> { }

public class ApplicationUserToken : IdentityUserToken<Guid> { }