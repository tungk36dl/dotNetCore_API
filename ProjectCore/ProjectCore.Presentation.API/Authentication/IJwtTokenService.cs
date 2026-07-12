using System.Security.Claims;

namespace ProjectCore.Presentation.API.Authentication;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string userName, string email, IReadOnlyList<string> permissions);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateExpiredToken(string token);
}
