using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.Interfaces.Auth;
using KapeRest.Application.DTOs.Auth;
using KapeRest.Application.DTOs.Jwt;
using KapeRest.Application.Interfaces.CurrentUserService;

namespace KapeRest.Application.Services.Auth
{
    public class AccountService
    {
        private IAccounts _accountRepository;
        private ICurrentUser _currentUser;
        public AccountService(IAccounts accountRepository, ICurrentUser currentUser)
        {
            _accountRepository = accountRepository;
            _currentUser = currentUser;
        }

        public async Task<int> TotalUsers()
        {
            return await _accountRepository.GetTotalUsersAsync();
        }
        public async Task<string> RegisterAccountService(RegisterAccountDTO register)
        {
            return await _accountRepository.RegisterAccount(register);
        }
       
        public async Task<CreateJwtTokenDTO> Login (LoginDTO login)
        {
            return await _accountRepository.Login(login);
        }

        public async Task Logout()
        {
            var email = _currentUser.Email;
            if (string.IsNullOrEmpty(email))
                throw new Exception("User is not logged in.");

            await _accountRepository.Logout(email);
        }

        public async Task<bool> ChangePassword(ChangePassDTO changePassDTO)
        {
            return await _accountRepository.ChangePassword(changePassDTO);
        }

        public async Task<JwtRefreshResponseDTO> TokenRefresh(JwtRefreshRequestDTO request)
        {
            return await _accountRepository.RefreshToken(request);
        }


    }
}
