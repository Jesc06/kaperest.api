using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.Interfaces.Auth;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using KapeRest.Application.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using KapeRest.Application.DTOs.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Auth
{
    public class RegisterAccountRepositories : IAccounts
    {
        private readonly UserManager<UsersIdentity> _userManager;
        private readonly SignInManager<UsersIdentity> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IJwtService _jwtService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RegisterAccountRepositories(
                UserManager<UsersIdentity> userManager,
                SignInManager<UsersIdentity> signInManager,
                ApplicationDbContext context,
                RoleManager<IdentityRole> roleManager,
                IConfiguration config,
                IJwtService jwtService
                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _config = config;
            _jwtService = jwtService;
        }


        public async Task<int> GetTotalUsersAsync()
        {
            return await _userManager.Users.CountAsync();
        }


        public async Task<string> RegisterAccount(RegisterAccountDTO register)
        {
            var users = new UsersIdentity
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
                    return "Cannot assign Admin role during registration.";

                await _userManager.AddToRoleAsync(users, register.Roles);

                _context.AuditLog.Add(new AuditLogEntities
                {
                    Username = register.Email,
                    Role = register.Roles,
                    Action = "Register",
                    Description = $"User {register.Email} registered with role {register.Roles}",
                    Date = DateTime.Now
                });
                await _context.SaveChangesAsync();

                return "Successfully registered account!";
            }
            return "Failed to registered account";
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
                username = user.UserName!,
                email = user.Email!,
                roles = getUserRoles,
                cashierId = user.CashierId,
                branchId = user.BranchId
            };

            var token = _jwtService.CreateToken(payload);
            var refreshToken = _jwtService.RefreshToken();

            var RefreshTokenExpiration = Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DURATION_MINUTES")!;
            var tokenExpiry = int.Parse(RefreshTokenExpiration);
            user.RefreshTokenHash = _jwtService.HashToken(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(tokenExpiry);

            await _userManager.UpdateAsync(user);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = user.Email,
                Role = getUserRoles.FirstOrDefault() ?? "User",
                Action = "Login",
                Description = $"User {user.Email} logged in",
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();

            return new CreateJwtTokenDTO
            {
                Token =  token,
                RefreshToken = refreshToken
            };

        }

        public async Task Logout(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return;

            var roles = await _userManager.GetRolesAsync(user);

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = user.Email,
                Role = roles.FirstOrDefault() ?? "User",
                Action = "Logout",
                Description = $"User {user.Email} logged out",
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangePassword(ChangePassDTO changePassDTO)
        {
            var user = await _userManager.FindByEmailAsync(changePassDTO.Email);
            if (user == null) return false;
            var result = await _userManager.ChangePasswordAsync(user, changePassDTO.CurrentPassword, changePassDTO.NewPassword);
            
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                _context.AuditLog.Add(new AuditLogEntities
                {
                    Username = user.Email,
                    Role = roles.FirstOrDefault() ?? "User",
                    Action = "Change Password",
                    Description = $"User {user.Email} changed password",
                    Date = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return result.Succeeded;
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

            //Check if refresh token is expired
            if (!user.RefreshTokenExpiryTime.HasValue || user.RefreshTokenExpiryTime.Value <= DateTime.UtcNow)
                return null;

            //Check if refresh token matches
            bool isValidRefreshToken = _jwtService.VerifyHashedToken(user.RefreshTokenHash ?? "", requestDTO.RefreshToken);
            if (!isValidRefreshToken)
                return null;

            //Rotate refresh token, but keep expiry fixed
            var newRefreshToken = _jwtService.RefreshToken();
            var hashedRefreshToken = _jwtService.HashToken(newRefreshToken);

            //Update user with new refresh token but keep original expiry
            var trackUser = await _userManager.FindByIdAsync(user.Id);
            trackUser.RefreshTokenHash = hashedRefreshToken;
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
                RefreshToken = newRefreshToken
            };
        }
        
    }
}
