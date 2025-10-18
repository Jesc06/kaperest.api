using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Account;
using KapeRest.Application.DTOs.Jwt;

namespace KapeRest.Application.Interfaces.Account
{
    public interface IAccounts
    {
        Task<bool> RegisterAccount(RegisterAccountDTO register);
        Task<CreateJwtTokenDTO> Login(LoginDTO login);
        Task Logout(string email);
        Task<bool> ChangePassword(ChangePassDTO changePassDTO);
        Task<JwtRefreshResponseDTO> RefreshToken(JwtRefreshRequestDTO requestToken);
    }
}
