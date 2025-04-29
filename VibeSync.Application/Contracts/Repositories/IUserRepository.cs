using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<User?> AddPartialUser(string userEmail);
    Task<bool> UserExistsAsync(string email);
    Task<User?> AddPasswordToUserAsync(string userId, string password);
    Task<User?> GetByEmailAsync(string userEmail);
    Task<User?> GetByIdAsync(string userId);
}