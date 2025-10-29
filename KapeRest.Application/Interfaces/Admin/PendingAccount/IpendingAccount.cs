using KapeRest.Application.DTOs.Admin.PendingAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.PendingAccounts;
using System.Collections;

namespace KapeRest.Application.Interfaces.Admin.PendingAcc
{
    public interface IpendingAccount
    {
        Task<string> RegisterPending(PendingAccDTO pending);
        Task<string> ApprovePendingAccount(int id, string username, string role);
        Task<string> RejectPendingAccount(int id, string username, string role);
        Task<ICollection> GetPendingAccounts();
    }
}
