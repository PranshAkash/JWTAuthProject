using JWTAuthProject.Models;
using System.Security.Claims;

namespace JWTAuthProject.AppCode.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        RefreshTokenModel GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
