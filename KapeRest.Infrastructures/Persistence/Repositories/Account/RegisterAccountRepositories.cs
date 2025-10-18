using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.Interfaces.Account;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using KapeRest.Application.DTOs.Account;
using Microsoft.Extensions.Configuration;
using KapeRest.Application.Interfaces.Jwt;
using KapeRest.Application.DTOs.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace KapeRest.Infrastructures.Persistence.Repositories.Account
{
    public class RegisterAccountRepositories : IAccounts
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterAccountRepositories(
                UserManager<Users> userManager,
                SignInManager<Users> signInManager,
                ApplicationDbContext context,
                RoleManager<IdentityRole> roleManager,
                IConfiguration config,
                IJwtService jwtService,
                IHttpContextAccessor httpContextAccessor
                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _config = config;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> RegisterAccount(RegisterAccountDTO register)
        {
            var users = new Users
            {
                FirstName = register.FirstName,
                MiddleName = register.MiddleName,
                LastName = register.LastName,
                UserName = register.Email,
                Email = register.Email,
            };
            var registerUser = await _userManager.CreateAsync(users, register.Password);
            if (registerUser.Succeeded)
            {
                if(register.Roles.Equals("Admin",StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Cannot assign Admin role during registration.");

                await _userManager.AddToRoleAsync(users, register.Roles);
                return true;
            }
            return false;
        }

        public async Task<CreateJwtTokenDTO> Login(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null) return null;

            var isLogin = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!isLogin) return null;

            var getUserRoles = await _userManager.GetRolesAsync(user);

            var payload = new JwtPayloadDTO
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                roles = getUserRoles
            };

            var token = _jwtService.CreateToken(payload);
            var refreshToken = _jwtService.RefreshToken();

            var tokenExpiry = int.Parse(_config["Jwt:TokenDurationInMinutes"] ?? "1");
            user.RefreshTokenHash = _jwtService.HashToken(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(tokenExpiry);

            await _userManager.UpdateAsync(user);   

            return new CreateJwtTokenDTO
            {
                token =  token,
                refreshToken = refreshToken
            };

        }

        public async Task Logout(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return;

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task<JwtRefreshResponseDTO> RefreshToken(JwtRefreshRequestDTO requestDTO)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(requestDTO.Token);
            if (principal is null)
                return null;

            var username = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? principal.FindFirst("name")?.Value;


            if (username is null)
                return null;

            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
                return null;

            if(!user.RefreshTokenExpiryTime.HasValue || user.RefreshTokenExpiryTime.Value <= DateTime.UtcNow)
                return null;

            bool isValidRefreshToken = _jwtService.VerifyHashedToken(user.RefreshTokenHash ?? "", requestDTO.RefreshToken);
            if (!isValidRefreshToken)
                return null;

            var trackUser = await _userManager.FindByIdAsync(user.Id);
            trackUser.RefreshTokenHash = null;
            trackUser.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(trackUser);

            var roles = await _userManager.GetRolesAsync(user);
            var newToken = _jwtService.CreateToken(new JwtPayloadDTO
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                roles = roles
            });

            return new JwtRefreshResponseDTO
            {
                Token = newToken,
                RefreshToken = requestDTO.RefreshToken
            };
        }



    }
}
