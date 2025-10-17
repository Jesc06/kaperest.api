using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.Interfaces.Account;
using KapeRest.Application.DTOs.Account;

namespace KapeRest.Application.Services.Account
{
    public class AccountService
    {
        private IAccounts _accountRepository;
        public AccountService(IAccounts accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<bool> RegisterAccountService(RegisterAccountDTO register)
        {
            return await _accountRepository.RegisterAccount(register);
        }
        public async Task<bool> Login (LoginDTO login)
        {
            return await _accountRepository.Login(login);
        }

    }
}
