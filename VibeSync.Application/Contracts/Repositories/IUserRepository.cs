namespace VibeSync.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<string?> AddPartialUser(string userEmail);
    Task<bool> UserExists(string email);
    Task AddPasswordToUser(string userId, string password);
    Task<string?> GetUserIdByEmail(string userEmail);
}