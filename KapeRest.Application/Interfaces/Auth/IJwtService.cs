using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Jwt;

namespace KapeRest.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        string CreateToken(JwtPayloadDTO payload, IEnumerable<Claim>? additionalClaims = null);
        string RefreshToken();
        string HashToken(string token);
        bool VerifyHashedToken(string hashedToken, string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
