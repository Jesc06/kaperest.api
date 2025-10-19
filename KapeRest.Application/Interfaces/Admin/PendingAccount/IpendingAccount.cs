using KapeRest.Application.DTOs.Admin.PendingAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.PendingAccounts;

namespace KapeRest.Application.Interfaces.Admin.PendingAcc
{
    public interface IpendingAccount
    {
        Task RegisterPending(PendingAccDTO pending);
        Task ApprovePendingAccount(int id);
        Task RejectPendingAccount(int id);
        Task<List<PendingUserAccount>> GetPendingAccounts();
    }
}
