using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts
{
    public class PendingAccountRepo : IpendingAccount
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UsersIdentity> _userManager;
        public PendingAccountRepo(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> RegisterPending(PendingAccDTO pending)
        {
            var alreadyExists = await _context.PendingUserAccount.FirstOrDefaultAsync(x => x.Email == pending.Email);

            if(alreadyExists is not null)
               return "A pending account with this email already exists.";

            if(pending.Role == "Cashier")
            {
                var findExistence = await _context.Branches.FirstOrDefaultAsync(f => f.Id == pending.BranchId);
                if (findExistence is null)
                    return "branch is not exist";
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
                BranchId = pending.BranchId
            };

            _context.PendingUserAccount.Add(pendingUser);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = pending.Email,
                Role = pending.Role,
                Category = "Authentication",
                Action = "Pending Account",
                AffectedEntity = pending.Email,
                Description = "Processing pending account",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return "Successfully registered pending accounts!";
        }

        public async Task<string> ApprovePendingAccount(int id, string username, string role)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                return "Pending account not found.";
            
            if(pending.Status is not "Pending")
                throw new Exception("Account already proceed");

            var user = new UsersIdentity
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                UserName = pending.Email,
                Email = pending.Email,
                BranchId = pending.BranchId
            };

            var result = await _userManager.CreateAsync(user, pending.Password);
            if(!result.Succeeded)
                return "Failed to create user account. already exist username";

            await _userManager.AddToRoleAsync(user, pending.Role);

            _context.PendingUserAccount.Remove(pending);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = username,
                Role = role,
                Category = "Authentication",
                Action = "Approved Account",
                AffectedEntity = pending.Email,
                Description = "Approved pending account successfully registered.",
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
                Category = "Authentication",
                Action = "Rejected Account",
                AffectedEntity = pending.Email,
                Description = "Rejected pending account",
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
                Branch = new
                {
                    f.Branch.BranchName,
                    f.Branch.Location
                }
            }).ToListAsync();

            return list;
        }


    }
}
