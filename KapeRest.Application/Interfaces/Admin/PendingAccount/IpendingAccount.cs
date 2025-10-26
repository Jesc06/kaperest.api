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
        Task ApprovePendingAccount(int id, string username, string role);
        Task RejectPendingAccount(int id, string username, string role);
        Task<List<PendingUserAccount>> GetPendingAccounts();
    }
}
