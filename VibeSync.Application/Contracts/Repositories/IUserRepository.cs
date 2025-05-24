using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string email);
    Task<User?> CreateUserAsync(string email, string password, string fullName);
    Task<User?> GetByEmailAsync(string userEmail);
    Task<User?> GetByIdAsync(Guid userId);
}