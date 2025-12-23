using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Core.Entities.Branch;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.PendingAccounts
{
    public class PendingAccountRepository : IpendingAccount
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UsersIdentity> _userManager;
        public PendingAccountRepository(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> RegisterPending(PendingAccDTO pending)
        {
            var alreadyExists = await _context.PendingUserAccount
               .FirstOrDefaultAsync(x => x.Email == pending.Email);

            if (alreadyExists is not null)
                return "A pending account with this email already exists.";

            //Validate role rules
            if (pending.Role == "Cashier")
            {
                var findBranch = await _context.Branches
                    .FirstOrDefaultAsync(f => f.Id == pending.BranchId);
                if (findBranch is null)
                    return "Branch does not exist.";
            }
            else if (pending.Role == "Staff")
            {
                if (string.IsNullOrEmpty(pending.CashierId))
                    return "Staff must be linked to a cashier.";

                var cashierExists = await _userManager.FindByIdAsync(pending.CashierId);
                if (cashierExists == null)
                    return "Cashier does not exist.";
            }
            else
            {
                pending.BranchId = null;
            }

            var pendingUser = new PendingUserAccount
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                Email = pending.Email,
                Password = pending.Password,
                Role = pending.Role,
                Status = "Pending",
                BranchId = pending.BranchId,
                CashierId = pending.CashierId //Save Cashier link
            };

            _context.PendingUserAccount.Add(pendingUser);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = pending.Email,
                Role = pending.Role,
                Action = "Pending Account",
                Description = "Processing pending account",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return "Successfully registered pending account!";
        }

        public async Task<string> ApprovePendingAccount(int id, string username, string role)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                return "Pending account not found.";

            if (pending.Status is not "Pending")
                throw new Exception("Account already processed.");

            var user = new UsersIdentity
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                UserName = pending.Email,
                Email = pending.Email,
                BranchId = pending.BranchId,
                //assign Cashier link if Staff
                CashierId = pending.Role == "Staff" ? pending.CashierId : null
            };


            var branch = await _context.Branches
                .Where(b => b.Id == pending.BranchId)
                .FirstOrDefaultAsync();

            branch.Staff = $"{pending.FirstName} {pending.MiddleName} {pending.LastName}";
            branch.Status = "Active";


            var result = await _userManager.CreateAsync(user, pending.Password);
            if (!result.Succeeded)
                return "Failed to create user account. Already exists.";

            await _userManager.AddToRoleAsync(user, pending.Role);

            _context.PendingUserAccount.Remove(pending);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = username,
                Role = role,
                Action = "Approved Account",
                Description = $"Approved {user.FirstName} pending account {user.Email} successfully registered.",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return "Successfully approved!";
        }

        public async Task<string> RejectPendingAccount(int id, string username, string role)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                return "Pending account not found.";

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = username,
                Role = role,
                Action = "Rejected Account",
                Description = $"Rejected pending account {pending.Email}",
                Date = DateTime.Now
            });

            _context.PendingUserAccount.Remove(pending);
            await _context.SaveChangesAsync();
            return "Successfully rejected account!";
        }

        public async Task<ICollection> GetPendingAccounts()
        {
            var list = await _context.PendingUserAccount
               .Include(p => p.Branch)
               .Select(f => new
               {
                   f.Id,
                   f.FirstName,
                   f.LastName,
                   f.Email,
                   f.Role,
                   f.Status,
                   f.CashierId,
                   Branch = new
                   {
                       f.Branch.BranchName,
                       f.Branch.Location
                   }
               }).ToListAsync();

            return list;
        }

        public async Task<ICollection> ExistingCashierAccountNavigaiton()
        {
            var cashierAccount = await _context.UsersIdentity.Select(c => new
            {
                c.Id,
                c.UserName,
                c.BranchId,
                c.Branch.BranchName,
                c.Branch.Location
            }).ToListAsync();
           return cashierAccount;
        }


    }
}
