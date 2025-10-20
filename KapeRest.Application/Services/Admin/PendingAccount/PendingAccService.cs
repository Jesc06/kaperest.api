using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Domain.Entities.PendingAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.PendingAcc
{
    public class PendingAccService
    {
        private readonly IpendingAccount _pendingAccount;
        private ICurrentUser _currentUser;
        public PendingAccService(IpendingAccount pendingAccount, ICurrentUser currentUser)
        {
            _pendingAccount = pendingAccount;
            _currentUser = currentUser;
        }

        public async Task RegisterPending(PendingAccDTO pending)
        {
            await _pendingAccount.RegisterPending(pending);
        }

        public async Task ApprovePendingAccount(int id)
        {
            var username = _currentUser.Email;
            var role = _currentUser.Role;
            await _pendingAccount.ApprovePendingAccount(id,username,role);
        }

        public async Task RejectPendingAccount(int id)
        {
            var username = _currentUser.Email;
            var role = _currentUser.Role;
            await _pendingAccount.RejectPendingAccount(id,username,role);
        }

        public async Task<List<PendingUserAccount>> GetPendingAccounts()
        {
            return await _pendingAccount.GetPendingAccounts();
        }


    }
}
