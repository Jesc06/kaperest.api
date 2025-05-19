using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using KapeRest.Application.DTOs.Jwt;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;
using System.Net.Http.Headers;
using KapeRest.Application.Interfaces.Auth;

namespace KapeRest.Infrastructure.Services.JwtService
{
    public class GenerateTokenService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly Byte[] _key;
        private readonly string Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        private readonly string Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        private readonly string Key = Environment.GetEnvironmentVariable("JWT_KEY")!;
        private readonly string TokenDurationInMinutes = Environment.GetEnvironmentVariable("JWT_TOKEN_DURATION_MINUTES")!;
        
        public GenerateTokenService(IConfiguration config)
        {
            _config = config;
            _key = Encoding.UTF8.GetBytes(Key);
        }

        public string CreateToken(JwtPayloadDTO payload, IEnumerable<Claim>? additionalClaim = null)
        {
            var expiry = double.Parse(TokenDurationInMinutes ?? "1");
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, payload.username ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", payload.id.ToString()),
                new Claim(ClaimTypes.Email, payload.email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, payload.id.ToString())
            };
            #region--Added via modified features not totally needed in generating token--
            var cashierIdValue = !string.IsNullOrEmpty(payload.cashierId)
            ? payload.cashierId
            : payload.id.ToString();//kapag walang cashierID sa payload na naka assign automatic yung Id ng authorized user yung magiging default cashierID

            claims.Add(new Claim("cashierId", cashierIdValue));

            if (payload.branchId != null)//ito ay option nilagay ko lang for features na gusto ko pero in by default alisin na ito
                claims.Add(new Claim("branchId", payload.branchId.ToString()!));
            #endregion
            if (payload.roles != null)
                claims.AddRange(payload.roles.Select(r => new Claim(ClaimTypes.Role, r)));
            if (additionalClaim != null)
                claims.AddRange(additionalClaim);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        public bool VerifyHashedToken(string hashedToken, string token)
        {
            return hashedToken == HashToken(token);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateLifetime = false // allow expired token to get claims
            };
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, validationParams, out var securityToken);
                if (securityToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;
                return principal;
            }
            catch{return null;}
        }
        
    }
}
