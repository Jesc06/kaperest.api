using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Account;

namespace KapeRest.Application.Interfaces.Account
{
    public interface IAccounts
    {
        Task<bool> RegisterAccount(RegisterAccountDTO register);
        Task<bool> Login(LoginDTO login);
    }
}
