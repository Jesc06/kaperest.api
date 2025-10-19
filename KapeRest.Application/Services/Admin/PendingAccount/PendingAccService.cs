using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
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
        public PendingAccService(IpendingAccount pendingAccount)
        {
            _pendingAccount = pendingAccount;
        }

        public async Task RegisterPending(PendingAccDTO pending)
        {
            await _pendingAccount.RegisterPending(pending);
        }

        public async Task ApprovePendingAccount(int id)
        {
            await _pendingAccount.ApprovePendingAccount(id);
        }

        public async Task RejectPendingAccount(int id)
        {
            await _pendingAccount.RejectPendingAccount(id);
        }

        public async Task<List<PendingUserAccount>> GetPendingAccounts()
        {
            return await _pendingAccount.GetPendingAccounts();
        }


    }
}
