using System.ComponentModel.DataAnnotations.Schema;

namespace VibeSync.Domain.Domains;

[NotMapped]
public class User
{
    public User(string id, string name, string email, string? password = null)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    private string? Password { get; set; } = null;

    public bool HasPassword => !string.IsNullOrWhiteSpace(Password);
}
