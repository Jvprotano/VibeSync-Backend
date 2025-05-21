using System.ComponentModel.DataAnnotations.Schema;

namespace VibeSync.Domain.Domains;

[NotMapped]
public class User
{
    public User(Guid id, string fullName, string email, string? password = null, bool confirmedEmail = false)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Password = password;
        ConfirmedEmail = confirmedEmail;
    }

    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public bool ConfirmedEmail { get; private set; }
    private string? Password { get; set; } = null;

    public bool HasPassword => !string.IsNullOrWhiteSpace(Password);
}
