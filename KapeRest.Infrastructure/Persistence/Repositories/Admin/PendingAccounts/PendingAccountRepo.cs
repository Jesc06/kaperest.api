using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task RegisterPending(PendingAccDTO pending)
        {
            var alreadyExists = await _context.PendingUserAccount.FirstOrDefaultAsync(x => x.Email == pending.Email);
                if(alreadyExists is not null)
                    throw new Exception("A pending account with this email already exists.");

                var pendingUser = new PendingUserAccount
                {
                    FirstName = pending.FirstName,
                    MiddleName = pending.MiddleName,
                    LastName = pending.LastName,
                    Email = pending.Email,
                    Password = pending.Password,
                    Role = pending.Role,
                    Status = "Pending",
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
        }

        public async Task ApprovePendingAccount(int id, string username, string role)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                throw new Exception("Pending account not found.");
            
            if(pending.Status is not "Pending")
                throw new Exception("Account already proceed");

            var user = new UsersIdentity
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                UserName = pending.Email,
                Email = pending.Email,
            };

            var result = await _userManager.CreateAsync(user, pending.Password);
            if(!result.Succeeded)
                throw new Exception("Failed to create user account. already exist username");

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
        }

        public async Task RejectPendingAccount(int id, string username, string role)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                throw new Exception("Pending account not found.");

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
        }

        public async Task<List<PendingUserAccount>> GetPendingAccounts()
        {
            var list = await _context.PendingUserAccount.ToListAsync();
            return list;
        }


    }
}
