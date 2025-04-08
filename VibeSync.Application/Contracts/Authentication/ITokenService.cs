using VibeSync.Domain.Domains;

namespace VibeSync.Application.Contracts.Authentication;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}