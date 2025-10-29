using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Auth;
using KapeRest.Application.DTOs.Jwt;

namespace KapeRest.Application.Interfaces.Auth
{
    public interface IAccounts
    {
        Task<string> RegisterAccount(RegisterAccountDTO register);
        Task<CreateJwtTokenDTO> Login(LoginDTO login);
        Task Logout(string email);
        Task<bool> ChangePassword(ChangePassDTO changePassDTO);
        Task<JwtRefreshResponseDTO> RefreshToken(JwtRefreshRequestDTO requestToken);
    }
}
