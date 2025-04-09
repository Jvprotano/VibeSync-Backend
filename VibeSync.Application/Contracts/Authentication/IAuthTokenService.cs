using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Authentication;

public interface IAuthTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}